using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Acceso
    {
        public string usuario { get; set; }
        public string clave { get; set; }


    }
    public class Permiso
    {
        public int IdUnico { get; set; }
        public int CodigoUsuario { get; set; }
        public int Accesos { get; set; }
    }
}
