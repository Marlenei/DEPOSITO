using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
//    IdRubro int (Auto)
//Rubro   varchar(20)
//Codigo varchar(5)
//Activo bit
//IdUsuario int

    public class Rubros
    {
        public int IdRubro {  get; set; }
        public string Rubro { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
        public int IdUsuario { get; set; }

    }
}
