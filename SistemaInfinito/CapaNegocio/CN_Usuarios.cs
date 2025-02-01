using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;


namespace CapaNegocio
{
    public class CN_Usuarios
    {

        private CD_Usuarios objCapaDato = new CD_Usuarios();
        public List<Usuario> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje=string.Empty;

            if(string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "El campo Nombres es obligatorio";
                return 0;
            }

            if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El campo Apellidos es obligatorio";
                return 0;
            }
            if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El campo Correo es obligatorio";
                return 0;
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                string clave = CN_Recursos.GenerarClave();
                string asunto = "Registro de Usuario Sistema Infinito";
                string mensaje = "<h3> Su cuenta fue creada correctamente </h3><br><p>Su contraseña para acceder es:!clave!</p>";
                mensaje = mensaje.Replace("!clave!", clave);
                bool respuesta = CN_Recursos.EnviarCorreo(obj.Correo, asunto, mensaje);
                
                if (respuesta)
                {
                    obj.Clave = CN_Recursos.ConvertirSha256(clave);
                    return objCapaDato.Registrar(obj, out Mensaje);
                }
                else
                {
                    Mensaje = "Error al enviar el correo";
                    return 0;
                }
                

            }

            return objCapaDato.Registrar(obj, out Mensaje);
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "El campo Nombres es obligatorio";                
            }

            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El campo Apellidos es obligatorio";               
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El campo Correo es obligatorio";
                
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(obj, out Mensaje);

            }
            else
            {
                return false;
            }   
             
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }

        public bool CambiarClave(int idusuario, string nuevaclave, out string Mensaje)
        {
            return objCapaDato.CambiarClave(idusuario, nuevaclave, out Mensaje);
        }

        public bool ReestablecerClave(int idusuario, string correo, out string Mensaje)
        { 

            Mensaje = string.Empty;
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado= objCapaDato.ReestablecerClave(idusuario, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);
            if (resultado)
            {
                string asunto="Contraseña Reestablecida";
                string mensaje_correo = "<h3> Su contraseña fue reestablecida correctamente </h3><br><p>Su nueva contraseña para acceder es:!clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", nuevaclave);
                bool respuesta = CN_Recursos.EnviarCorreo(correo, asunto, mensaje_correo);


                if (respuesta)
                {
                    return true;
                }
                else
                {
                    Mensaje = "No se pudo enviar el coorreo con  la clave";
                    return false;
                }

            }
            else
            {
                Mensaje = "Error al reestablecer la clave";
                return false;
            }

        
        }
    }
}
