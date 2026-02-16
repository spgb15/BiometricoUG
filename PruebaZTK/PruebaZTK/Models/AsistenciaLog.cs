using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PruebaZTK.Models
{
    [Table("Asistencias")]
    public class AsistenciaLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UsuarioId { get; set; }
        [Required]
        public DateTime FechaHora { get; set; }
        public int Estado { get; set; }
        public int TipoVerificacion { get; set; }
        public DateTime FechaRegistroServidor { get; set; } = DateTime.Now;
        public string DispositivoSN { get; set; }

    }
}
