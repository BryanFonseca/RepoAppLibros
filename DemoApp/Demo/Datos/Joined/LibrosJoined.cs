using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class LibrosJoined
    {
        public int IDLibro { get; set; }
        public string Nombre { get; set; }
        public string Seccion { get; set; }
        public string Editorial { get; set; }
        public string ISBN { get; set; }
        public bool Prestado { get; set; }
        public LibrosJoined(int id, string nom, string secc, string edit, string isbn, bool prest)
        {
            IDLibro = id;
            Nombre = nom;
            Seccion = secc;
            Editorial = edit;
            ISBN = isbn;
            Prestado = prest;
        }
    }
}
