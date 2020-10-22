using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordCounter
{
    class Program
    {
        static Dictionary<string, int> map = new Dictionary<string, int>();
        private readonly static object lockMapObject = new object();

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            string path = args[0];
            int maxThreadCount = 5;
            if (args.Length > 1)
            {
                int o = 0;
                bool result = int.TryParse(args[1], out o);
                if (result && o > 0)
                {
                    maxThreadCount = o;
                }
                else
                {
                    Console.WriteLine("Argument error for thread count: Expected positive integer!");
                }
            }

            Console.WriteLine("Start process with " + path);
            Console.WriteLine("Thread Pool Count:  " + maxThreadCount);

            string text = System.IO.File.ReadAllText(path);

            string[] sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");

            for (int i = 0; i < sentences.Length; i++)
            {
                sentences[i] = Regex.Replace(sentences[i], @"[\.!\?,;;]*", "").TrimStart();
            }

            int sentenceCount = sentences.Length;
            double avgWordCount = sentences.Average(r => r.Where(x => x == ' ').Count() + 1);


            List<Worker> workers = new List<Worker>();

            int thCnt = 0;
            foreach (var item in sentences)
            {
                if (workers.Count < maxThreadCount)
                {
                    Worker worker = new Worker();
                    worker.FoundWord += Th_FoundWord;
                    worker.AddQueue(item);
                    workers.Add(worker);
                    worker.Start();
                }
                else
                {
                    workers[thCnt % maxThreadCount].AddQueue(item);
                }
                thCnt++;
            }

            workers.ForEach(r => r.SetStop());

            while (workers.Any(r => r.IsAlive))
            {

            }

            WriteMap();

            Console.ReadKey();
        }

        private static void WriteMap()
        {
            foreach (var item in map)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }
        }

        private static void Th_FoundWord(object sender, string word)
        {
            lock (lockMapObject)
            {
                if (map.ContainsKey(word))
                {
                    map[word]++;
                }
                else
                {
                    map.Add(word, 1);
                }
            }
        }
    }
}
