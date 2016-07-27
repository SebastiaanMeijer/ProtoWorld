/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace SUMO_Packet_Listener_TEST
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Client or Server? (Type C or S)");
            Boolean S = Console.ReadLine().ToLower() == "s" ? true : false;
            String ip = "";
            if (!S)
            {
                Console.WriteLine("Enter IP:");
                ip = Console.ReadLine();
            }
            int Port;
            Console.WriteLine("Enter port:");
            Port = int.Parse(Console.ReadLine());

            if (S)
            {

                //UdpClient Server = new UdpClient(Port);
                TcpListener Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                Console.WriteLine("Listening...");

                Socket Server = Listener.AcceptSocket();
                //Socket Server = Listener.AcceptSocket();

                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int count = 0;
                byte[] result = new byte[90000];

                var applicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.SetBufferSize(128, 1024);
                using (BinaryWriter writer = new BinaryWriter(new FileStream(applicationPath + "\\" + "bufferoutput.xml", FileMode.Create)))
                {
                    try
                    {
                        while (true)
                        {
                            // byte[] result = Server.Receive(ref RemoteIpEndPoint); // For UDP

                            int BytesReceived = Server.Receive(result);
                            if (BytesReceived == 0)
                                break;
                            var bytes = result.Take(BytesReceived).ToArray();
                            //writer.Write("Packet " + count + "\t" + "Total bytes: " + BytesReceived + Environment.NewLine);
                            writer.Write(bytes);
                            //writer.Write(" ----------------------------------" + Environment.NewLine);
                            Console.WriteLine("Bytes received:" + BytesReceived);
                            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
                            Console.WriteLine("Packet " + count++);
                            Console.WriteLine("-----------------------------");
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

            }
            else
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                TcpClient client = new TcpClient();
                Console.WriteLine("Press any key to connect.");
                Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine("Connecting to " + ip + ":" + Port + "...");
                client.Connect(ip, Port);
                Console.WriteLine("Connected.");

                while (true)
                {
                    Console.WriteLine("Type something to send:");
                    String message = Console.ReadLine();
                    if (message == "quit")
                    {
                        client.Close();
                        return;
                    }
                    NetworkStream stream = client.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] buff = asen.GetBytes(message);
                    stream.Write(buff, 0, buff.Length);
                }

            }
        }
    }
}
