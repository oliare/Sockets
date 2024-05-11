using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Sockets
{
    public partial class MainWindow : Window
    {
        static string address = "127.0.0.1";
        static int port = 8080;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int code = int.Parse(postIndexTextBox.Text);
                string response = await SendMessageToServer(code.ToString());
                streetsListBox.Items.Add(response);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task<string> SendMessageToServer(string message)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    byte[] buffer = Encoding.Unicode.GetBytes(message);
                    await Task.Run(() => socket.SendTo(buffer, ipPoint));

                    buffer = new byte[1024];
                    int bytes = await Task.Run(() => socket.ReceiveFrom(buffer, ref endPoint));

                    string response = Encoding.Unicode.GetString(buffer, 0, bytes);

                    return response;
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}