// Marco Giugliani, 4L, Gioco dei dadi in WPF
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
//aggiunte
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ComunicazioneSocket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NomeUtente nu = new NomeUtente();

        public MainWindow()
        {
            InitializeComponent();

            lblRegolamento.Content = "REGOLAMENTO: Si sfidano 2 giocatori. Inserisci l'ip e il numero di porta" +
                " dell'avversario e premi \n'Lancia dado' per lanciare il dado. NON TIRARE SE NON VEDI IL PUNTEGGIO" +
                " DELL'AVVERSARIO\nAGGIORNATO. Il primo dei due che arriva a 20 punti vince. \tBuona partita!";
            //come ip address posso mettere anche 127.0.0.1     porta inventata tra quelle libere
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 60000);

            Thread t1 = new Thread(new ParameterizedThreadStart(SocketReceive));
            t1.Start(localEndPoint);

            nu.ShowDialog();
            Title = nu.Nome;

            txtSourceIP.Text = localEndPoint.Address.ToString() + " : " + localEndPoint.Port;
        }

        
        int _somma = 0, _sommaAvversario = 0;

        //asincrona, mentre ricevo posso fare altre cose
        public async void SocketReceive(object sourceEndPoint)
        {
            IPEndPoint sourceEP = (IPEndPoint)sourceEndPoint;

            Socket t = new Socket(sourceEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(sourceEP);

            byte[] byteRicevuti = new byte[256];
            string message = "";

            int bytes = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if (t.Available > 0)
                    {
                        message = "";
                        bytes = t.Receive(byteRicevuti, byteRicevuti.Length, 0);
                        message += Encoding.ASCII.GetString(byteRicevuti, 0, bytes);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicezione.Content = "Ultimo punteggio avversario: ";
                            lblRicezione.Content += message;
                            _sommaAvversario += int.Parse(message);
                            lblParzialeAvversario.Content = "Punteggio avversario: ";
                            lblParzialeAvversario.Content += _sommaAvversario.ToString();

                            if (_sommaAvversario >= 20)
                            {
                                MessageBox.Show("HAI PERSO!", nu.Nome, MessageBoxButton.OK, MessageBoxImage.Information);
                                btnInvia.IsEnabled = false;
                            }
                        }));
                    }
                }
            });
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            imgDado1.Visibility = Visibility.Hidden;
            imgDado2.Visibility = Visibility.Hidden;
            imgDado3.Visibility = Visibility.Hidden;
            imgDado4.Visibility = Visibility.Hidden;
            imgDado5.Visibility = Visibility.Hidden;
            imgDado6.Visibility = Visibility.Hidden;
            _somma = 0;
            _sommaAvversario = 0;
            lblParziale.Content = "Punteggio: ";
            lblParzialeAvversario.Content = "Punteggio avversario: ";
            lblRicezione.Content = "Ultimo punteggio avversario: ";
            txtIpAdd.Focus();
            txtIpAdd.SelectAll();
            btnInvia.IsEnabled = true;
        }

        private void btnChiudi_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            imgDado1.Visibility = Visibility.Hidden;
            imgDado2.Visibility = Visibility.Hidden;
            imgDado3.Visibility = Visibility.Hidden;
            imgDado4.Visibility = Visibility.Hidden;
            imgDado5.Visibility = Visibility.Hidden;
            imgDado6.Visibility = Visibility.Hidden;
            try
            {
                if (txtIpAdd.Text == "" && txtDestPort.Text == "")
                {
                    txtIpAdd.Focus();
                    throw new ArgumentException("Devi inserire le informazioni sul destinatario!");
                }
                if (txtIpAdd.Text == "")
                {
                    txtIpAdd.Focus();
                    throw new ArgumentException("Devi inserire l'ip del destinatario!");
                }
                if (txtDestPort.Text == "")
                {
                    txtDestPort.Focus();
                    throw new ArgumentException("Devi inserire il numero di porta del destinatario!");
                }

                IPAddress ipDest = IPAddress.Parse(txtIpAdd.Text);
                int portDest = int.Parse(txtDestPort.Text);

                IPEndPoint remoteEndPoint = new IPEndPoint(ipDest, portDest);

                Socket s = new Socket(ipDest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                Random rnd = new Random();
                int risultato = rnd.Next(1, 7);
                switch (risultato)
                {
                    case 1:
                        imgDado1.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                    case 2:
                        imgDado2.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                    case 3:
                        imgDado3.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                    case 4:
                        imgDado4.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                    case 5:
                        imgDado5.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                    case 6:
                        imgDado6.Visibility = Visibility.Visible;
                        _somma += risultato;
                        break;
                }

                byte[] byteInviati = Encoding.ASCII.GetBytes(risultato.ToString());

                s.SendTo(byteInviati, remoteEndPoint);

                lblParziale.Content = "Punteggio: ";
                lblParziale.Content += _somma.ToString();


                if (_somma >= 20)
                {
                    MessageBox.Show("HAI VINTO!", nu.Nome, MessageBoxButton.OK, MessageBoxImage.Information);
                    btnInvia.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problema: " + ex.Message, "HOST1 : ERRORE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
