using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Client
{
    class Client
    {
        private NetworkStream sockStream;
        private BinaryWriter write;
        private string message = "";
       
        private Thread tipoThread;
        
        static void Main(string[] args)
        {
            Client cliente = new Client();
            Console.ReadKey();
        }

        public Client()
        {
            RunClient();

        }

		

        public void RunClient()
        {
            TcpClient cliente;
            try
            {

                cliente = new TcpClient();

                cliente.Connect("localhost", 9050);
                

                sockStream = cliente.GetStream();
                write = new BinaryWriter(sockStream);
				DateTime dataI = DateTime.Now;
				
				int num = 1;
				write.Write(num.ToString());
                do
                {
                    try
                    {
						
						message = Console.ReadLine();
						write.Write(message);

						
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.ReadKey();
                        System.Environment.Exit(System.Environment.ExitCode);
                    }

                } while (message != "END");
                write.Close();
                sockStream.Close();
                cliente.Close();

            }
			catch (SocketException ex)
			{
				Console.WriteLine("Without Connection...");
			}

            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }

        }
    }
}
