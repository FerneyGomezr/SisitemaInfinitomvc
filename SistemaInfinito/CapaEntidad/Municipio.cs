using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Municipio
    {

        public String IdMunicipio { get; set; }
        public Departamento IdDepartamento { get; set; }
        public string Descripcion { get; set; }
    }
}
