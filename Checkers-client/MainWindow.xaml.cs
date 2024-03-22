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

                // Odbierz początkową wiadomość (informację o rozpoczęciu gry) od serwera
                byte[] startMessageBuffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(startMessageBuffer, 0, startMessageBuffer.Length);
                string startMessage = Encoding.ASCII.GetString(startMessageBuffer, 0, bytesRead);
                GameBoardLabel.Content = startMessage;

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
                        // Update UI with received data (game board)
                        GameBoardLabel.Content = data;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
