using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ClientProcessors
{
    public class WebSenderProccessor : IProcessor
    {
        private static readonly object _lock = new object();

        public Queue<string> PathsQueue { get; set; }

        public WebSenderProccessor()
        {
            PathsQueue = new Queue<string>();
        }

        public void StartProcess()
        {
            Process();
        }

        private void Process()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (PathsQueue.Count != 0)
                    {
                        var path = PathsQueue.Dequeue();
                        if (File.Exists(path))
                        {
                            byte[] buffer = File.ReadAllBytes(path);
                            Send(buffer);
                        }
                    }
                }

                Thread.Sleep(2500);
            }
        }

        private void Send(byte[] buffer)
        {
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://url");
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "POST";

            ////using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            ////{
            //    string json = "{\"Date\":\"" + DateTime.Now.ToString() + "\"," +
            //                  "\"File\":\"" + buffer.ToString() + "\"}";

            //    //streamWriter.Write(json);
            //    //streamWriter.Flush();
            //    //streamWriter.Close();
            ////}

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //}
        }
    }
}
