using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Text;

namespace BiometricoZTK.Controllers
{
    [ApiController]
    [Route("iclock")]
    public class ZKController : ControllerBase
    {
        private readonly IZKProcessingService _zkService;
        private readonly IZKCommandService _zkCommandService;

        public ZKController(IZKProcessingService zkService, IZKCommandService zkCommandService)
        {
            _zkService = zkService;
            _zkCommandService = zkCommandService;
        }

        [HttpGet("cdata")]
        public IActionResult GetCData([FromQuery] string? SN)
        {
            Console.WriteLine($"[GET] Verificación de equipo SN: {SN}");
            return Ok("OK");
        }

        [HttpPost("cdata")]
        public async Task<IActionResult> ReceiveData([FromQuery] string? SN, [FromQuery] string? table)
        {
            if(table == "ALARM")
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, Encoding.ASCII);
                string Alarma = await reader.ReadToEndAsync();

                Console.WriteLine("$\"!!! ALERTA DE SEGURIDAD - EQUIPO: {SN} !!!\"");
                Console.WriteLine($"Detalle del sabotaje: {Alarma}");

                return Ok("OK");
            }

            if (table != "ATTLOG") return Ok("OK");

            try
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, Encoding.ASCII, leaveOpen: true);
                string body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    await _zkService.ProcesarTextoPlano(body, SN ?? "Desconocido");
                }
            }
            catch (Exception ex)
            {
                // Silenciar error para no bloquear el reloj
            }

            return Ok("OK");
        }

        [HttpGet("getrequest")]
        public async Task<IActionResult> HeartBeat([FromQuery] string sn) {
            string respuesta = await _zkCommandService.GetPendingCommands(sn);
            return Ok(respuesta);
        }

        [HttpPost("devicecmd")]
        public async Task<IActionResult> DeviceCommandResponse([FromQuery] string SN)
        {
            using var reader = new StreamReader(Request.Body, Encoding.ASCII);
            string responseBody = await reader.ReadToEndAsync();

            Console.WriteLine($"[COMANDO FINALIZADO] Equipo: {SN}, Respuesta: {responseBody}");

            return Ok("OK");
        }
    }
}
