using System;
using System.IO;
using System.Net.Sockets;



namespace Client
{
    class Client
    {
        private NetworkStream sockStream;
        private BinaryWriter write;
        private string message = "";
       
        
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
            TcpClient client;
            try
            {

                client = new TcpClient("localhost", 9050);
                
                

                sockStream = client.GetStream();
                write = new BinaryWriter(sockStream);
                
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
                client.Close();

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
