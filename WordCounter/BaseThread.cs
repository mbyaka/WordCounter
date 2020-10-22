using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WordCounter
{
    abstract class BaseThread
    {
        private Thread _thread;

        protected BaseThread()
        {
            _thread = new Thread(new ThreadStart(this.RunThread));
        }

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;

        public abstract void RunThread();
    }
}
