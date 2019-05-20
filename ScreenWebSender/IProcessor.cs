using System.Collections.Generic;

namespace ClientProcessors
{
    public interface IProcessor
    {
        Queue<string> PathsQueue { get; set; }

        void StartProcess();
    }
}
