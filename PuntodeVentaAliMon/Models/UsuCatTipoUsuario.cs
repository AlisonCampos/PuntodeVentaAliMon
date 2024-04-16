using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    [Table("UsuCatTipoUsuario")]
    public class UsuCatTipoUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}