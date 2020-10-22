using System;
using System.Collections.Generic;
using System.Text;

namespace WordCounter
{
    class Worker : BaseThread
    {
        public delegate void FoundWordEventHandler(object sender, string word);
        public event FoundWordEventHandler FoundWord;
        private List<WordClass> Queue { get; set; }

        public Worker()
        {
            Queue = new List<WordClass>();
        }

        private bool canStop { get; set; } = false;

        public void AddQueue(string item)
        {
            lock (this)
            {
                Queue.Add(new WordClass() { Text = item, IsMarked = false });
            }
        }

        public override void RunThread()
        {
            int i = 0;

            while (!GetStop())
            {
                if (i < GetQueueCount())
                {
                    WordClass item = null;
                    lock (this)
                    {
                        item = Queue[i];
                    }

                    if (!item.IsMarked)
                    {
                        var words = item.Text.Split(' ');
                        foreach (var word in words)
                        {
                            FoundWord(this, word);
                            item.IsMarked = true;
                        }
                    }
                    i++;
                }
            }
        }

        private int GetQueueCount()
        {
            lock (this)
            {
                return Queue.Count;
            }
        }

        private bool GetStop()
        {
            lock (this)
            {
                return canStop;
            }
        }

        internal void SetStop()
        {
            lock (this)
            {
                canStop = true;
            }
        }
    }
}
