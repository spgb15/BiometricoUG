using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Domain.Entities
{
    public class ActividadLog
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public int Estado { get; set; }
        public int TipoVerificacion { get; set; }
        public string DispositivoSN { get; set; }
        public DateTime FechaProcesado { get; set; } = DateTime.Now;

        public ActividadLog(string usuarioId, DateTime fechaHora, int estado, int tipoVerificacion, string dispositivoSN)
        {
            if (string.IsNullOrEmpty(usuarioId)) throw new ArgumentException("El ID de usuario no puede ser nulo o vacío.", nameof(usuarioId));
            UsuarioId = usuarioId;
            FechaHora = fechaHora;
            Estado = estado;
            TipoVerificacion = tipoVerificacion;
            DispositivoSN = dispositivoSN;
        }

        protected ActividadLog() { }
    }
}
