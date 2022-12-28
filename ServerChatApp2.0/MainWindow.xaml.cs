using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ServerChatApp2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket mainSock;
        Socket accSock;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000));
            mainSock.Listen(0);
            accSock = mainSock.Accept();
            mainSock.Close();

            new Thread(() => {
                while (true) {
                    byte[] sizeBuffer = new byte[4];
                    accSock.Receive(sizeBuffer, 0 , sizeBuffer.Length, SocketFlags.None);
                    int size = BitConverter.ToInt32(sizeBuffer, 0);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        txtResponse.Document.Blocks.Clear();
                    });


                    //this will hold the data for the buffers that we receive
                    MemoryStream ms = new MemoryStream();
;                   while (size > 0)
                    {
                        byte[] buffer;
                        //size 30
                        // buffersize 20

                        if (size < accSock.ReceiveBufferSize)
                        {
                            buffer = new byte[size];
                        }
                        else {
                            buffer = new byte[accSock.ReceiveBufferSize];
                        }

                        int receive = accSock.Receive(buffer);
                        size -= receive; 
                        ms.Write(buffer, 0, buffer.Length);
                    }
                    ms.Close();

                    byte[] data = ms.ToArray();
                    Application.Current.Dispatcher.Invoke(() => {
                        txtResponse.Document.Blocks.Add(new Paragraph(new Run(Encoding.ASCII.GetString(data))));
                    });

                }
            }).Start();
        }
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }
    }
}
