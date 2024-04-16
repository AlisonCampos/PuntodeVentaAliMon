using PuntodeVentaAliMon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntodeVentaAliMon.Controllers
{
    public class AccountController : Controller
    {
        private PuntoVentaContext _context = new PuntoVentaContext();
        private SqlConnection con;
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["PuntoVentaDatabase"].ToString();
            con = new SqlConnection(constr);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyUser(string nombreUsuario, string contrasena)
        {
            connection();
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from UsuUsuarios where nombreUsuario=@nombreUsuario and contrasena=@contrasena", con);
            cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            cmd.Parameters.AddWithValue("@contrasena", contrasena);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                Session["UserID"] = dr["id"].ToString();
                Session["UserType"] = dr["usuCatTipoUsuario"].ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "El nombre de usuario o la contraseña es incorrecta.");
                return View("Login");
            }
        }


        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                int userType = Convert.ToInt32(Session["UserType"]);

                switch (userType)
                {
                    case 1:  
                        return RedirectToAction("AdminView", "Account");
                    case 2:  
                        return RedirectToAction("UserView", "Account");
                    default:
                        return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult AdminView()
        {
            if (Session["UserID"] != null && Convert.ToInt32(Session["UserType"]) == 1)
            {
                var usuarios = _context.UsuUsuarios.Include(u => u.Estado).Include(u => u.TipoUsuario).ToList();
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult UserView()
        {
            if (Session["UserID"] != null && Convert.ToInt32(Session["UserType"]) == 2)
            {
                var usuarios = _context.UsuUsuarios.Include(u => u.Estado).Include(u => u.TipoUsuario).ToList();
                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}