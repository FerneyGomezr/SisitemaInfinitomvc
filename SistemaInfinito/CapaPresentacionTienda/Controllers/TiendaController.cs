using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaNegocio;
using CapaEntidad;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using CapaEntidad.Paypal;
using System.Globalization;
using CapaPresentacionTienda.Filter;

namespace CapaPresentacionTienda.Controllers
{
    public class TiendaController : Controller
    {
        // GET: Tienda
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DetalleProducto(int idproducto = 0)
        {
            Producto oProducto = new Producto();
            bool conversion;
            oProducto= new CN_Producto().Listar().Where(p=>p.IdProducto == idproducto).FirstOrDefault();
            if(oProducto != null)
            {
                oProducto.base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);
                oProducto.extension = Path.GetExtension(oProducto.NombreImagen);
            }


        return View(oProducto); 
        
        }    

        [HttpGet]
        public ActionResult ListaCategorias()
        {
            List<Categoria> lista = new CN_Categoria().Listar();

            lista = new CN_Categoria().Listar();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);    
        }

        [HttpPost]
        public ActionResult ListaMarcaPorCategoria(int idcategoria)
        {
            List<Marca> lista= new List<Marca>();
                
            lista = new CN_Marca().ListarMarcaPorCategoria(idcategoria);

         
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ListarProducto(int idcategoria, int idmarca)
        {
            List<Producto> lista = new List<Producto>();

            bool conversion;
            lista = new CN_Producto().Listar().Select(p => new Producto()
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen,
                base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out conversion),
                extension=Path.GetExtension(p.NombreImagen),
                Activo = p.Activo
            }).Where(p=> 
            p.oCategoria.IdCategoria == (idcategoria ==0 ? p.oCategoria.IdCategoria :idcategoria) && 
            p.oMarca.IdMarca == (idmarca ==0 ? p.oMarca.IdMarca : idmarca) && 
            p.Stock > 0 && p.Activo == true ).ToList();
            var jsonresult = Json(new { data = lista }, JsonRequestBehavior.AllowGet);  
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;
        }

        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto)
        {
            Cliente cliente = Session["Cliente"] as Cliente;
            int idcliente = cliente == null ? 0 : cliente.IdCliente;
            //int idcliente = Session["Cliente"] == null ? 0 : ((Cliente)Session["Cliente"]).IdCliente;
            
         

            bool existe = new CN_Carrito().ExisteCarrito(idcliente, idproducto);
            bool respuesta = false;
            string mensaje = string.Empty;
            if (existe)
            {
                mensaje = "El producto ya existe ene el carrito";
             }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            }
           
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CantidadEnCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            int cantidad = new CN_Carrito().CantidadEnCarrito(idcliente);
            return Json(new { cantidad = cantidad }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarProductosCarrito()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            List<Carrito> olista = new List<Carrito>();
            bool conversion;
            olista= new CN_Carrito().ListarProducto(idcliente).Select(p => new Carrito()
            {
                oProducto = new Producto()
                {
                    IdProducto = p.oProducto.IdProducto,
                    Nombre = p.oProducto.Nombre,
                    oMarca = p.oProducto.oMarca,
                    Precio = p.oProducto.Precio,
                    RutaImagen = p.oProducto.RutaImagen,
                    base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.oProducto.RutaImagen, p.oProducto.NombreImagen), out conversion),
                    extension = Path.GetExtension(p.oProducto.NombreImagen),
                    
                },
                Cantidad = p.Cantidad
            }).ToList();
            
          
            return Json(new { data = olista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult OperacionCarrito(int idproducto, bool sumar)
        {
            //Cliente cliente = Session["Cliente"] as Cliente;
            //int idcliente = cliente == null ? 0 : cliente.IdCliente;
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCarrito(int idproducto)
        {
            bool respuesta = false;
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            string mensaje= string.Empty;
            respuesta = new CN_Carrito().EliminarCarrito(idcliente, idproducto);
            return Json(new { respuesta = respuesta }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObtenerDepartamento()
        {
            List<Departamento> lista = new CN_Ubicacion().ObtenerDepartamento();
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObtenerMunicipio(string IdDepartamento)
        {
            List<Municipio> lista = new CN_Ubicacion().ObtenerMunicipio(IdDepartamento);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }
        [ValidarSession]
        [Authorize]
        public ActionResult Carrito()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)      {
            decimal total = 0;
            DataTable detalle_venta=new DataTable();
            detalle_venta.Locale=new System.Globalization.CultureInfo("es-CO");
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("TotaL", typeof(decimal));


            List<Item> oLista = new List<Item>();


           foreach (var item in oListaCarrito)
           {
                decimal subtotal =Convert.ToDecimal(item.Cantidad.ToString()) * item.oProducto.Precio;
                total += subtotal;
                oLista.Add(new Item()
                {
                    name = item.oProducto.Nombre,
                    quantity = item.Cantidad.ToString(),
                    unit_amount = new UnitAmount()
                    {
                        currency_code = "USD",
                        value = item.oProducto.Precio.ToString("G",new CultureInfo("es-CO"))
                    }
                });
                detalle_venta.Rows.Add(new object[]
                {
                    item.oProducto.IdProducto,
                    item.Cantidad,
                    subtotal

                });
                
           }

            PurchaseUnit purchaseUnit = new PurchaseUnit()
            {
                amount = new Amount()
                {
                    currency_code = "USD",
                    value = total.ToString("G", new CultureInfo("es-CO")),
                    breakdown = new Breakdown()
                    {
                        item_total = new ItemTotal()
                        {
                            currency_code = "USD",
                            value = total.ToString("G", new CultureInfo("es-CO"))
                        }
                    }
                },
                description = "Compra de productos",
                items = oLista
            };

            Checkout_Order oCheckOutOrder = new Checkout_Order()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>()
                {
                    purchaseUnit
                },
                application_context = new ApplicationContext()
                {
                    brand_name = "Cosméticos Infinito",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = "https://localhost:44372/Tienda/PagoEfectuado",
                    cancel_url = "https://localhost:44372/Tienda/Carrito"
                }
            };

           oVenta.MontoTotal = total;
           oVenta.IdCliente = ((Cliente)Session["Cliente"]).IdCliente;
           TempData["Venta"] = oVenta;
           TempData["DetalleVenta"] = detalle_venta;


            CN_Paypal oPaypal = new CN_Paypal();
            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();
            response_paypal= await oPaypal.CrearSolicitud(oCheckOutOrder);

            return Json(response_paypal, JsonRequestBehavior.AllowGet);
        }

        [ValidarSession]
        [Authorize]
        public async Task<ActionResult> PagoEfectuado() { 
          string token = Request.QueryString["token"];
            CN_Paypal oPaypal = new CN_Paypal();
            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();
            response_paypal = await oPaypal.AprobarPago(token);

         
            ViewData["Status"] = response_paypal.Status;
            if(response_paypal.Status)
            {
              
                Venta oVenta = (Venta) TempData["Venta"] ;
                DataTable detalle_venta = (DataTable) TempData["DetalleVenta"];
                oVenta.IdTransaccion = response_paypal.Response.purchase_units[0].payments.captures[0].id;
                string mensaje= string.Empty;
                bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta,out mensaje);

                ViewData["IdTransaccion"] = oVenta.IdTransaccion;
                
            }
            return View();
        }


        [ValidarSession]
        [Authorize]
        public ActionResult MisCompras()
        {
            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            List<DetalleVenta> olista = new List<DetalleVenta>();
            bool conversion;
            olista = new CN_Venta().ListarCompras(idcliente).Select(p => new DetalleVenta()
            {
                oProducto = new Producto()
                {
                    
                    Nombre = p.oProducto.Nombre,                    
                    Precio = p.oProducto.Precio,                    
                    base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.oProducto.RutaImagen, p.oProducto.NombreImagen), out conversion),
                    extension = Path.GetExtension(p.oProducto.NombreImagen),
                   

                },
                Cantidad = p.Cantidad,
                Total = p.Total,
                IdTransaccion = p.IdTransaccion
            }).ToList();


            return View(olista);
        }

    }
}