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
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int IdUsuarioPedido { get; set; }
        public int CodigoArea { get; set; }
        public int CodigoSector { get; set; }

        public DateTime FechaHoraActualizacion { get; set; }
        public int IdUsuarioEntrega { get; set; }
        public string Observaciones { get; set; }
        public string NroPedido { get; set; }
        public bool Visado { get; set; }
        public int UsuarioVisado { get; set; }
    }
 
}




