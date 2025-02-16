using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using System.Data.SqlClient;    
using System.Data;


namespace CapaDatos
{
    public class CD_Ubicacion
    {

        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> lista = new List<Departamento>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT  * FROM Departamento";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Departamento()
                            {
                                IdDepartamento = rdr["IdDepartamento"].ToString(),
                                Descripcion = rdr["Descripcion"].ToString(),
                                
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                lista = new List<Departamento>();
            }
            return lista;
        }

        public List<Municipio> ObtenerMunicipio(String IdDepartamento)
        {
            List<Municipio> lista = new List<Municipio>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT  * FROM Municipio where IdDepartamento= @IdDepartamento";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Municipio()
                            {
                                IdMunicipio = rdr["IdMunicipio"].ToString(),
                                Descripcion = rdr["Descripcion"].ToString(),

                            });
                        }
                    }
                }
            }
            catch (Exception)
            {

                lista = new List<Municipio>();
            }
            return lista;
        }

    }
}
