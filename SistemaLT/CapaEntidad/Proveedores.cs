using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
//    IdProveedor Int(Auto)  Clave P
//RazonSocial VarChar(20)
//FechaAlta Smalldatetime
//IdUsuario int
//Activo  bit

    public class Proveedores
    {
        public int IdProveedor {  get; set; }
        public string RazonSocial { get; set; }
        public bool Activo { get; set; }
        public int IdUsuario {  get; set; }

    }
}
