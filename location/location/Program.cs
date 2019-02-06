using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class location
{
    static void Main(string[] args)
    {
        try
        {
            int c, i;
            int hLocation = 0;
            int pLocation = 0;
            int timeout = 1000;
            string address = "localhost";
            string http = "";
            bool hUsed = false;
            bool pUsed = false;
            List<string> temp = new List<string>();
            int port = 43;
            for (i = 0; i < args.Length; i++)
            {
                temp.Add(args[i]);
            }
            for (i = 0; i < args.Length; i++)
            {
                if (args[i] == "/h")
                {
                    address = args[i + 1];
                    hLocation = i;
                    hUsed = true;
                }
                if (args[i] == "/p")
                {
                    port = Convert.ToInt32(args[i + 1]);
                    pLocation = i;
                    pUsed = true;
                }
                if (args[i] == "/h9")
                {
                    http = "/h9";
                    temp.Remove("/h9");
                }
                if (args[i] == "/h0")
                {
                    http = "/h0";
                    temp.Remove("/h0");
                }
                if (args[i] == "/h1")
                {
                    http = "/h1";
                    temp.Remove("/h1");
                }
            }
            if (pUsed == true && hUsed == true)
            {
                temp.RemoveRange(hLocation, 2);
                temp.RemoveRange(pLocation, 2);
            }
            if (pUsed == true && hUsed == false)
            {
                temp.RemoveRange(pLocation, 2);
            }
            if (hUsed == true && pUsed == false)
            {
                temp.RemoveRange(hLocation, 2);
            }
            args = temp.ToArray();
            TcpClient client = new TcpClient();
            client.Connect(address, port);
            client.ReceiveTimeout = timeout;
            client.SendTimeout = timeout;
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());
            switch (http)
            {
                default :
                    if (args.Length == 2)
                    {
                        sw.WriteLine(args[0] + " " + args[1]);
                        sw.Flush();
                        string line = sr.ReadLine().Trim();
                        Console.WriteLine(args[0] + " location changed to be in " + args[1]);
                        break;
                    }
                    else
                    {
                        sw.WriteLine(args[0]);
                        sw.Flush();
                        string line = sr.ReadLine().Trim();
                        if (line == "ERROR: no entries found")
                        {
                            Console.WriteLine("ERROR: no entries found");
                        }
                        else
                        {
                            Console.WriteLine(args[0] + " is in " + line);
                        }
                        break;
                    }

                case "/h9":
                    if (args.Length == 2)
                    {
                        sw.WriteLine("PUT /" + args[0]);
                        sw.WriteLine("");
                        sw.WriteLine(args[1]);
                        sw.Flush();
                        Console.WriteLine(args[0] + " is now in " + args[1]);
                        break;
                    }
                    else
                    {
                        sw.WriteLine("GET /" + args[0]);
                        sw.Flush();
                        string line = sr.ReadLine().Trim();
                        String[] segments = line.Split(new char[] { ' ' }, 4);
                        if (segments[1] == "404")
                        {
                            Console.WriteLine("ERROR: no entries found");
                        }
                        else
                        {
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            Console.WriteLine(args[0] + " is in " + line);
                        }
                        break;
                    }

                case "/h0":
                    if (args.Length == 2)
                    {
                        sw.WriteLine("POST /" + args[0] + " HTTP/1.0");
                        sw.WriteLine("Content-Length: " + args[1].Length);
                        sw.WriteLine("");
                        sw.WriteLine(args[1]);
                        sw.Flush();
                        Console.WriteLine(args[0] + " is now in " + args[1]);
                        break;
                    }
                    else
                    {
                        sw.WriteLine("GET /?" + args[0] + " HTTP/1.0");
                        sw.WriteLine("");
                        sw.Flush();
                        string line = sr.ReadLine().Trim();
                        String[] segments = line.Split(new char[] { ' ' }, 4);
                        if (segments[1] == "404")
                        {
                            Console.WriteLine("ERROR: no entries found");
                        }
                        else
                        {
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            Console.WriteLine(args[0] + " is in " + line);
                        }
                        break;
                    }

                case "/h1":
                    if (args.Length == 2)
                    {
                        string sentName = "name=";
                        string sentLocation = "&location=";
                        int contentLength = 
                            sentName.Length + sentLocation.Length + args[0].Length + args[1].Length;
                        sw.WriteLine("POST / HTTP/1.1");
                        sw.WriteLine("Host: " + address);
                        sw.WriteLine("Content-Length: " + contentLength);
                        sw.WriteLine();
                        sw.WriteLine(sentName + args[0] + sentLocation + args[1]);
                        sw.Flush();
                        Console.WriteLine(args[0] + " is now in " + args[1]);
                        break;
                    }
                    else
                    {
                        sw.WriteLine("GET /?name=" + args[0] + " HTTP/1.1");
                        sw.WriteLine("Host: " + address);
                        sw.WriteLine("");
                        sw.Flush();
                        string line = sr.ReadLine().Trim();
                        String[] segments = line.Split(new char[] { ' ' }, 4);
                        if (segments[1] == "404")
                        {
                            Console.WriteLine("ERROR: no entries found");
                        }
                        else
                        {
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            Console.WriteLine(args[0] + " is in " + line);
                        }
                        break;
                    }
            } 
        }
        catch
        {
            Console.WriteLine("Exception thrown");
        }
    }
}
