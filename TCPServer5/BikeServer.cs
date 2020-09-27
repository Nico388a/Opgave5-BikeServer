using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BikeUnitTest1;
using Newtonsoft.Json;

namespace TCPServer5
{
    class BikeServer
    {
        private static List<Bike> bikes = new List<Bike>()
        {
            new Bike(1, "Green", 6, 250),
            new Bike(2, "Red", 3, 200),
            new Bike(3, "Yellow", 5, 150)
        };
        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 4646);

            server.Start();

            while (true)
            {
                TcpClient socket = server.AcceptTcpClient();

                Task.Run(() => DoClient(socket));
            }
            
        }

        private void DoClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);
            
            string str1 = sr.ReadLine();
            string str2 = sr.ReadLine();

            if (str1 == "HentAlle")
            {
              GetAll(sw,str2);  
            }
            else if (str1 == "Hent")
            {
                GetOne(sw, str2);

            }
            else if (str1 == "Gem")
            {
                Save(sw, str2);
            }
            else
            {
                sw.WriteLine("Protocol blev ikke accepteret");
            }

            sw.Flush();
        }

        private void GetAll(StreamWriter writer, string str)
        {
            if (str.Length == 0)
            {
                string json = JsonConvert.SerializeObject(bikes);
                writer.WriteLine(json);
                writer.Flush();
            }
            
        }

        private void GetOne(StreamWriter writer, string str)
        {
            if (str.Length != 0)
            {
                int tal = Convert.ToInt32(str);

                Bike b = bikes.Find(c=>c.Id==tal);

                string json = JsonConvert.SerializeObject(b);
                writer.WriteLine(json);

                writer.Flush();
            }
        }

        private void Save(StreamWriter writer, string str)
        {
            if (str.Length != 0)
            {
                Bike bj = JsonConvert.DeserializeObject<Bike>(str);
                
                bikes.Add(bj);

                writer.Flush();
            }
        }
    }
}
