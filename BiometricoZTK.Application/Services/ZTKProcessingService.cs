using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Domain.Entities;
using BiometricoZTK.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Services
{
    public class ZTKProcessingService : IZKProcessingService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public ZTKProcessingService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task ProcesarTextoPlano(string rawBody, string SerialNumber)
        {
            var logsEntrantes = ParsearCuerpo(rawBody, SerialNumber);
            if (!logsEntrantes.Any()) return;

            var fechaMin = logsEntrantes.Min(l => l.FechaHora);
            var fechaMax = logsEntrantes.Max(l => l.FechaHora);

            var existentes = await _attendanceRepository.GetExistingLogAsync(SerialNumber, fechaMin, fechaMax);

            var nuevosLogs = logsEntrantes.Where(en =>
            !existentes.Any(ex => 
                ex.UsuarioId == en.UsuarioId && 
                ex.FechaHora == en.FechaHora)).ToList();

            if(nuevosLogs.Any())
            {
                await _attendanceRepository.AddRangeAsync(nuevosLogs);
            }
        }

        private List<ActividadLog> ParsearCuerpo(string rawBody, string serialNumber)
        {
            var lista = new List<ActividadLog>();

            string[] lineas = rawBody.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linea in lineas)
            {
                var columnas = linea.Trim().Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (columnas.Length >= 4)
                {
                    try
                    {
                        string fechaCompletaStr = $"{columnas[1]} {columnas[2]}";

                        if (DateTime.TryParse(fechaCompletaStr, out DateTime fechaHora))
                        {
                            var log = new ActividadLog(
                                usuarioId: columnas[0],
                                fechaHora: fechaHora,
                                estado: int.Parse(columnas[3]),
                                tipoVerificacion: int.Parse(columnas[4]),
                                dispositivoSN: serialNumber
                            );

                            lista.Add(log);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return lista;
        }
    }
}
