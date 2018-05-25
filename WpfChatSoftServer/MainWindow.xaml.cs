using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfChatSoftClient;

namespace WpfChatSoftServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket serverSocket = null;
        private string serverIP = "127.0.0.1";
        private string serverPort = "8000";
        private IPAddress serverAddress = null;
        private IPEndPoint serverEndPoint = null;
        private int backlog = 1;

        bool isServerStarted = false;
        bool isClientConnected = false;

        private Socket connSocket = null;
        private Thread connThread = null;

        private IPAddress clientAddress = null;
        private string clientPort = null;
        private IPEndPoint clientEndPoint = null;

        public MainWindow()
        {
            InitializeComponent();
            MainWindow_Load();
        }

        private void MainWindow_Load()
        {
            StartClient();
        }

        private void StartClient()
        {
            WpfChatSoftClient.MainWindow clientWindow = new WpfChatSoftClient.MainWindow();
            clientWindow.Show();
        }

        private void ShowMsg(string msg)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                txtShow.AppendText(msg + "\n");
            });
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if(serverSocket == null)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverAddress = IPAddress.Parse(serverIP);
                serverEndPoint = new IPEndPoint(serverAddress, int.Parse(serverPort));
                serverSocket.Bind(serverEndPoint);
            }
            serverSocket.Listen(backlog);
            isServerStarted = true;
            ShowMsg("服务器启动");
            ShowMsg("Server Addr: " + serverIP + ":" + serverPort);


            connThread = new Thread(connect);
            connThread.IsBackground = true;
            connThread.Start();
        }

        private void connect()
        {
            while(isServerStarted == true)
            {
                connSocket = serverSocket.Accept();
                clientEndPoint = connSocket.RemoteEndPoint as IPEndPoint;
                clientAddress = clientEndPoint.Address;
                clientPort = clientEndPoint.Port.ToString();
                ShowMsg("Client Connected...");
                ShowMsg("Client Addr: " + clientAddress.ToString() + ":" + clientPort);
            }
        }
    }
}
