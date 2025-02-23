using System;//para manejar objetos
using System.Collections.Generic;//para manejar listas
using System.Linq;//para hacer consultas linq
using System.Text;//para manejar cadenas de texto
using System.Threading.Tasks;//para manejar tareas asincronas
using System.Configuration;//obtener y dar lectura del archivo de webconfig
using System.Net.Http;//para hacer peticiones a la api
using System.Net.Http.Headers;//para hacer peticiones a la api
using Newtonsoft.Json;//para convertir el json en un objeto
using CapaEntidad.Paypal;



namespace CapaNegocio
{
    public class CN_Paypal
    {
        //para obtener los valores del archivo de webconfig
        private static string urlpaypal= ConfigurationManager.AppSettings["urlpaypal"];
        private static string clientId = ConfigurationManager.AppSettings["ClientId"];
        private static string secret = ConfigurationManager.AppSettings["Secret"];


        public async Task<Response_Paypal<Response_Checkout>> CrearSolicitud(Checkout_Order orden)
        {
            //para hacer la peticion a la api de paypal

            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal);
                var authToken=Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var json =JsonConvert.SerializeObject(orden);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response= await client.PostAsync("v2/checkout/orders", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Checkout checkout= JsonConvert.DeserializeObject<Response_Checkout>(jsonRespuesta);    
                    response_paypal.Response = checkout;
                }
                return response_paypal;
            }
        }

        public async Task<Response_Paypal<Response_Capture>> AprobarPago(string token)
        {
            //para hacer la peticion a la api de paypal

            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal);
                var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

               
                var data = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"v2/checkout/orders/{token}/capture", data);
                response_paypal.Status = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    Response_Capture capture = JsonConvert.DeserializeObject<Response_Capture>(jsonRespuesta);
                    response_paypal.Response = capture;
                }
                return response_paypal;
            }
        }
    }
}
