using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Editorial
    {
        public int IDEditorial { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public string Correo { get; set; }

        public Editorial(int id, string nomb, string ubic, string correo)
        {
            IDEditorial = id;
            Nombre = nomb;
            Ubicacion = ubic;
            Correo = correo;
        }
    }
}
