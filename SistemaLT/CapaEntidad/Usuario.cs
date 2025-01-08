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

    public class AccesoResultado
    {
        public int result { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }

    public class Permiso
    {
        //public int codigousuario { get; set; }
        public int IdUnico { get; set; }
        public int CodigoUsuario { get; set; }
        public int Accesos { get; set; }
    }

    public class PermisoResultado
    {
        public List<Permiso> Permisos { get; set; }
        //public int IdUnico { get; set; }
        //public int CodigoUsuario { get; set; }
        //public int Accesos { get; set; }
    }
}
