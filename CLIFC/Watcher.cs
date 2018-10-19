using System;
using System.IO;
using System.Collections.Generic;
using System.Timers;

namespace CLIFC
{
    class Watcher
    {
        private Timer timer;
        private readonly string[] files;
        private IDictionary<string, DateTime> dates;
        private readonly Action action;

        private void Update (Object source, ElapsedEventArgs e)
        {
            bool executed = false;
            foreach (string file in files)
            {
                DateTime time = File.GetLastWriteTime(file);
                if (dates[file] < time)
                {
                    dates[file] = time;
                    if (!executed)
                    {
                        action();
                        executed = true;
                    }
                }
            }
        }

        public Watcher (string[] files, Action action)
        {
            this.files = files;
            this.action = action;
            dates = new Dictionary<string, DateTime>();
            //Init
            foreach (string file in files)
                dates.Add(file, File.GetLastWriteTime(file));
        }

        public void Run ()
        {
            timer = new Timer()
            {
                Interval = 2000,
                Enabled = true
            };
            timer.Elapsed += Update;
            Console.WriteLine("Press \'enter\' to quit.");
            Console.Read();
            timer.Stop();
            timer.Dispose();
        }

    }

}

