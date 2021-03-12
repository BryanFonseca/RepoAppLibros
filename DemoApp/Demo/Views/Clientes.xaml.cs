using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Demo.Core.Models;
using Demo.Core.Services;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace Demo.Views
{
    public sealed partial class DataGridPage : Page, INotifyPropertyChanged
    {
        //string ConnectionString = @"server=DESKTOP-8FL4BMQ\SQLEXPRESS;database=Biblioteca;trusted_connection=true;";
        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        // TODO WTS: Change the grid as appropriate to your app, adjust the column definitions on DataGridPage.xaml.
        // For more details see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid
        public DataGridPage()
        {
            InitializeComponent();
            Visitante.ItemsSource = GetVisitantes();
            CargarElementosSensibles(SingletonUsuarios.Instance.usuario);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Source.Clear();

            // TODO WTS: Replace this with your actual data
            var data = await SampleDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
        private void CargarElementosSensibles(Usuarios privilegios)
        {
            if (privilegios == Usuarios.Trabajador)
            {
                AgregarVisitanteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AgregarVisitanteButton.Visibility = Visibility.Visible;
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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        public ObservableCollection<Visitantes> GetVisitantes()
        {
            //const string GetLibrosPrestamoQuery = "select IDLibro, Nombre from libro";
            string GetVisitantesPrestamoQuery = "select * from visitante";// where upper(Nombres) like upper('%" + busqueda + "%')";
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

        public ObservableCollection<Visitantes> GetVisitantes(string busqueda)
        {
            //const string GetLibrosPrestamoQuery = "select IDLibro, Nombre from libro";
            string GetVisitantesPrestamoQuery = "select * from visitante where upper(Nombres) like upper('%" + busqueda + "%')"; ;// 
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

        private void BuscarClienteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Visitante.ItemsSource = GetVisitantes(BusquedaClienteTextBox.Text);
        }
        private void EnterBuscarCliente(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Visitante.ItemsSource = GetVisitantes(BusquedaClienteTextBox.Text);
            }
        }

        private void AgregarVisitanteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IngresarVisitante.Visibility = Visibility.Visible;
        }

        private void CerrarVentanaIngresarVisitante_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IngresarVisitante.Visibility = Visibility.Collapsed;
        }

        private void BotonIngresarVisitante_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //se ingresa visitante
            try
            {                
                using (SqlConnection conn = new SqlConnection(SingletonUsuarios.Instance.ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("InsertarVisitante", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Nom", SqlDbType.VarChar).Value = Nombres.Text;
                        command.Parameters.Add("@Ced", SqlDbType.VarChar).Value = Cedula.Text;
                        command.Parameters.Add("@Celu", SqlDbType.VarChar).Value = Celular.Text;
                        command.Parameters.Add("@Dir", SqlDbType.VarChar).Value = Direccion.Text;
                        command.ExecuteNonQuery();
                        Nombres.Text = string.Empty;
                        Cedula.Text = string.Empty;
                        Celular.Text = string.Empty;
                        Direccion.Text = string.Empty;
                        IngresarVisitante.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            finally
            {
                Visitante.ItemsSource = GetVisitantes();
            }
        }
    }
}
