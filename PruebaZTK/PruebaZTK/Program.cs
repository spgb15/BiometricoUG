using PruebaZTK.Data;
using PruebaZTK.Services;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

class Program
{


    static void Main(string[] args)
    {
        Console.Title = "Servidor Biométrico ADMS v1.0";

        Console.WriteLine("Iniciando motor de base de datos");
        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
            Console.WriteLine("Base de datos lista");
        }

        var servidor = new BioServer(8081);
        servidor.Iniciar();
    }
}