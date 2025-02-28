using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CapaEntidad
{
    public class Acceso
    {
        public string usuario { get; set; }
        public string clave { get; set; }

       

    }

    public class UsuarioDatos
    {
        [JsonProperty("CodigoUnico")]
        public int CodigoUnico { get; set; }

        // Campos desde la API (strings)
        [JsonProperty("CodArea")]
        public string CodArea { get; set; }

        [JsonProperty("CodSector")]
        public string CodSector { get; set; }

        // Campos convertidos a enteros
        public int CodigoArea { get; set; }
        public int CodigoSector { get; set; }

        [JsonProperty("NombreArea")]
        public string NombreArea { get; set; }

        [JsonProperty("NombreSector")]
        public string NombreSector { get; set; }

        public void ConvertirCampos()
        {
            if (!int.TryParse(CodArea, out int codigoArea))
            {
                throw new InvalidOperationException("Formato inválido para CodArea");
            }
            CodigoArea = codigoArea;

            if (!int.TryParse(CodSector, out int codigoSector))
            {
                throw new InvalidOperationException("Formato inválido para CodSector");
            }
            CodigoSector = codigoSector;
        }
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
