using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    [Table("UsuUsuarios")]
    public class UsuUsuario
    {
        [Key]
        public int Id { get; set; }
        public string nombreUsuario { get; set; }
        public string contrasena { get; set; }

        // Claves foráneas
        public int usuCatEstado { get; set; }
        public int usuCatTipoUsuario { get; set; }

        // Propiedades de navegación
        [ForeignKey("usuCatEstado")]
        public virtual UsuCatEstado Estado { get; set; }
        [ForeignKey("usuCatTipoUsuario")]
        public virtual UsuCatTipoUsuario TipoUsuario { get; set; }
    }
}