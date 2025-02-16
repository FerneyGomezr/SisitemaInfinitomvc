using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using CapaDatos;
using System.Data;
using System.Data.SqlClient;

namespace CapaNegocio
{
    public class CN_Ubicacion
    {
        private CD_Ubicacion objCapaDato = new CD_Ubicacion();

        public List<Departamento> ObtenerDepartamento()
        {

           return objCapaDato.ObtenerDepartamento();
        }



        public List<Municipio> ObtenerMunicipio(String IdDepartamento)
        {
            return objCapaDato.ObtenerMunicipio(IdDepartamento);
        }   


    }
}
