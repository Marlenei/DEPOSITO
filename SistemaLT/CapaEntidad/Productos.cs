using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
//    Productos
//IdProducto  int (Auto) Clave P
//IdRubro int Clave F
//IdTipo  int Clave F
//Detalle VarChar(50)
//StockMinimo int
//StockActual int
//FechaUltimaModificacoin smalldatetime
//Activo  bit
//IdUsuario   int

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

    }
}
