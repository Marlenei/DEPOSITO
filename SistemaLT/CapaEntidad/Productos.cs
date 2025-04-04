using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{

    public class Productos
    {
        public int IdProducto { get; set; }
        public Rubros oRubros { get; set; }
        public Tipos oTipos { get; set; }
        public string Detalle {  get; set; }
        public int StockMinimo { get; set; }
        public int StockActual { get; set; }
        public string CodigoId { get; set; }
        public bool Activo { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaAlta { get; set; }

    }
}
