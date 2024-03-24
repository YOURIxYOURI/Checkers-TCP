using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checkers_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 8888); // Adres IP i port serwera

                stream = client.GetStream();

                MessageBox.Show("Connected to the server.", "Success", MessageBoxButton.OK);
                ConnectButton.IsEnabled = false;
                
                await ReceiveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReceiveData()
        {

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() =>
                    {
                       GenerateGameBoard(data);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string moveData = $"{StartRowEntry.Text}{StartColEntry.Text}{EndRowEntry.Text}{EndColEntry.Text}";
                byte[] buffer = Encoding.ASCII.GetBytes(moveData);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await ReceiveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GenerateGameBoard(string boardString)
        {
            // Podziel ciąg znaków planszy na wiersze
            string[] rowsData = boardString.Split("\r\n");

            int cols = rowsData[0].Length;

            // Tworzymy siatkę (Grid) do umieszczenia pól planszy
            Grid grid = new Grid();
            grid.Margin = new Thickness(10);
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;

            // Dodajemy odpowiednią liczbę wierszy i kolumn do siatki
            for (int i = 0; i < 8; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int j = 0; j < cols; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Wypełniamy siatkę planszą na podstawie stringa
            for (int i = 0; i < 8; i++)
            {
                string rowData = rowsData[i];

                for (int j = 0; j < cols; j++)
                {
                    char fieldValue = j < rowData.Length ? rowData[j] : ' '; // Ustaw spację dla brakujących pól

                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);

                    Label cellLabel = new Label();
                    cellLabel.Content = fieldValue == ' ' ? " " : fieldValue.ToString();
                    cellLabel.FontSize = 16;
                    cellLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    cellLabel.VerticalAlignment = VerticalAlignment.Center;

                    // Ustawiamy tło dla pola w zależności od jego pozycji na planszy
                    if ((i + j) % 2 == 0)
                    {
                        border.Background = Brushes.LightGray;
                    }
                    else
                    {
                        border.Background = Brushes.White;
                    }

                    border.Child = cellLabel;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    grid.Children.Add(border);
                }
            }

            // Dodajemy siatkę do okna lub innego kontenera w interfejsie WPF
            // Zakładając, że istnieje StackPanel o nazwie gameBoardPanel
            gameBoardPanel.Children.Clear(); // Wyczyszczenie istniejących elementów
            gameBoardPanel.Children.Add(grid);
        }

    }
}
