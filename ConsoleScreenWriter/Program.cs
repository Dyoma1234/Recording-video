using System.Threading;
using ClientProcessors;
using ConsoleClient.DependencyInjection;
using Unity;


namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = CurrentDependencyInjectionContext.Container;

            var webSender = container.Resolve<IProcessor>();
            Thread SenderThread = new Thread(webSender.StartProcess)
            {
                IsBackground = true
            };

            using (IScreenWriter writer = container.Resolve<IScreenWriter>())
            {
                SenderThread.Start();
                writer.Init();
                writer.Start();
            }
        }
    }
}
