using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Seccion
    {
        public int IDSeccion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadLibros { get; set; }

        public Seccion(int id, string nomb, string desc, int cantLibros) 
        {
            IDSeccion = id;
            Nombre = nomb;
            Descripcion = desc;
            CantidadLibros = cantLibros;
        }
    }
}
