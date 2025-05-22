using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Egresos
    {

        public Productos oProductos { get; set; }
        public int IdEgreso { get; set; }
        public string CodigoId { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaEgreso { get; set; }
        public int CodigoArea { get; set; }
        public string NombreArea { get; set; }
        public int CodigoSector { get; set; }
        public string NombreSector { get; set; }
        public char TipoSalida { get; set; }
        public string NombreyApellido { get; set; }
        public DateTime? FechaAct { get; set; }

    }
}
 