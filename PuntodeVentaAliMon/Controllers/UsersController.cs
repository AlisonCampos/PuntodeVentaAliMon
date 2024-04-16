using PuntodeVentaAliMon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PuntodeVentaAliMon.Controllers
{
    public class UsersController : Controller
    {
        private PuntoVentaContext _context = new PuntoVentaContext();

        public ActionResult Buscar(string nombreUsuario, int? usuCatEstado, int? usuCatTipoUsuario)
        {
            var usuarios = _context.UsuUsuarios.AsQueryable();

            if (!string.IsNullOrEmpty(nombreUsuario))
            {
                usuarios = usuarios.Where(u => u.nombreUsuario.Contains(nombreUsuario));
            }

            if (usuCatEstado.HasValue)
            {
                usuarios = usuarios.Where(u => u.usuCatEstado == usuCatEstado.Value);
            }

            if (usuCatTipoUsuario.HasValue)
            {
                usuarios = usuarios.Where(u => u.usuCatTipoUsuario == usuCatTipoUsuario.Value);
            }

            return View(usuarios.ToList());
        }
        public ActionResult Edit(int id)
        {
            var usuario = _context.UsuUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre", usuario.usuCatEstado);
            ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre", usuario.usuCatTipoUsuario);

            return View(usuario);
        }


        [HttpPost]
        public ActionResult Edit(UsuUsuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre de usuario contiene solo espacios
                if (string.IsNullOrWhiteSpace(usuario.nombreUsuario))
                {
                    ModelState.AddModelError("", "El nombre de usuario no puede contener solo espacios. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre", usuario.usuCatEstado);
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre", usuario.usuCatTipoUsuario);
                    return View(usuario);
                }

                // Verificar si el nombre de usuario contiene caracteres especiales
                if (!Regex.IsMatch(usuario.nombreUsuario, @"^[a-zA-Z0-9]+$"))
                {
                    ModelState.AddModelError("", "El nombre de usuario no puede contener caracteres especiales. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre", usuario.usuCatEstado);
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre", usuario.usuCatTipoUsuario);
                    return View(usuario);
                }

                // Verificar si el nombre de usuario ya existe
                var usuarioExistente = _context.UsuUsuarios.FirstOrDefault(u => u.nombreUsuario == usuario.nombreUsuario && u.Id != usuario.Id);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("", "El nombre de usuario ya existe. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre", usuario.usuCatEstado);
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre", usuario.usuCatTipoUsuario);
                    return View(usuario);
                }

                _context.Entry(usuario).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Buscar");
            }
            ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre", usuario.usuCatEstado);
            ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre", usuario.usuCatTipoUsuario);
            return View(usuario);
        }


        public ActionResult Delete(int id)
        {
            var usuario = _context.UsuUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.UsuUsuarios.Find(id);
            _context.UsuUsuarios.Remove(usuario);
            _context.SaveChanges();
            return RedirectToAction("Buscar");
        }

        public ActionResult Create()
        {
            ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre");
            ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Create(UsuUsuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre de usuario contiene solo espacios
                if (string.IsNullOrWhiteSpace(usuario.nombreUsuario))
                {
                    ModelState.AddModelError("", "El nombre de usuario no puede contener solo espacios. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre");
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre");
                    return View(usuario);
                }

                // Verificar si el nombre de usuario contiene caracteres especiales
                if (!Regex.IsMatch(usuario.nombreUsuario, @"^[a-zA-Z0-9]+$"))
                {
                    ModelState.AddModelError("", "El nombre de usuario no puede contener caracteres especiales. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre");
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre");
                    return View(usuario);
                }

                // Verificar si el nombre de usuario ya existe
                var usuarioExistente = _context.UsuUsuarios.FirstOrDefault(u => u.nombreUsuario == usuario.nombreUsuario);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("", "El nombre de usuario ya existe. Por favor, elige otro.");
                    ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre");
                    ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre");
                    return View(usuario);
                }

                _context.UsuUsuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Buscar");
            }
            ViewBag.usuCatEstado = new SelectList(_context.UsuCatEstado, "Id", "Nombre");
            ViewBag.usuCatTipoUsuario = new SelectList(_context.UsuCatTipoUsuarios, "Id", "Nombre");
            return View(usuario);
        }


    }
}