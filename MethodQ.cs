using System;
namespace MethodQ
{
    public class MethodQ : IMethodQ
    {
        public ulong Schedule(Action action, DateTime executionUtc)
        {
            throw new NotImplementedException();
        }

        public ulong Schedule(Action action, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public bool Cancel(ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
