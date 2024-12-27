﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Ingresos
    {
        public int IdIngreso { get; set; }
        public Proveedores oProveedores { get; set; }
        public Productos oProductos { get; set; }
        public string CodigoId { get; set; }
        public int Cantidad { get; set; }
        public string Observaciones { get; set; }
        public int IdUsuario { get; set; }
        //public Productos oStockActual { get; set; }
        public char TipoIngreso { get; set; }
        public string FechaIngreso { get; set; }
    }
}