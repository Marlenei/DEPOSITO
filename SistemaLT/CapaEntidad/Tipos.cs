using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
//    IdTipo int (Auto) Clave P
//Tipo    varchar(20)
//IdRubro int Clave F
//activo  bit
//IdUsuario   int

    public class Tipos
    {
        public int IdTipo {  get; set; }
        public string Tipo { get; set; }
        public Rubros oRubros { get; set; }
        public bool Activo { get; set; }
        public int IdUsuario { get; set; }
    }
}
