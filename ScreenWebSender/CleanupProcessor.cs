using System;
using System.Collections.Generic;

namespace ClientProcessors
{
    class CleanupProcessor : IProcessor
    {
        public Queue<string> PathsQueue { get; set; }
        public void StartProcess()
        {
            throw new NotImplementedException();
        }
    }
}
