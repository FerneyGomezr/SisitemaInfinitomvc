using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto
    {

        private CD_Producto objCapaDatos = new CD_Producto();

        public List<Producto> Listar()
        {
            return objCapaDatos.Listar();
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El campo Nombre  es Obligatorio";

            }
            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "El campo Descripcion es Obligatoria";

            }

           else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "El campo Marca es Obligatorio";
            }

            else if (obj.oCategoria.IdCategoria == 0)
            {
                Mensaje = "El campo Categoria es Obligatorio";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "El campo Precio es Obligatorio";
            }
                  
            
                     
            
            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDatos.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }


        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El campo Nombre  es Obligatorio";

            }
            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "El campo Descripcion es Obligatoria";

            }

            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "El campo Marca es Obligatorio";
            }

            else if (obj.oCategoria.IdCategoria == 0)
            {
                Mensaje = "El campo Categoria es Obligatorio";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "El campo Precio es Obligatorio";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDatos.Editar(obj, out Mensaje);

            }
            else
            {
                return false;
            }

        }

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDatos.Eliminar(id, out Mensaje);
        }

        public bool GuardarDatosImagen(Producto obj, out string Mensaje)
        {
            return objCapaDatos.GuardarDatosImagen(obj, out Mensaje);
        }
    }
}
