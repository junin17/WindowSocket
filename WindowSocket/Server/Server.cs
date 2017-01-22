using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{

    class Server
    {


        private int connections;

        public bool maximizedWindow { get; set; }
        private Thread thread;
        private Window window;
        public List<Message> buffer { get; set; }




        public Server(Window window)
        {
            maximizedWindow = false;
            buffer = new List<Message>();
            this.window = window;
            thread = new Thread(new ThreadStart(RunServer));
            thread.Start();
        }

        /// <summary>
        /// Starts Server
        /// </summary>
        public void RunServer()
        {
            TcpListener listerner;


            connections = 0;
            try
            {
                IPAddress ipAddress = Dns.GetHostEntry(Properties.Settings.Default.Adress).AddressList[0];
                listerner = new TcpListener(ipAddress, Properties.Settings.Default.Port);

                listerner.Start();
                while (true)
                {

                    if (connections < 1)
                    {

                        var message = new Message { MessageReceived = "Wait for connections", Date = DateTime.Now };
                        WriteOnBuffer(message);

                    }


                    connections++;

                    TcpClient client = listerner.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(ThreadProc, client);





                }
            }


            catch (Exception ex)
            {
                window.Invoke(new Action(() => { window.LogException(ex.ToString()); }));
            }
        }

        private void ThreadProc(object obj)
        {
            var connection = (TcpClient)obj;
            NetworkStream socketStream = connection.GetStream();
            BinaryReader reader = new BinaryReader(socketStream);

            //Timeout of Sockets in Minutes
            connection.Client.ReceiveTimeout = Properties.Settings.Default.Timeout * 60000;


            String textReceived = "";

            do
            {
                try
                {

                    textReceived = reader.ReadString();


                    //log Socket
                    Util.WriteOnLog("LOG_SOCKET", "Message Received - " + textReceived);




                    
                    if (textReceived.Length > Properties.Settings.Default.CharLimit)
                    {
                        textReceived = textReceived.Substring(0, Properties.Settings.Default.CharLimit);
                    }

                    var message = new Message { MessageReceived = textReceived, Date = DateTime.Now , Signal = 1};


                    Write(message);
                    window.Invoke(new Action(() => { window.ChangeIcon(message); }));



                }

                catch (Exception ex)
                {
                    try
                    {
                        if (ex is EndOfStreamException)
                        {
                            break;
                        }
                        if (ex is IOException || ex is SocketException)
                        {
                            var message = new Message { MessageReceived = "Failure on Comunication", Date = DateTime.Now, Signal = 0 };
                            var messageException = new Message { MessageReceived = ex.Message.Substring(0, Properties.Settings.Default.CharLimit) + "...", Date = DateTime.Now };
                            Write(message);
                            Util.WriteOnLog("LOG_ERROR", "Failure on Comunication");
                            Util.WriteOnLog("LOG_ERROR", ex.Message);
                            
                            break;
                        }
                        else
                        {

                            window.Invoke(new Action(() => { window.LogException(ex.Message); }));
                            break;
                        }
                    }
                    catch (Exception error)
                    {
                        Util.WriteOnLog("LOG_ERROR", error.ToString());
                    }


                }

            } while (connection.Connected);

            reader.Close();
            socketStream.Close();
            connection.Close();

        }

        private void Write(Message message)
        {
            if (!maximizedWindow)
            {
                WriteOnBuffer(message);
            }
            else
            {
                window.Invoke(new Action(() => { window.WriteOnScreen(message.MessageReceived, message.Date.ToString("dd/MM/yyyy HH:mm:ss")); }));

            }
        }

        private void WriteOnBuffer(Message mensagem)
        {
            lock ("buffer")
            {
                if (buffer.Count >= Properties.Settings.Default.LastMesssages)
                {
                    buffer.RemoveAt(0);
                }
                buffer.Add(mensagem);
            }
        }

        


    }



    public class Message
    {
        public String MessageReceived { get; set; }
        public DateTime Date { get; set; }
        public int Signal { get; set; }
    }
}
