using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Visitantes
    {
        public int IDVisitante { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }

        public Visitantes(int idvis, string nom, string ci, string cel, string dir)
        {
            IDVisitante = idvis;
            Nombre = nom;
            Cedula = ci;
            Celular = cel;
            Direccion = dir;
        }
    }
}
