using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using DocumentFormat.OpenXml.Vml.Office;
// referencia para control de sesion que no permita naver por url
using System.Web.Security;

namespace CapaPresentacionAdmin.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CambiarClave()
        {
            return View();
        }

        public ActionResult ReestablecerClave()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {


            Usuario oUsuario = new Usuario();
            oUsuario = new CN_Usuarios().Listar().Where(x => x.Correo == correo && x.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();


            if (oUsuario==null)
            {
                ViewBag.Error = "Usuario o clave incorrecta";
                return View();
            }
            else
            {

                // el campo reestablecer es para saber si el usuario debe cambiar la clave cuando esta en 1 debe restablecerla 
                if(oUsuario.Reestablecer)
                {
                 TempData["IdUsuario"] = oUsuario.IdUsuario;
                    return RedirectToAction("CambiarClave");
                }
                ViewBag.Error = null;

               FormsAuthentication.SetAuthCookie(oUsuario.Correo, false);
                return RedirectToAction("Index", "Home");
            }

           
        }


        [HttpPost]
        public ActionResult CambiarClave(string idusuario , string claveactual, string nuevaclave, string confirmarclave)
        {

            if (nuevaclave != confirmarclave)
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"]= claveactual;
                ViewBag.Error = "Las claves no coinciden";
                return View();
            }


            Usuario oUsuario = new Usuario();
            oUsuario = new CN_Usuarios().Listar().Where(u => u.IdUsuario == int.Parse(idusuario)).FirstOrDefault(); ;

            if (oUsuario.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"] = "";
                ViewBag.Error = "Clave actual incorrecta";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);
            string mensaje = string.Empty;
            bool respuesta = new CN_Usuarios().CambiarClave(int.Parse(idusuario), nuevaclave, out mensaje);
            if (respuesta)
            {                 //ViewBag.Error = "Clave cambiada con exito";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdUsuario"] = idusuario;
                ViewBag.Error = mensaje;
                return View();
            }

           
        }



        [HttpPost]
        public ActionResult ReestablecerClave(string correo)
        {
            Usuario oUsuario = new Usuario();
            oUsuario = new CN_Usuarios().Listar().Where(x => x.Correo == correo).FirstOrDefault();

            if (oUsuario == null)
            {
                ViewBag.Error = "Correo no registrado";
                return View();
            }
            else
            {
                string clave = CN_Recursos.GenerarClave();
                string mensaje = string.Empty;
                bool respuesta = new CN_Usuarios().ReestablecerClave(oUsuario.IdUsuario, correo, out mensaje);
                if (respuesta)
                {
                    ViewBag.Error = "Clave reestablecida con exito";
                    return RedirectToAction("Index","Acceso");
                }
                else
                {
                    ViewBag.Error = mensaje;
                    return View();
                }
            }
        }
        

        public ActionResult CerrarSesion()
        {        
            FormsAuthentication.SignOut();
            Session["Usuario"] = null;
            return RedirectToAction("Index","Acceso");
        }   
    }
}
