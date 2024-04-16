using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    public class VenVentaProducto
    {
        public int id { get; set; }
        public int idVenVenta { get; set; }
        public int idProProducto { get; set; }
        public float cantidad { get; set; }
        public decimal total { get; set; }
        public virtual VenVenta VenVenta { get; set; }
        public virtual ProProducto ProProducto { get; set; }
    }
}