using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using System.Threading;

namespace Demo.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static StackPanel PanLogin;
        public static Button CerrarBoton;
        public static Image imageAdmin;
        public static TextBlock textoUser;
        public MainPage()
        {
            InitializeComponent();
            PanLogin = PanelLogin;
            CerrarBoton = CerrarSesionButton;
            imageAdmin = ImagenAdmin;
            textoUser = TextoUsuario;
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

        private void EnterIniciarSesion(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SetUsuarios();
                IniciarSesion();
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SetUsuarios();
            IniciarSesion();                       
        }
        private void IniciarSesion()
        {
            Thread.Sleep(200);
            if (SingletonUsuarios.Instance.conectado == true)
            {
                CerrarSesionButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ImagenAdmin.Visibility = Windows.UI.Xaml.Visibility.Visible;
                TextoUsuario.Visibility = Windows.UI.Xaml.Visibility.Visible;

                PanelLogin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ConAcceso();
            }
            else
            {
                SinAcceso();
            }
        }

        private void CerrarSesionButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Thread.Sleep(200);
            SinAcceso();
        }
        private void SinAcceso()
        {
            ImagenAdmin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            CerrarSesionButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            TextoUsuario.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
            PanelLogin.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ShellPage.Current.PanelsAccess(false);
        }
        private void ConAcceso()
        {
            ShellPage.Current.PanelsAccess(true);
            if(SingletonUsuarios.Instance.usuario == Usuarios.Trabajador)
            {
                TextoUsuario.Text = "Trabajador";
            }
            else
            {
                TextoUsuario.Text = "Administrador";
            }
        }
        private void SetUsuarios()
        {
            var usuario = Usuario.Text;
            var pass = Password.Password;
            var connStringPrueba = $@"Server = DESKTOP-8FL4BMQ\SQLEXPRESS; Database = Biblioteca; User Id = {usuario}; Password = {pass};";
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connStringPrueba))
                {
                    sqlConn.Open();
                    SingletonUsuarios.Instance.conectado = true;
                }
            }
            catch (SqlException sqlex)
            {
                Debug.WriteLine(sqlex.Message);
                SingletonUsuarios.Instance.conectado = false;
            }
            finally
            {
                SingletonUsuarios.Instance.AsignarTipoUsuario(usuario, pass);                
            }            
        }
    }
}
