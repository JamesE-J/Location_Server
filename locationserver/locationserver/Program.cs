using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace locationserver
{
    class Program
    {
        /// <summary>
        /// This method is called from the main as soon at the start to 
        /// create a server and to start listening for connections from a client.
        /// </summary>
        static void runServer(Dictionary<string, string> dict)
        {
            TcpListener listener;
            Socket connection;
            Handler RequestHandler;
            try
            {
                //Create a TCP Socket
                listener = new TcpListener(IPAddress.Any, 43);
                listener.Start();
                Console.WriteLine("Server Is Listening For Connection");
                //listens for a connection and loops forever
                while (true)
                {
                    connection = listener.AcceptSocket();
                    RequestHandler = new Handler();
                    Thread t = new Thread(() => RequestHandler.doRequest(connection, dict));
                    t.Start();
                }
            } catch (Exception e)
            {
                //Catches exception
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
        /// <summary>
        /// This is the main, it is the main.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Dictionary<string, string> dict =
            new Dictionary<string, string>();
            dict.Add("505520", "is being tested");
            runServer(dict);
        }
    }
}
class Handler
{
    public void doRequest(Socket connection, Dictionary<string, string> dict)
    {
        NetworkStream socketStream;
        socketStream = new NetworkStream(connection);
        Console.WriteLine("---CONNECTION STARTED---");
        try
        {
            StreamWriter sw = new StreamWriter(socketStream);
            StreamReader sr = new StreamReader(socketStream);
            String line = sr.ReadLine().Trim();
            Console.WriteLine("INPUT:" + line);
            String[] segments = line.Split(new char[] { ' ' }, 4);
            switch (segments[0])
            {
                case "GET":
                    if (segments.Length == 2)
                    {
                        if (dict.ContainsKey(segments[1].TrimStart('/')))
                        {
                            sw.WriteLine("HTTP/0.9 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.WriteLine(dict[segments[1].TrimStart('/')]);
                            sw.Flush();
                        }
                        else
                        {
                            sw.WriteLine("HTTP/0.9 404 Not Found");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.Flush();
                        }
                        break;
                    }
                    switch (segments[2])
                    {
                        case "HTTP/1.0":
                            if (dict.ContainsKey(segments[1].TrimStart('/', '?')))
                            {
                                sw.WriteLine("HTTP/1.0 200 OK");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.WriteLine(dict[segments[1].TrimStart('/', '?')]);
                                sw.Flush();
                            }
                            else
                            {
                                sw.WriteLine("HTTP/1.0 404 Not Found");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.Flush();
                            }
                            break;
                        case "HTTP/1.1":
                            if (dict.ContainsKey(segments[1].Remove(0, 7)))
                            {
                                sw.WriteLine("HTTP/1.1 200 OK");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.WriteLine(dict[segments[1].Remove(0, 7)]);
                                sw.Flush();
                            }
                            else
                            {
                                sw.WriteLine("HTTP/1.1 404 Not Found");
                                sw.WriteLine("Content-Type: text/plain");
                                sw.WriteLine("");
                                sw.Flush();
                            }
                            break;
                    }
                    break;
                case "PUT":
                    string name = segments[1].TrimStart('/');
                    line = sr.ReadLine().Trim();
                    line = sr.ReadLine().Trim();
                    dict[name] = line;
                    sw.WriteLine("HTTP/0.9 200 OK");
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine("");
                    sw.Flush();
                    break;
                case "POST":
                    switch (segments[2])
                    {
                        case "HTTP/1.0":
                            name = segments[1].TrimStart('/');
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            dict[name] = line;
                            sw.WriteLine("HTTP/1.0 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.Flush();
                            break;
                        case "HTTP/1.1":
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            line = sr.ReadLine().Trim();
                            String[] post1Segments = line.Split(new char[] { '&' }, 4);
                            name = post1Segments[0].Remove(0, 5);
                            dict[name] = post1Segments[1].Remove(0, 9);
                            sw.WriteLine("HTTP/1.1 200 OK");
                            sw.WriteLine("Content-Type: text/plain");
                            sw.WriteLine("");
                            sw.Flush();
                            break;
                    }
                    break;
                default:
                    if (segments.Length > 1)
                    {
                        if (dict.ContainsKey(segments[0]))
                        {
                            string[] locationArray = segments.Skip(1).ToArray();
                            string location = String.Join(" ", locationArray);
                            dict[segments[0]] = location;
                            sw.WriteLine("OK");
                            sw.Flush();
                        }
                        else
                        {
                            string[] locationArray = segments.Skip(1).ToArray();
                            string location = String.Join(" ", locationArray);
                            dict.Add(segments[0], location);
                            sw.WriteLine("OK");
                            sw.Flush();
                        }
                    }
                    else if (dict.ContainsKey(segments[0]))
                    {
                        sw.WriteLine(dict[segments[0]]);
                        sw.Flush();
                    }
                    else
                    {
                        sw.WriteLine("ERROR: no entries found");
                        sw.Flush();
                    }
                    break;
            }
        }
        catch
        {
            Console.WriteLine("ERROR: Something went wrong");
        }
        finally
        {
            socketStream.Close();
            connection.Close();
            Console.WriteLine("---CONNECTION ENDED---");
        }
    }
}
