using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Configuration;
using System.IO;

namespace CapaPresentacionAdmin.Controllers
{
    [Authorize]
    public class MantenedorController : Controller
    {


        public ActionResult Categorias()
        {
            return View();
        }

        public ActionResult Marcas()
        {
            return View();
        }

        public ActionResult Productos()
        {
            return View();
        }

        //CATEGORIAS
        #region categorias
        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> oLista = new List<Categoria>();
            oLista = new CN_Categoria().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarCategoria(Categoria objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdCategoria == 0)
            {
                resultado = new CN_Categoria().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Categoria().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Categoria().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
        #endregion categorias
        //MARCAS
        #region marcas
        [HttpGet]
        public JsonResult ListarMarcas()
        {
            List<Marca> oLista = new List<Marca>();
            oLista = new CN_Marca().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {
                resultado = new CN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Marca().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        #endregion marcas

        //PRODUCTOS
        #region Productos
        [HttpGet]
        public JsonResult ListarProductos()
        {
            List<Producto> oLista = new List<Producto>();
            oLista = new CN_Producto().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoimagen)
        {
            string mensaje = string.Empty;
            bool operacion_exitosa = true;
            bool guardar_imagen_exito = true;

            Producto oProducto = new Producto();
            oProducto = JsonConvert.DeserializeObject<Producto>(objeto);

            decimal precio;

            if (decimal.TryParse(oProducto.PrecioTexto, NumberStyles.AllowDecimalPoint, new CultureInfo("es-CO"), out precio))
            {
                oProducto.Precio = precio;
            }
            else
            {
                return Json(new { operacionExitosa = false, mensaje = "El formato de precio debe ser ##.##" }, JsonRequestBehavior.AllowGet);

            }

            if (oProducto.IdProducto == 0)
            {
                int idProductoGenerado = new CN_Producto().Registrar(oProducto, out mensaje);
                if(idProductoGenerado != 0)
                {
                    oProducto.IdProducto = idProductoGenerado;

                }
                else {                    
                    operacion_exitosa = false;
                }


            }
            else
            {
                operacion_exitosa = new CN_Producto().Editar(oProducto, out mensaje);
            }

            if (operacion_exitosa)
            {
                if (archivoimagen != null)
                {
                    string ruta_guardar = ConfigurationManager.AppSettings["ServidorFotos"];
                    string extension = Path.GetExtension(archivoimagen.FileName);
                    string nombre_imagen = string.Concat(oProducto.IdProducto.ToString(), extension);
                    try
                    {
                        archivoimagen.SaveAs(Path.Combine(ruta_guardar, nombre_imagen));
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardar_imagen_exito = false;
                    }

                    if (guardar_imagen_exito)
                    {
                        oProducto.RutaImagen = ruta_guardar;
                        oProducto.NombreImagen = nombre_imagen;
                        bool rspta = new CN_Producto().GuardarDatosImagen(oProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardo el producto  pero hubo problemas con la imagen";

                    }
                }
            }

            return Json(new { operacionExitosa = operacion_exitosa, idGenerado = oProducto.IdProducto,mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        //registro de imagen



        [HttpPost]
        public JsonResult ImagenProducto(int id)
        {
            bool conversion;
            Producto oProducto = new CN_Producto().Listar().Where(p =>p.IdProducto ==id).FirstOrDefault();
            string textoBase64= CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);

            return Json(new
            { 
                conversion=conversion,                         
                textobase64=textoBase64,
                extension=Path.GetExtension(oProducto.NombreImagen)}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {

            if (id <= 0)
            {
                return Json(new { resultado = false, mensaje = "ID inválido." });
            }
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Producto().Eliminar(id, out mensaje);
            return Json(new { resultado = respuesta, mensaje = mensaje });
        }

        #endregion Productos
    }
}