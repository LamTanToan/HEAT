using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace HEAT
{
    public partial class frmHeat : Form
    {
        private Socket SendSock;
        private Socket RecvSock;
        private Thread thread;
        byte[] bytes_username = new byte[4096];
        byte[] bytes_content = new byte[4096];
        byte[] bytes_sendtouser = new byte[4096];
        string[] s = new string[3];
        string content;
        public frmHeat()
        {
            InitializeComponent();
        }

        private void frmHeat_Load(object sender, EventArgs e)
        {   
            txtSendToUser.Enabled = false;
            txtSend.Enabled = false;
            txtReceive.Enabled = false;
            RecvSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint IP = new IPEndPoint(IPAddress.Any,12345);
            RecvSock.Bind(IP);
            thread = new Thread(new ThreadStart(ThreadProcess));
            thread.Start();
        }
        private void ThreadProcess()
        {
            while (true)
            {
                while (RecvSock.Available > 0)
                {
                    int NumByte = RecvSock.Receive(bytes_content);
                    s[0] = Encoding.Unicode.GetString(bytes_content, 0, NumByte);

                    NumByte = RecvSock.Receive(bytes_username);
                    s[1] = Encoding.Unicode.GetString(bytes_username, 0, NumByte);

                    NumByte = RecvSock.Receive(bytes_sendtouser);
                    s[2] = Encoding.Unicode.GetString(bytes_sendtouser, 0, NumByte);

                    if (s[2].Equals(txtUsername.Text))
                    {
                        txtReceive.Text += s[1] + " : " + s[0] + Environment.NewLine;
                    }
                    else
                    {
                        MessageBox.Show("Username này chưa connect !","Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        txtReceive.Text = "";
                        return;
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint IP = new IPEndPoint(IPAddress.Broadcast,12345);
            SendSock.Connect(IP);

            content = txtSend.Text;
            byte[] bytes_content = Encoding.Unicode.GetBytes(content);
            SendSock.Send(bytes_content);

            string username = txtUsername.Text;
            byte[] bytes_username = Encoding.Unicode.GetBytes(username);
            SendSock.Send(bytes_username);

            string sendtouser = txtSendToUser.Text;
            byte[] bytes_sendtouser = Encoding.Unicode.GetBytes(sendtouser);
            SendSock.Send(bytes_sendtouser);

            txtReceive.Text += txtUsername.Text + " : " + content + Environment.NewLine;
            txtSend.Text = "";

            SendSock.Close();
        }

        private void frmHeat_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Abort();
            RecvSock.Close();
        }

        private void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                SendSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint IP = new IPEndPoint(IPAddress.Broadcast, 12345);
                SendSock.Connect(IP);

                content = txtSend.Text;
                byte[] bytes_content = Encoding.Unicode.GetBytes(content);
                SendSock.Send(bytes_content);

                string username = txtUsername.Text;
                byte[] bytes_username = Encoding.Unicode.GetBytes(username);
                SendSock.Send(bytes_username);

                string sendtouser = txtSendToUser.Text;
                byte[] bytes_sendtouser = Encoding.Unicode.GetBytes(sendtouser);
                SendSock.Send(bytes_sendtouser);

                txtReceive.Text += txtUsername.Text + " : " + content + Environment.NewLine;
                txtSend.Text = "";

                SendSock.Close();
            }
        }

        private void txtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            content = txtUsername.Text;
            txtSendToUser.Enabled = true;
            txtSend.Enabled = true;
            txtReceive.Enabled = true;
            txtReceive.BackColor = Color.White;
            if (content == "")
            {
                txtSendToUser.Enabled = false;
                txtSend.Enabled = false;
                txtReceive.Enabled = false;
                txtReceive.BackColor = SystemColors.Control;

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            thread.Abort();
            RecvSock.Close();
            Close();
        }



    }
}
