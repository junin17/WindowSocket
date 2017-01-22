using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace Server
{
    public partial class Window : Form
    {






        private Server server;



        public Window()
        {

            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Hide();

        }

        /// <summary>
        /// Main
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Get the name of process
                string processName = Process.GetCurrentProcess().ProcessName;

                //Array of all process with same name
                Process[] process = Process.GetProcessesByName(processName);





                if (process.Length > 1)
                {
                    MessageBox.Show("You don't open more than one process of the same application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                    System.Environment.Exit(1);
                }

                Application.Run(new Window());
            }
            catch (Exception ex)
            {
                Util.WriteOnLog("LOG_ERROR", ex.ToString());
            }


        }


        /// <summary>
        /// Write received messages on Screen.
        /// </summary>
        public void WriteOnScreen(string texto, string data)
        {
            try
            {
                this.CheckLines();
                StringBuilder exit = new StringBuilder();
                exit.Capacity = Properties.Settings.Default.CharLimit + 50;


                exit.Append(data);
                exit.Append(": ");
                exit.Append(texto ?? "");

                

                Util.WriteOnLog("LOG_SCREEN", "Nova Mensagem Tela - " + exit.ToString());
                
                this.AppendText(exit.ToString());


                exit.Clear();
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }



        /// <summary>
        /// Checking how many lines having on text box
        /// </summary>
        private void CheckLines()
        {
            try
            {

                if (Properties.Settings.Default.LastMesssages + 1 <= TextBox.Lines.Count())
                {

                    List<string> lines = TextBox.Lines.ToList();
                    string line = lines[0];
                    lines.RemoveAt(0);

                    line = null;

                    TextBox.Lines = lines.ToArray();
                }


            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }

        /// <summary>
        /// Método que adiciona texto na tela.
        /// </summary>
        private void AppendText(string text)
        {
            try
            {



                this.TextBox.Text += text;
                this.TextBox.Text += new string(' ', 400);

                this.TextBox.Text += Environment.NewLine;

                this.TextBox.ScrollToCaret();


            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void CloseWindow()
        {
            try
            {
                IconServer.Visible = false;

                Application.Exit();
                System.Environment.Exit(1);
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }
        }


        /// <summary>
        /// Maximize Screen and flush buffer.
        /// </summary>
        private void Max()
        {
            try
            {


                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.TextBox.ScrollToCaret();
                this.server.maximizedWindow = true;

                lock ("buffer")
                {
                    foreach (var mensagem in this.server.buffer)
                    {
                        this.WriteOnScreen(mensagem.MessageReceived, mensagem.Date.ToString("dd/MM/yyyy HH:mm:ss"));
                    }
                    this.server.buffer.Clear();
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }


        }

        public void LogException(string excecao)
        {

            Util.WriteOnLog("LOG_ERROR", excecao);

            this.CloseWindow();


        }

        public void ChangeIcon(Message message)
        {
            try
            {
                //log Icone
                Util.WriteOnLog("LOG_ICON", "New signal - " + message.Signal);
                Util.WriteOnLog("LOG_ICON", "New Message - " + message.MessageReceived);


                IconServer.Visible = true;
                StringBuilder text = new StringBuilder();
                text.Capacity = 150;
                text.Append(message.Date.ToString("dd/MM/yyyy HH:mm:ss"));
                text.Append(": ");

                if (message.MessageReceived != null && message.MessageReceived.Length > 7)
                {
                    text.Append(message.MessageReceived.Substring(0, 7));
                    text.Append("...");
                }
                else
                {
                    text.Append(message.MessageReceived ?? "");
                }


                if (text.Length > 63)
                {
                    string substring = text.ToString().Substring(0, 60);
                    text.Clear();
                    text.Append(substring);
                    text.Append("...");
                }

                switch (message.Signal)
                {
                    // Programação Reprovada
                    case 1:
                        IconServer.Icon = Properties.Resources.icon1;
                        IconServer.Text = text.ToString();
                        break;

                    // Recebendo Mensagem
                    case 0:
                        IconServer.Icon = Properties.Resources.icon2;
                        IconServer.Text = text.ToString();
                        WarningComunictation();
                        break;


                }

                text.Clear();
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }

        /// <summary>
        /// Event who maximize application.
        /// </summary>
        private void Maximize(object sender, EventArgs e)
        {
            try
            {
                this.Max();
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }



        /// <summary>
        /// Event who minimze the application.
        /// </summary>
        private void Minimize(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button)
                {
                    this.server.maximizedWindow = false;
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;

                }
            }
            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }


        /// <summary>
        /// Shows a Message Box to inform failure on comunication.
        /// </summary>
        public void WarningComunictation()
        {
            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.Max();

                StringBuilder text = new StringBuilder();

                text.Append("Failure on Comunication");



                MessageBox.Show(new Form() { WindowState = FormWindowState.Maximized, TopMost = true }, text.ToString(), "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                text.Clear();
            }

            catch (Exception ex)
            {
                this.LogException(ex.ToString());
            }

        }


        private void AppServer_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(DoWork);
        }

        private void DoWork(object state)
        {
            server = new Server(this);
        }



    }
}
