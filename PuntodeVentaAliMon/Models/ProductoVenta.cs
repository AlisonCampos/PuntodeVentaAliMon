using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    public class ProductoVenta
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}