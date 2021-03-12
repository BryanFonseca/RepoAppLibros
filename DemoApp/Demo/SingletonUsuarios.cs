using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace Demo
{
    public enum Usuarios { Administrador, Trabajador}
    class SingletonUsuarios
    {
        private readonly static SingletonUsuarios _instance = new SingletonUsuarios();

        private string connString;
        List<(string Usuario, string Contraseña)> listaUsuarios = new List<(string Usuario, string Contraseña)>();
        public bool conectado = false;
        public string ConnectionString
        {
            get { return connString; }
        }
        public Usuarios usuario;
        private SingletonUsuarios()
        {
            //los usuarios de la base de datos, esto simula una base de datos de usuario
            listaUsuarios.Add(("Trabajador", "12345")); //usuario normal
            listaUsuarios.Add(("Admin", "12345")); //usuario administrador
        }

        public static SingletonUsuarios Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AsignarTipoUsuario(string userName, string pass)
        {
            foreach (var item in listaUsuarios)
            {
                if (userName == item.Usuario && pass == item.Contraseña)
                {
                    if(item.Usuario == "Trabajador")
                    {
                        usuario = Usuarios.Trabajador;
                        connString = @"Server = DESKTOP-8FL4BMQ\SQLEXPRESS; Database = Biblioteca; User Id = Trabajador; Password = 12345;";
                    }
                    else
                    {
                        usuario = Usuarios.Administrador;
                        connString = @"Server = DESKTOP-8FL4BMQ\SQLEXPRESS; Database = Biblioteca; User Id = Admin; Password = 12345;";
                    }
                    break;
                }
            }
        }
    }
}
