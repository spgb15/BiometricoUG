using PruebaZTK.Data;
using PruebaZTK.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PruebaZTK.Services
{
    public class BioServer
    {
        private int _port;
        private bool _isRunning;

        public BioServer(int port)
        {
            _port = port;
        }

        static ConcurrentQueue<string> ColaComandos = new ConcurrentQueue<string>();
        static int ContadorComandos = 1;

        public void Iniciar()
        {
            TcpListener server = new TcpListener(IPAddress.Any, _port);
            server.Start();
            _isRunning = true;
            Console.WriteLine($"Servidor TCP escuchando en el puerto {_port}...");

            while (_isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread t = new Thread(() => ProcesarCliente(client));
                t.Start();
            }
        }

        private void ProcesarCliente(TcpClient client)
        {
            try
            {

                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[8096];

                    while (client.Connected)
                    {
                        if (!stream.DataAvailable)
                        {
                            Thread.Sleep(10);
                            if (client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0) break;
                            continue;
                        }

                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string rawData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        string serialNumber = ExtraerSN(rawData);

                        if (rawData.Contains("table=ATTLOG"))
                        {
                            GuardarFichaje(rawData, serialNumber);
                        }
                        else if (rawData.Contains("getrequest"))
                        {
                            if (ColaComandos.TryDequeue(out string comandoPendiente))
                            {
                                Console.WriteLine($"\n<< ENVIANDO COMANDO AL RELOJ: {comandoPendiente}");
                            }
                        }
                        string responseBody = "OK";

                        string header = "HTTP/1.1 200 OK\r\n" +
                            "Content-Type: text/plain\r\n" +
                            "Content-Length: " + responseBody.Length + "\r\n" +
                            "\r\n" +
                            responseBody;

                        byte[] responseBytes = Encoding.ASCII.GetBytes(header);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                        stream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private string ExtraerSN(string rawData)
        {
            try
            {
                int indexSN = rawData.IndexOf("SN=");
                if(indexSN != -1)
                {
                    int start = indexSN + 3;
                    int end = rawData.IndexOf("&", start);
                    if (end == -1) end = rawData.IndexOf(" ", start);

                    if(end != -1)
                    {
                        return rawData.Substring(start, end - start);
                    }
                }
            }
            catch { }
            return "Desconocido";
        }

        private void GuardarFichaje(string rawData, string sn)
        {
            using (var db = new AppDbContext())
            {
                var lineas = rawData.Split("\n");
                int guardados = 0;

                foreach (var linea in lineas)
                {
                    var datos = linea.Trim().Split('\t', ' ');

                    if (datos.Length >= 4 && DateTime.TryParse(datos[1] + " " + datos[2], out DateTime fecha))
                    {
                        var nuevoFichaje = new AsistenciaLog
                        {
                            UsuarioId = datos[0],
                            FechaHora = fecha,
                            Estado = int.Parse(datos[3]),
                            TipoVerificacion = int.Parse(datos[4]),
                            DispositivoSN = sn
                        };

                        db.Asistencias.Add(nuevoFichaje);
                        guardados++;
                    }
                }

                if (guardados > 0)
                {
                    db.SaveChanges();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Datos guardados");
                    Console.ResetColor();
                }
            }
        }

        static List<AsistenciaLog> ParsearAttLog(string rawHttp)
        {
            var lista = new List<AsistenciaLog>();

            string[] partesHttp = rawHttp.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (partesHttp.Length < 2) return lista;

            string body = partesHttp[1];

            string[] lineas = body.Split("\n");

            foreach (string linea in lineas)
            {
                string[] datos = linea.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (datos.Length >= 4)
                {
                    try
                    {
                        lista.Add(new AsistenciaLog
                        {
                            UsuarioId = datos[0],
                            FechaHora = DateTime.Parse(datos[1] + " " + datos[2]),
                            Estado = int.Parse(datos[3]),
                            TipoVerificacion = int.Parse(datos[4])
                        });
                    }
                    catch { }
                }
            }
            return lista;
        }

        static void AgregarComando(string cmd)
        {
            ColaComandos.Enqueue(cmd);
        }
    }
}
