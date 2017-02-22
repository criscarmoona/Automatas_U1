using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace AutomatasU1
{
    public sealed partial class MainPage : Page
    {

        #region Variables
        ObservableCollection<string> estados = new ObservableCollection<string>();
        ObservableCollection<string> transiciones = new ObservableCollection<string>();
        ObservableCollection<string> estadosAceptacion = new ObservableCollection<string>();
        #endregion

        #region Constructor
        public MainPage()
        {
            this.InitializeComponent();
        }
        #endregion

        private string[,] GenerarMatriz()
        {
            string[,] matrizEstadoActual = new string[transiciones.Count, estados.Count];
            for (int i = 0; i < estados.Count; i++)
            {
                for (int j = 0; j < transiciones.Count; j++)
                {
                    try
                    {
                        string seleccionCombo = ((ComboBox)((StackPanel)(Tabla.Children[i + 1])).Children[j + 1]).SelectedItem.ToString();
                        matrizEstadoActual[j, i] = seleccionCombo;
                    }
                    catch { }
                }
            }
            return matrizEstadoActual;
        }
        private void RellenarTabla(string[,] tablaAnterior)
        {
            for (int i = 0; i < estados.Count; i++)
            {
                for (int j = 0; j < transiciones.Count; j++)
                {
                    try
                    {
                        ((ComboBox)((StackPanel)(Tabla.Children[i + 1])).Children[j + 1]).SelectedItem = tablaAnterior[j, i];
                    }
                    catch { }
                }
            }
        }
        private async void BotonAgregarEstado_Click(object sender, RoutedEventArgs e)
        {

            var contentDialogAgregarEstado = new ContentDialog
            {
                Title = "Agregar Estado",
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancelar",
                Content = new TextBox
                {
                    Width = 100,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    MaxLength = 1
                }
            };

            if (await contentDialogAgregarEstado.ShowAsync() == ContentDialogResult.Primary)
            {
                if (!estados.Contains(((TextBox)contentDialogAgregarEstado.Content).Text))
                {
                    estados.Add(((TextBox)contentDialogAgregarEstado.Content).Text);
                    GenerarTabla();

                }
                else
                {
                    var error = new ContentDialog
                    {
                        Title = "Error",
                        PrimaryButtonText = "Ok",
                        Content = "Ya existe este estado"
                    };
                    await error.ShowAsync();
                }
            }
        }

        private void GenerarTabla()
        {

            var matrizEstadoActual = GenerarMatriz();

            Tabla.Children.Clear(); //Limpiar tabla

            //lista de transiciones columna 1
            StackPanel transicionesLista = new StackPanel();
            transicionesLista.Children.Add(new TextBlock() { Height = 40 }); //Para que empiecen las transiciones en el segundo, porque la primera es el nombre del estado
            for (int i = 0; i < transiciones.Count; i++)
            {
                ToggleButton esEstadoAceptacion = new ToggleButton() { Name = transiciones[i], Content = "F" };
                if (estadosAceptacion.Contains(transiciones[i])) { esEstadoAceptacion.IsChecked = true; }
                esEstadoAceptacion.Checked += EsEstadoAceptacion_Checked;
                esEstadoAceptacion.Unchecked += EsEstadoAceptacion_Unchecked;
                TextBlock nombreTransicion = new TextBlock() { Text = transiciones[i], Height = 40 };
                Button botonBorrarTransicion = new Button() { Content = "x", Name = transiciones[i] };
                botonBorrarTransicion.Click += Borrartransicion;
                StackPanel NombreBotonJuntos = new StackPanel() { Orientation = Orientation.Horizontal };
                NombreBotonJuntos.Children.Add(esEstadoAceptacion);
                NombreBotonJuntos.Children.Add(nombreTransicion);
                NombreBotonJuntos.Children.Add(botonBorrarTransicion);
                transicionesLista.Children.Add(NombreBotonJuntos);
            }
            Tabla.Children.Add(transicionesLista);

            //Columnas por cada estado
            for (int i = 0; i < estados.Count; i++)
            {
                TextBlock nombreEstado = new TextBlock() { Text = estados[i], Height = 40 };
                Button botonBorrarEstado = new Button() { Content = "x", Name = estados[i] };
                botonBorrarEstado.Click += BorrarEstado;
                StackPanel NombreBotonJuntos = new StackPanel() { Orientation = Orientation.Horizontal };
                NombreBotonJuntos.Children.Add(nombreEstado);
                NombreBotonJuntos.Children.Add(botonBorrarEstado);
                StackPanel nuevoEstado = new StackPanel() { Name = estados[i] };
                nuevoEstado.Children.Add(NombreBotonJuntos);
                for (int j = 0; j < transiciones.Count; j++)
                {
                    nuevoEstado.Children.Add(new ComboBox() { Height = 40, ItemsSource = transiciones }); //Los espacios para llenar las transiciones
                }
                Tabla.Children.Add(nuevoEstado);
            }
            RellenarTabla(matrizEstadoActual);
        }

        private void EsEstadoAceptacion_Unchecked(object sender, RoutedEventArgs e)
        {
            estadosAceptacion.Remove(((ToggleButton)sender).Name);
        }

        private void EsEstadoAceptacion_Checked(object sender, RoutedEventArgs e)
        {
            estadosAceptacion.Add(((ToggleButton)sender).Name);
        }

        private void Borrartransicion(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(((Button)sender).Name);
            var matrizEstadoActual = GenerarMatriz();
            int x = transiciones.IndexOf(((Button)sender).Name);
            transiciones.Remove(((Button)sender).Name);
            estadosAceptacion.Remove(((Button)sender).Name);
            ObservableCollection<string> nuevasTransiciones = new ObservableCollection<string>();
            for (int i = 0; i < transiciones.Count; i++)
            {
                if (i >= x)
                {
                    int numeroTransicion = Int32.Parse(transiciones[i].Replace("s", ""));
                    nuevasTransiciones.Add("s" + (numeroTransicion - 1));
                }
                else nuevasTransiciones.Add(transiciones[i]);
            }
            string[,] w = new string[transiciones.Count, estados.Count];
            int p = 0;
            for (int i = 0; i <= transiciones.Count; i++)
            {
                if (i != x)
                {
                    for (int j = 0; j < estados.Count; j++)
                    {
                        w[p, j] = matrizEstadoActual[i, j] != ((Button)sender).Name ? matrizEstadoActual[i, j] : null;

                    }
                    p++;
                }
            }
            for (int i = 0; i < transiciones.Count; i++)
            {
                for (int j = 0; j < estados.Count; j++)
                {
                    var indice = transiciones.IndexOf(w[i, j]);
                    w[i, j] = indice >= 0 ? nuevasTransiciones[indice] : null;
                }
            }
            transiciones = nuevasTransiciones;
            for (int i = 0; i < estadosAceptacion.Count; i++)
            {
                if (!transiciones.Contains(estadosAceptacion[i]))
                {
                    estadosAceptacion.Remove(estadosAceptacion[i]);
                }
            }
            var q = w;

            GenerarTabla();
            RellenarTabla(q);
        }

        private void BorrarEstado(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(((Button)sender).Name);
            var matrizEstadoActual = GenerarMatriz();
            int x = estados.IndexOf(((Button)sender).Name);
            estados.Remove(((Button)sender).Name);
            string[,] w = new string[transiciones.Count, estados.Count];
            int p = 0;
            for (int i = 0; i <= estados.Count; i++)
            {
                if (i != x)
                {
                    for (int j = 0; j < transiciones.Count; j++)
                    {
                        w[j, p] = matrizEstadoActual[j, i] != ((Button)sender).Name ? matrizEstadoActual[j, i] : null;

                    }
                    p++;
                }
            }

            GenerarTabla();
            RellenarTabla(w);
        }

        private void BotonAgregarTransicion_Click(object sender, RoutedEventArgs e)
        {
            transiciones.Add("s" + transiciones.Count);
            GenerarTabla();
        }

        private void ChecarCadenaClic(object sender, RoutedEventArgs e)
        {
            var matriz = GenerarMatriz();
            string cadena = Cadena.Text;
            string estadoActual = "s0";
            for (int i = 0; i < cadena.Length; i++)
            {
                string nuevoEstado = matriz[(transiciones.IndexOf(estadoActual)), (estados.IndexOf(cadena[i].ToString()))];
                if (nuevoEstado != null)
                {
                    estadoActual = nuevoEstado;
                }
                else
                {
                    estadoActual = "ERROR";
                break;
                }
            }
            if (estadosAceptacion.Contains(estadoActual))
            {
                BorderResultado.Background = new SolidColorBrush(Colors.Green);
                Resultado.Text = "Correcto";
            }
            else
            {
                BorderResultado.Background = new SolidColorBrush(Colors.Red);
                Resultado.Text = "Incorrecto";
            }
        }
    }
}
