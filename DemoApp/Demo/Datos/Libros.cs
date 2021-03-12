using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Libros
    {
        public int IDLibro { get; set; }
        public int IDEditorial { get; set; }
        public int IDSeccion { get; set; }
        public string Nombre { get; set; }
        public string ISBN { get; set; }
        public bool Prestado { get; set; }
        public Libros(int id, int idedit, int idsecc, string nomb, string isbn, bool prest )
        {
            IDLibro = id;
            IDEditorial = idedit;
            IDSeccion = idsecc;
            Nombre = nomb;
            ISBN = isbn;
            Prestado = prest;
        }
    }
}
