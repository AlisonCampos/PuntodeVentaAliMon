using PuntodeVentaAliMon.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PuntodeVentaAliMon.Controllers
{
    public class VentasController : Controller
    {
        private PuntoVentaContext _context = new PuntoVentaContext();
        public ActionResult CrearVenta()
        {
            ViewBag.Categorias = new SelectList(_context.ProCatCategorias, "id", "nombre");
            return View();
        }
        private List<VenVenta> ObtenerVentas()
        {
            // Aquí podrías realizar consultas a tu base de datos para obtener las ventas
            // Esto depende de tu estructura de base de datos y cómo estén modeladas tus ventas

            // Por ejemplo, podrías tener una tabla llamada "Ventas" con una clase modelo llamada Venta
            var ventas = _context.VenVenta.ToList();
            // Esto asume que tienes una tabla llamada "Ventas" en tu base de datos

            return ventas;
        }
        public ActionResult Ventas()
        {
            // Aquí podrías obtener los datos de ventas de tu base de datos u otro origen de datos
            var ventas = ObtenerVentas();

            return View(ventas);
        }
        public JsonResult GetSubcategorias(int idProCatCategoria)
        {
            var subcategorias = _context.ProCatSubcategorias
                .Where(s => s.id == idProCatCategoria)
                .Select(s => new { s.id, s.nombre })
                .ToList();
            return Json(subcategorias, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductos(int idProCatSubcategoria)
        {
            var productos = _context.ProProductos
                .Where(p => p.idProCatSubcategoria == idProCatSubcategoria)
                .Select(p => new { p.id, p.nombreProducto })
                .ToList();
            return Json(productos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDetallesProducto(int idProProducto)
        {
            var producto = _context.ProProductos
                .Where(p => p.id == idProProducto)
                .Select(p => new { p.precio, p.stock })
                .FirstOrDefault();

            if (producto == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(producto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FinalizarVenta(string folio, IEnumerable<ProductoVenta> productos)
        {
            // Asegurarse de que la lista de productos no esté vacía
            if (productos == null || !productos.Any())
            {
                TempData["ErrorMessage"] = "No hay productos para procesar la venta.";
                return RedirectToAction("Ventas"); // Cambia "Ventas" por el nombre de tu acción o vista de ventas
            }

            // Verificar si el usuario está logueado
            if (Session["UserID"] == null)
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Ventas"); // Cambia "Ventas" por el nombre de tu acción o vista de ventas
            }

            // Obtener el ID del usuario desde la sesión
            int userId = Convert.ToInt32(Session["UserID"]);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Crear un nuevo registro de venta
                    var nuevaVenta = new VenVenta
                    {
                        idUsuUsuario = userId, // Usar el ID del usuario de la sesión
                        folio = folio,
                        fechaVenta = DateTime.Now,
                        idVenCatEstado = 1 // Estado "En proceso"
                    };

                    _context.VenVenta.Add(nuevaVenta);
                    _context.SaveChanges();

                    foreach (var item in productos)
                    {
                        var producto = _context.ProProductos.FirstOrDefault(p => p.nombreProducto == item.Nombre && p.stock >= item.Cantidad);
                        if (producto == null)
                        {
                            // Manejar el caso en que no haya suficiente stock o el producto no exista
                            transaction.Rollback();
                            TempData["ErrorMessage"] = $"Stock insuficiente o producto no encontrado: {item.Nombre}";
                            return RedirectToAction("Ventas"); // Cambia "Ventas" por el nombre de tu acción o vista de ventas
                        }

                        // Actualizar el stock del producto
                        producto.stock -= item.Cantidad;

                        // Crear un registro en VenVentaProducto para cada producto vendido
                        var ventaProducto = new VenVentaProducto
                        {
                            idVenVenta = nuevaVenta.id,
                            idProProducto = producto.id,
                            cantidad = item.Cantidad,
                            total = item.Total // Asegúrate de que este total sea el precio por la cantidad vendida
                        };

                        _context.VenVentaProducto.Add(ventaProducto); // Asegúrate que el nombre de la propiedad sea el correcto según tu contexto
                    }

                    _context.SaveChanges(); // Guarda todos los cambios de los productos vendidos

                    // Actualizar el estado de la venta a "Completada"
                    nuevaVenta.idVenCatEstado = 2; // Estado "Completada"
                    _context.SaveChanges(); // Guarda el cambio de estado de la venta

                    transaction.Commit(); // Completa la transacción si todo fue exitoso

                    TempData["SuccessMessage"] = "Venta completada con éxito.";
                    return RedirectToAction("Ventas"); // Cambia "Ventas" por el nombre de tu acción o vista de ventas
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Revierte la transacción en caso de error
                    TempData["ErrorMessage"] = "Error al finalizar la venta: " + ex.Message;
                    return RedirectToAction("Ventas"); // Cambia "Ventas" por el nombre de tu acción o vista de ventas
                }
            }
        }

    }
}