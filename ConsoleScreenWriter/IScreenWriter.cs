using System;

namespace ConsoleClient
{
    public interface IScreenWriter : IDisposable
    {
        void Init();
        void Start();
    }
}
