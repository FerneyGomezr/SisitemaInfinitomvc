using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public Dashboard verDashBoard()
       {
            Dashboard oDashboard = new Dashboard();
            try
            {
                using (SqlConnection oconecion=new SqlConnection(Conexion.cn))
                {
                    oconecion.Open();
                    SqlCommand ocmd = new SqlCommand("sp_ReporteDashboard", oconecion);
                    ocmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader odr = ocmd.ExecuteReader();
                    while (odr.Read())
                    {                        
                        oDashboard.TotalCliente = odr.GetInt32(odr.GetOrdinal("TotalCliente"));
                        oDashboard.TotalVenta = odr.GetInt32(odr.GetOrdinal("TotalVenta"));
                        oDashboard.TotalProducto = odr.GetInt32(odr.GetOrdinal("TotalProducto"));
                        
                    }
                }

            }
            catch (Exception)
            {

               oDashboard = new Dashboard();
            }



            return oDashboard;
        }


        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> lista = new List<Reporte>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", oconexion);
                    DateTime parsedFechaInicio, parsedFechaFin;
                    if (DateTime.TryParseExact(fechainicio, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedFechaInicio) &&
                        DateTime.TryParseExact(fechafin, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedFechaFin))
                    {
                        cmd.Parameters.AddWithValue("fechainicio", parsedFechaInicio.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("fechafin", parsedFechaFin.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("idtransaccion", idtransaccion);
                        cmd.CommandType = CommandType.StoredProcedure;
                        oconexion.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lista.Add(new Reporte()
                                {
                                    FechaVenta = rdr["FechaVenta"].ToString(),
                                    Cliente = rdr["Cliente"].ToString(),
                                    Producto = rdr["Producto"].ToString(),
                                    Precio = Convert.ToDecimal(rdr["Precio"], new CultureInfo("es-CO")),
                                    Cantidad = Convert.ToInt32(rdr["Cantidad"]),
                                    Total = Convert.ToDecimal(rdr["Total"], new CultureInfo("es-CO")),
                                    IdTransaccion = rdr["IdTransaccion"].ToString(),
                                });
                            }
                        }
                    }
                    else
                    {
                        throw new FormatException("Invalid date format.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lista;
        }



    }
}
