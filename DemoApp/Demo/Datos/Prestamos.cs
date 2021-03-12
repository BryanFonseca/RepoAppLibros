using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Prestamos
    {
        public int ID { get; set; }
        public string Libro { get; set; }
        public string Visitante { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }

        public Prestamos(int id, string lib, string visi, DateTime fprest, DateTime fdevol)
        {
            ID = id;
            Libro = lib;
            Visitante = visi;
            FechaPrestamo = fprest;
            FechaDevolucion = fdevol;
        }
    }
}
