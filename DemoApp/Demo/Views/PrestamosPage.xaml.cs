using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace Demo.Views
{
    public sealed partial class PrestamosPage : Page, INotifyPropertyChanged
    {
        public PrestamosPage()
        {
            InitializeComponent();
            Prestamos.ItemsSource = GetPrestamos();
        }

        private void EnterVisitante(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                BuscarVisitantes();
            }
        }
        private void EnterLibros(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                BuscarLibros();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Libros> GetLibros(string busqueda)
        {
            string GetLibrosPrestamoQuery = "select * from libro where upper(Nombre) like upper('%" +  busqueda + "%') and prestado = 0";
            var libros = new ObservableCollection<Libros>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if(conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetLibrosPrestamoQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    int idedit = reader.GetInt32(1);
                                    int idsecc = reader.GetInt32(2);
                                    string nom = reader.GetString(3);
                                    string isb = reader.GetString(4);
                                    bool prest = reader.GetBoolean(5);
                                    var libro = new Libros(id, idedit, idsecc, nom, isb, prest);
                                    libros.Add(libro);
                                }
                            }
                        }    
                    }
                }
                return libros;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
                return null;
            }
        }
        public ObservableCollection<Visitantes> GetVisitantes(string busqueda)
        {
            //const string GetLibrosPrestamoQuery = "select IDLibro, Nombre from libro";
            string GetVisitantesPrestamoQuery = "select * from visitante where upper(Nombres) like upper('%" + busqueda + "%')";
            var visitantes = new ObservableCollection<Visitantes>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetVisitantesPrestamoQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int IDVisitante = reader.GetInt32(0);
                                    string Nombres = reader.GetString(1);
                                    string Cedula = reader.GetString(2);
                                    string Celular = reader.GetString(3);
                                    string Direccion = reader.GetString(4);
                                    var visitante = new Visitantes(IDVisitante, Nombres, Cedula, Celular, Direccion);
                                    visitantes.Add(visitante);
                                }
                            }
                        }
                    }
                }
                return visitantes;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
                return null;
            }
        }
        public ObservableCollection<Prestamos> GetPrestamos()
        {
            //const string GetLibrosPrestamoQuery = "select IDLibro, Nombre from libro";
            string GetPrestamosQuery = "select * from vista_prestamo";
            var prestamos = new ObservableCollection<Prestamos>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetPrestamosQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int id = reader.GetInt32(0);
                                    string libro = reader.GetString(1);
                                    string visitante = reader.GetString(2);
                                    DateTime fPrestamo = reader.GetDateTime(3);
                                    DateTime fDevolucion = reader.GetDateTime(4);
                                    var prestamo = new Prestamos(id, libro, visitante, fPrestamo, fDevolucion);
                                    prestamos.Add(prestamo);
                                }
                            }
                        }
                    }
                }
                return prestamos;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
                return null;
            }
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void BotonBuscarLibrosPrestamos_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BuscarLibros();
        }
        private void BotonBuscarVisitantesPrestamos_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BuscarVisitantes();
        }

        private void BuscarVisitantes()
        {
            Visitante_Prestamos.ItemsSource = GetVisitantes(TextBoxVisitante.Text);
        }
        private void BuscarLibros()
        {
            Libro_Prestamos.ItemsSource = GetLibros(TextBoxLibros.Text);        
        }

        private void BotonPrestar_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //uso de procedimiento almacenado para prestar libros

            int idLibro = 0;
            int idVisitante = 0;

            try
            {
                if (Libro_Prestamos.SelectedItem != null)
                {
                    idLibro = ((Libros)Libro_Prestamos.SelectedItem).IDLibro;
                }
                if (Visitante_Prestamos.SelectedItem != null)
                {
                    idVisitante = ((Visitantes)Visitante_Prestamos.SelectedItem).IDVisitante;
                }
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("InsertarPrestamo", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@IDLibro", SqlDbType.Int).Value = idLibro;
                        command.Parameters.Add("@IDVisitante", SqlDbType.Int).Value = idVisitante;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
            }
            finally
            {
                //se debe recargar la tabla
                ActualizarTablasPrestarDevolver();
            }            
        }

        private void BotonDevolver_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int idPrestamo = 0;
                        
            try
            {
                if (Prestamos.SelectedItem != null)
                {
                    idPrestamo = ((Prestamos)Prestamos.SelectedItem).ID;
                }
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("DevolverLibro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@IDPrestamo", SqlDbType.Int).Value = idPrestamo;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
            }
            finally
            {
                //se debe recargar la tabla
                ActualizarTablasPrestarDevolver();
            }
        }
        private void ActualizarTablasPrestarDevolver()
        {
            Prestamos.ItemsSource = GetPrestamos();
            Libro_Prestamos.ItemsSource = GetLibros("");
        }
    }
}
