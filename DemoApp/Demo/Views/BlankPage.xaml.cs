using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Demo.Core.Models;
using Demo.Core.Services;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Demo.Views
{
    public sealed partial class BlankPage : Page, INotifyPropertyChanged
    {
        public BlankPage()
        {
            InitializeComponent();
            Libro.ItemsSource = GetLibros();
            CargarElementosSensibles(SingletonUsuarios.Instance.usuario);
        }
        private void CargarElementosSensibles(Usuarios privilegios)
        {
            if(privilegios == Usuarios.Trabajador)
            {
                BotonAgregarLibro.Visibility = Visibility.Collapsed;
                TrashButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                BotonAgregarLibro.Visibility = Visibility.Visible;
                TrashButton.Visibility = Visibility.Visible;
            }
        }
        public ObservableCollection<string> GetCBEditorial()
        {
            string GetEditorialQuery = "select * from editorial";
            var editoriales = new ObservableCollection<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetEditorialQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int ID = reader.GetInt32(0);
                                    string Nombre = reader.GetString(1);
                                    string Ubic = reader.GetString(2);
                                    string Corre = reader.GetString(3);
                                    var editorial = new Editorial(ID, Nombre, Ubic, Corre);
                                    editoriales.Add(editorial.Nombre);
                                }
                            }
                        }
                    }
                }
                return editoriales;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
                return null;
            }
        }

        public ObservableCollection<string> GetCBSeccion()
        {
            string GetEditorialQuery = "select * from seccion";
            var secciones = new ObservableCollection<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetEditorialQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int ID = reader.GetInt32(0);
                                    string Nombre = reader.GetString(1);
                                    string Desc = reader.GetString(2);
                                    int Cant = reader.GetInt32(3);
                                    var seccion = new Seccion(ID, Nombre, Desc, Cant);
                                    secciones.Add(seccion.Nombre);
                                }
                            }
                        }
                    }
                }
                return secciones;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        public enum VerLibros {Todos, Prestados, Retrasados}
        public ObservableCollection<LibrosJoined> GetLibros(VerLibros opcion = VerLibros.Todos)
        {
            string GetLibrosPrestamoQuery = "SELECT * FROM vista_libros";
            if(opcion == VerLibros.Todos)
            {
                GetLibrosPrestamoQuery = "SELECT * FROM vista_libros";
            }else if(opcion == VerLibros.Prestados)
            {
                GetLibrosPrestamoQuery = "SELECT * FROM vista_libros where prestado = 1";
            }else if(opcion == VerLibros.Retrasados)
            {
                GetLibrosPrestamoQuery = "select l.IDLibro, l.Nombre, l.Seccion, l.Editorial, l.isbn, l.prestado from vista_libros l inner join Prestamo p on p.IDLibro = l.IDLibro where getdate() >= p.FechaDevolucion";
            }
            var libros = new ObservableCollection<LibrosJoined>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetLibrosPrestamoQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int IDLibro = reader.GetInt32(0);
                                    string Nombre = reader.GetString(1);
                                    string Seccion = reader.GetString(2);
                                    string Editorial = reader.GetString(3);
                                    string ISBN = reader.GetString(4);
                                    bool Prestado = reader.GetBoolean(5);
                                    var libro = new LibrosJoined(IDLibro, Nombre, Seccion, Editorial, ISBN, Prestado);
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

        public ObservableCollection<LibrosJoined> GetLibros(string busqueda, VerLibros opcion = VerLibros.Todos)
        {
            string GetLibrosPrestamoQuery = "SELECT * FROM vista_libros where upper(Nombre) like upper('%" + busqueda + "%');";
            if (opcion == VerLibros.Todos)
            {
                GetLibrosPrestamoQuery = "SELECT * FROM vista_libros where upper(Nombre) like upper('%" + busqueda + "%');";
            }
            else if (opcion == VerLibros.Prestados)
            {
                GetLibrosPrestamoQuery = "SELECT * FROM vista_libros where prestado = 1 and where upper(Nombre) like upper('%" + busqueda + "%');";
            }
            else if (opcion == VerLibros.Retrasados)
            {
                GetLibrosPrestamoQuery = "select l.IDLibro, l.Nombre, l.Seccion, l.Editorial, l.isbn, l.prestado from vista_libros l inner join Prestamo p on p.IDLibro = l.IDLibro where getdate() >= p.FechaDevolucion and where upper(Nombre) like upper('%" + busqueda + "%');";
            }
            var libros = new ObservableCollection<LibrosJoined>();

            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        using (SqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = GetLibrosPrestamoQuery;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int IDLibro = reader.GetInt32(0);
                                    string Nombre = reader.GetString(1);
                                    string Seccion = reader.GetString(2);
                                    string Editorial = reader.GetString(3);
                                    string ISBN = reader.GetString(4);
                                    bool Prestado = reader.GetBoolean(5);
                                    var libro = new LibrosJoined(IDLibro, Nombre, Seccion, Editorial, ISBN, Prestado);
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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ComboboxMostrar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboboxMostrar.SelectedIndex == 0)
            {
                Libro.ItemsSource = GetLibros( VerLibros.Todos);
            }
            if (ComboboxMostrar.SelectedIndex == 1)
            {
                Libro.ItemsSource = GetLibros( VerLibros.Prestados);
            }
            if (ComboboxMostrar.SelectedIndex == 2)
            {
                Libro.ItemsSource = GetLibros( VerLibros.Retrasados);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemCollection editoriales = EditorialCB.Items;
            //Se ingresa un libro
            try
            {
                ObservableCollection<string> editItems = GetCBEditorial();
                string nombreEdit = "";
                foreach (var item in editItems)
                {
                    if (EditorialCB.SelectedItem.ToString() == item)
                    {
                        nombreEdit = item;
                    }
                }
                ObservableCollection<string> seccItems = GetCBSeccion();
                string nombreSecc = "";
                foreach (var item in seccItems)
                {
                    if (SeccionCB.SelectedItem.ToString() == item)
                    {
                        nombreSecc = item;
                    }
                }
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("InsertarLibro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Bit);
                        command.Parameters.Add("@Nom", SqlDbType.VarChar).Value = Nombre.Text;
                        command.Parameters.Add("@ISBN", SqlDbType.VarChar).Value = ISBN.Text;
                        command.Parameters.Add("@NombEditorial", SqlDbType.VarChar).Value = nombreEdit;
                        command.Parameters.Add("@NombSeccion", SqlDbType.VarChar).Value = nombreSecc;
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();
                        var result = returnParameter.Value;
                        if(result.ToString() == "1")
                        {
                            //si se ha retornado un 0, se cierra la ventana
                            IngresarLibro.Visibility = Visibility.Collapsed;
                            Libro.ItemsSource = GetLibros();
                            Nombre.Text = string.Empty;
                            ISBN.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            
        }

        private void BotonAgregarLibro_Click(object sender, RoutedEventArgs e)
        {
            IngresarLibro.Visibility = Visibility.Visible;
            EditorialCB.ItemsSource = GetCBEditorial();
            SeccionCB.ItemsSource = GetCBSeccion();                
        }

        private void CerrarVentanaIngresarLibro_Click(object sender, RoutedEventArgs e)
        {
            IngresarLibro.Visibility = Visibility.Collapsed;
            Nombre.Text = string.Empty;
            ISBN.Text = string.Empty;
        }

        private void BotonEliminarLibro_Click(object sender, RoutedEventArgs e)
        {
            VentanaEliminarLibro.Visibility = Visibility.Visible;
        }

        private void EliminarLibro(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("EliminarLibro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (int.TryParse(id.Text, out int idLibro))
                        {
                            command.Parameters.Add("@id", SqlDbType.Int).Value = idLibro;
                        }
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
                Libro.ItemsSource = GetLibros();
                id.Text = string.Empty;
            }
        }

        private void EliminarLibro(int idLibro)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("EliminarLibro", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;                        
                        command.Parameters.Add("@id", SqlDbType.Int).Value = idLibro;                        
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
                Libro.ItemsSource = GetLibros();
                id.Text = string.Empty;
            }
        }

        private void EliminarLibros(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("EliminarLibrosRangos", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (int.TryParse(inicio.Text, out int idInicio))
                        {
                            command.Parameters.Add("@inicio", SqlDbType.Int).Value = idInicio;
                        }
                        if (int.TryParse(fin.Text, out int idFinal))
                        {
                            command.Parameters.Add("@final", SqlDbType.Int).Value = idFinal;
                        }
                        command.ExecuteNonQuery();
                        VentanaEliminarLibros.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (SqlException eSql)
            {
                Debug.WriteLine("The exception is: " + eSql.Message);
            }
            finally
            {
                Libro.ItemsSource = GetLibros();
                inicio.Text = string.Empty;
                fin.Text = string.Empty;                
            }
        }

        private void AbrirVentanaEliminarRango_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CerrarVentanaEliminarLibros_Click(object sender, RoutedEventArgs e)
        {
            VentanaEliminarLibros.Visibility = Visibility.Collapsed;
            id.Text = string.Empty;
        }

        private void CerrarVentanaEliminarLibro_Click(object sender, RoutedEventArgs e)
        {
            VentanaEliminarLibro.Visibility = Visibility.Collapsed;
            inicio.Text = string.Empty;
            fin.Text = string.Empty;
        }

        private void TrashButton_Click(object sender, RoutedEventArgs e)
        {
            //DeleteOptions.
        }

        private void DeleteOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idLibro = 0;
            if(DeleteOptions.SelectedIndex == 0)
            {
                idLibro = ((LibrosJoined)Libro.SelectedItem).IDLibro;
                EliminarLibro(idLibro);
            }
            else if (DeleteOptions.SelectedIndex == 1)
            {
                PresionarBotonEliminarRangos();
            }
        }
        private void PresionarBotonEliminarRangos()
        {
            VentanaEliminarLibros.Visibility = Visibility.Visible;
            VentanaEliminarLibro.Visibility = Visibility.Collapsed;
        }

        private void BuscarLibroButton_Click(object sender, RoutedEventArgs e)
        {
            Libro.ItemsSource = GetLibros(BusquedaTextBox.Text);
        }

        private void EnterBuscarLibros(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Libro.ItemsSource = GetLibros(BusquedaTextBox.Text);
            }
        }
    }
}
