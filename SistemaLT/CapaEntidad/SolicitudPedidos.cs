using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class SolicitudPedidos
    {
        
        public int IdSolicitud { get; set; }
        public Productos oProductos { get; set; }
        
        public int CantidadPedida { get; set; }
        public int CantidadEntregada { get; set; }
        public string FechaPedido { get; set; }
        public string FechaEntrega { get; set; }
        public int IdUsuarioPedido { get; set; }

        public string FechaHoraActualizacion { get; set; }

        public int CodigoArea { get; set; }
        public int CodigoSector { get; set; }
       


        public int IdUsuarioEntrega { get; set; }
        public string Observaciones { get; set; }
        public string NroPedido { get; set; }
        public bool Visado { get; set; }
        public int UsuarioVisado { get; set; }
    }
    //public class UsuarioDatos
    //{
    //    public int CodigoUnico { get; set; }
    //    //public string NombreYApellido { get; set; }
    //    //public string Usuario { get; set; }
    //    public int CodigoArea { get; set; } // Asegurar que coincida con la API
    //    public int CodigoSector { get; set; }
    //}


    //public class Area
    //{
    //    public int CodigoArea { get; set; }
    //    public string NombreArea { get; set; }
    //    public string CorreoElectronico { get; set; }
    //    public bool Activo { get; set; }
    //}

    //public class Sector
    //{
    //    public int CodigoSector { get; set; }
    //    public string Nombre { get; set; }
    //    public int CodigoArea { get; set; } // Relación con área
    //    public bool Activo { get; set; }
    //}

}




