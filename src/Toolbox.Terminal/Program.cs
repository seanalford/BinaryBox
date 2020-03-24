using System.Threading.Tasks;

namespace BinaryBox.Terminal
{
    class Program
    {
        //static TcpConnection Connection;
        static string Host = "www.eagleresearchcorp.com"; //"192.168.1.125"; //"192.168.0.4";
        static int Port = 80; // 10001; //2001;

        static string Prompt = ">";

        static async Task Main(string[] args)
        {
            ////Connection = new TcpConnection();
            //Connection.ConnectTimeout = 5;
            //Console.Clear();
            //Console.WriteLine(FiggleFonts.Ogre.Render("Toolbox"));
            //Console.Write(Prompt);
            //string command = Console.ReadLine();

            //while (command.ToLower() != "exit")
            //{
            //    try
            //    {
            //        if (command.ToLower() == "cls" || command.ToLower() == "clear")
            //        {
            //            Console.Clear();
            //        }
            //        else if (command.ToLower() == "host")
            //        {
            //            Console.Write($"Host:{Host} (ENTER to use current host): ");
            //            string host = Console.ReadLine();
            //            if (host != "") { Host = host; }
            //        }
            //        else if (command.ToLower() == "port")
            //        {
            //            Console.Write($"Port:{Port} (ENTER to use current port): ");
            //            string port = Console.ReadLine();
            //            if (port != "") { Port = int.Parse(port); }
            //        }
            //        else if (command.ToLower() == "open")
            //        {
            //            Console.WriteLine($"Connecting {Host}:{Port}...");
            //            try
            //            {
            //                await Connection.ConnectAsync(Host, Port);
            //                Status();
            //            }
            //            catch (System.Exception ex)
            //            {
            //                Console.WriteLine($"Connection to {Host}:{Port} failed!\n{ex.Message}");
            //            }
            //        }
            //        else if (command.ToLower() == "close")
            //        {
            //            await Connection.DisconnectAsync();
            //            Status();
            //        }
            //        else if (command.ToLower().StartsWith("send"))
            //        {

            //        }
            //        else if (command.ToLower() == "status")
            //        {
            //            Status();
            //        }
            //        else if (command.ToLower() == "fw")
            //        {

            //        }
            //        else if (command.ToLower() == "-h")
            //        {
            //            Console.WriteLine(FiggleFonts.Ogre.Render("Help"));
            //            Console.WriteLine("cls | clear : Clear the terminal screen");
            //            Console.WriteLine("host : Set host.");
            //            Console.WriteLine("port : Set port");
            //            Console.WriteLine("open : Open connection to host.");
            //            Console.WriteLine("close : Close connection to host.");
            //            Console.WriteLine("status : Display host connection status");
            //            Console.WriteLine("");
            //        }
            //        else if (command != "")
            //        {
            //            Console.WriteLine("Unknonw command. -h for help");
            //        }
            //    }
            //    finally
            //    {
            //        command = "";
            //        Console.Write(Prompt);
            //        command = Console.ReadLine();
            //    }
            //}
        }

        static void Status()
        {
            //if (Connection != null)
            //{
            //    Console.WriteLine($"{Host}:{Port}: {Connection.State}");
            //}
        }
    }
}
