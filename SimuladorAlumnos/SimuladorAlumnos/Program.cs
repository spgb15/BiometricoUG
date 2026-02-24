// Bucle para crear 20 "biométricos falsos"
using System.Net;

Console.WriteLine("Iniciando simulación de biométricos...");

Parallel.For(0, 300, i =>
{
    string serialFalso = $"BIO-FACULTAD-{i}";
    using (var cliente = new WebClient())
    {
        // Simulamos un fichaje
        string url = $"http://10.87.103.245:8081/iclock/cdata?SN={serialFalso}&table=ATTLOG";
        string data = $"999{i}\t2026-02-15 08:00:00\t1\t15\t0\t0\t0\t0";

        try
        {
            cliente.UploadString(url, data);
            Console.WriteLine($"Biométrico {i} envió datos correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en biométrico {i}: {ex.Message}");
        }
    }
});

Console.WriteLine("Simulación completada. Presiona cualquier tecla para salir...");
Console.ReadKey();