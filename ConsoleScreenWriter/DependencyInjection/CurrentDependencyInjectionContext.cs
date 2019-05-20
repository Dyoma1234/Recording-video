using AForge.Vision.Motion;
using ClientProcessors;
using ConsoleClient.Logger;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace ConsoleClient.DependencyInjection
{
    public class CurrentDependencyInjectionContext
    {
        public static UnityContainer Container { set; get; }

        static CurrentDependencyInjectionContext()
        {
            Container = new UnityContainer();
            RegisterTypes(Container);
        }

        private static void RegisterTypes(UnityContainer container)
        {
            container.RegisterType<IProcessor, WebSenderProccessor>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILogger, ConsoleLogger>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMotionProcessing, BlobCountingObjectsProcessing>(new ContainerControlledLifetimeManager(), new InjectionConstructor(true));
            container.RegisterType<IMotionDetector, TwoFramesDifferenceDetector>(new InjectionConstructor(true));
            container.RegisterType<MotionDetector, MotionDetector>();
            container.RegisterType<IScreenWriter, ScreenWriter>(new ContainerControlledLifetimeManager());
        }
    }
}