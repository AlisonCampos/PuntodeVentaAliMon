using PuntodeVentaAliMon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PuntodeVentaAliMon.Controllers
{
    public class ProductsController : Controller
    {
        private PuntoVentaContext _context = new PuntoVentaContext();

        public ActionResult Buscar(string nombreProducto, int? idProCatCategoria, int? idProCatSubcategoria)
        {
            var productos = _context.ProProductos.AsQueryable();

            if (!string.IsNullOrEmpty(nombreProducto))
            {
                productos = productos.Where(p => p.nombreProducto.Contains(nombreProducto));
            }

            if (idProCatCategoria.HasValue)
            {
                productos = productos.Where(p => p.idProCatCategoria == idProCatCategoria.Value);
            }

            if (idProCatSubcategoria.HasValue)
            {
                productos = productos.Where(p => p.idProCatSubcategoria == idProCatSubcategoria.Value);
            }

            return View(productos.ToList());
        }


        [HttpGet]
        public ActionResult Editar(int id)
        {
            var producto = _context.ProProductos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre", producto.idProCatCategoria);
            ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre", producto.idProCatSubcategoria);

            return View(producto);
        }

        [HttpPost]
        public ActionResult Editar(ProProducto producto, HttpPostedFileBase imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imagen.InputStream))
                    {
                        producto.image = reader.ReadBytes(imagen.ContentLength);
                    }
                }

                try
                {
                    _context.Entry(producto).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Buscar");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el producto: " + ex.Message);
                }
            }
            ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre", producto.idProCatCategoria);
            ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre", producto.idProCatSubcategoria);
            return View(producto);
        }



        public ActionResult Eliminar(int id)
        {
            var producto = _context.ProProductos.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        [HttpPost, ActionName("Eliminar")]
        public ActionResult ConfirmarEliminar(int id)
        {
            var producto = _context.ProProductos.Find(id);
            _context.ProProductos.Remove(producto);
            _context.SaveChanges();
            return RedirectToAction("Buscar");
        }

        public ActionResult MostrarImagen(int id)
        {
            var producto = _context.ProProductos.Find(id);
            if (producto != null && producto.image != null)
            {
                return File(producto.image, "image/jpeg");
            }
            else
            {
                return null;
            }
        }
        public ActionResult Agregar()
        {
            ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre");
            ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(ProProducto producto, HttpPostedFileBase imagen)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre del producto contiene solo espacios
                if (string.IsNullOrWhiteSpace(producto.nombreProducto))
                {
                    ModelState.AddModelError("", "El nombre del producto no puede contener solo espacios. Por favor, elige otro.");
                    ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre", producto.idProCatCategoria);
                    ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre", producto.idProCatSubcategoria);
                    return View(producto);
                }

                // Verificar si el nombre del producto contiene caracteres especiales
                if (!Regex.IsMatch(producto.nombreProducto, @"^[a-zA-Z0-9\s]+$"))
                {
                    ModelState.AddModelError("", "El nombre del producto no puede contener caracteres especiales. Por favor, elige otro.");
                    ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre", producto.idProCatCategoria);
                    ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre", producto.idProCatSubcategoria);
                    return View(producto);
                }

                if (imagen != null && imagen.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(imagen.InputStream))
                    {
                        producto.image = reader.ReadBytes(imagen.ContentLength);
                    }
                }

                _context.ProProductos.Add(producto);
                _context.SaveChanges();
                return RedirectToAction("Buscar");
            }
            ViewBag.idProCatCategoria = new SelectList(_context.ProCatCategorias, "id", "nombre", producto.idProCatCategoria);
            ViewBag.idProCatSubcategoria = new SelectList(_context.ProCatSubcategorias, "id", "nombre", producto.idProCatSubcategoria);
            return View(producto);
        }

    }
}