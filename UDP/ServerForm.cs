using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDP
{
    public partial class ServerForm : Form
    {
        UdpClient server;
        IPEndPoint endPoint;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            server = new UdpClient(int.Parse(txtServerPort.Text));
            endPoint = new IPEndPoint(IPAddress.Any, 0);

            // Start Server in Seprated Thread to Avoid User Inteface Blocking
            Thread thr = new Thread(new ThreadStart(ServerStart));
            thr.Start();
            btnStartServer.Enabled = false;
            btnNewClient.Enabled = true;

            // Write Server Started
            WriteLog("Durum : Server Başarıyla Başaltıldı...");
            WriteLog(string.Format("---------------------------------------------------------------------------------"));

        }

        private void ServerStart()
        {
            // Keep Server Listening

            while (true)
            {
                byte[] buffer = server.Receive(ref endPoint);
                //[] buffer2 = server.Receive(ref endPoint);


                // Message Formate => ClientPortNumber.hostName.Msg
                // Now Split Msg to Array Of Strings
                string[] msg = Encoding.Unicode.GetString(buffer).Split('.');
                //string[] msg2 = Encoding.Unicode.GetString(buffer2).Split('.');

                int clientPort = int.Parse(msg[0]);
                string clientHostName = msg[1];
                string request = msg[2];

                // Write Log to Notify Msg Received 
                WriteLog(string.Format("{0} numaralı istemci Portu, {1} Server'a dedi ki:\n {2}.", clientPort, clientHostName, request));

                // Respond to Client (for e.g send it the Current DateTime in this Server)
                //string response = string.Format("Date time at Server : {0}.", DateTime.Now.ToLocalTime());
                string response = string.Format("Sayın {0} Numaralı Port, sorunuzu aldım size döneceğim. {1}", clientPort, DateTime.Now.ToLocalTime());

                // Respond to Client 
                buffer = Encoding.Unicode.GetBytes(response);
                //buffer2 = Encoding.Unicode.GetBytes(response2);

                server.Send(buffer, buffer.Length, clientHostName, clientPort);
                //server.Send(buffer2, buffer2.Length, clientHostName, clientPort);


                // Write Log to Notify Response Sent
                WriteLog(string.Format("Bilgi: {1} Serverdan {0} numaralı istemciye cevap gönderildi.", clientPort, clientHostName));
                WriteLog(string.Format("---------------------------------------------------------------------------------"));

            }
        }

        private void WriteLog(string msg)
        {
            MethodInvoker invoker = new MethodInvoker(delegate
            {
                txtLog.Text += string.Format("{0}.{1}", msg, Environment.NewLine);
            });

            this.BeginInvoke(invoker);
        }

        private void btnNewClient_Click(object sender, EventArgs e)
        {
            ClientForm client = new ClientForm();
            client.Show();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Close();
            Application.Exit();
        }
    }
}
