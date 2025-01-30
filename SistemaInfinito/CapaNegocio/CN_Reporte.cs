using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte oCD_Reporte = new CD_Reporte();

        public Dashboard verDashBoard()
        {
            return oCD_Reporte.verDashBoard();
        }
        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            return oCD_Reporte.Ventas(fechainicio, fechafin, idtransaccion);
        }

    }
}
