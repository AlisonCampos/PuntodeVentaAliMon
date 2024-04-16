using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    public class VenCatEstado
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public virtual ICollection<VenVenta> VenVentas { get; set; }
    }
}