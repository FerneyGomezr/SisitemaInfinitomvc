using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Usuarios
    {

        public List<Usuario> Listar()
        {
           List<Usuario> lista = new List<Usuario>();

       
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT  IdUsuario,Nombres,Apellidos,Correo,Clave,Reestablecer,Activo FROM usuario";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(rdr["IdUsuario"]),
                                Nombres = rdr["Nombres"].ToString(),
                                Apellidos = rdr["Apellidos"].ToString(),
                                Correo = rdr["Correo"].ToString(),
                                Clave = rdr["Clave"].ToString(),
                                Reestablecer = Convert.ToBoolean(rdr["Reestablecer"]),
                                Activo = Convert.ToBoolean(rdr["Activo"])
                            });
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
                //lista = new List<Usuario>();
            }
            return lista;
        }
    }
}
