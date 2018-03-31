using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Producer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserInterface;

namespace Consumer
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Error");
                GetSmsFromRabbitMq a = new GetSmsFromRabbitMq();
             
                a.RunWorkerProcessForSmss("Error");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.ReadLine();
            }
        }

       

      
    }
}
