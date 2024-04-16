using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    public class VenVenta
    {
        public int id { get; set; }
        public int idUsuUsuario { get; set; }
        public string folio { get; set; }
        public DateTime fechaVenta { get; set; }
        public int idVenCatEstado { get; set; }
        public virtual VenCatEstado VenCatEstado { get; set; }
        public virtual ICollection<VenVentaProducto> VenVentaProductos { get; set; }
    }
}