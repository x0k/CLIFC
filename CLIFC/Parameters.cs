using System.IO;
using System.Collections.Generic;
using System;

namespace CLIFC
{

    class Options
    {
        private IDictionary<string, string[]> arguments;

        public static IDictionary<string, string[]> Parse(string[] args)
        {
            IDictionary<string, string[]> arguments = new Dictionary<string, string[]>();
            string currentName = "";
            List<string> values = new List<string>();
            foreach (string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if (currentName != "")
                        arguments[currentName] = values.ToArray();
                    values.Clear();
                    currentName = arg.Substring(1);
                }
                else if (currentName == "")
                    arguments[arg] = new string[0];
                else
                    values.Add(arg);
            }
            if (currentName != "")
                arguments[currentName] = values.ToArray();
            return arguments;
        }

        public Options(string[] args)
        {
            arguments = Parse(args);
            List<string> fs = new List<string>();
            if (args.Length > 0)
            {
                void add(string path)
                {
                    try
                    {
                        string fp = Path.GetFullPath(path);
                        fs.Add(fp);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error with getting full path of {0} file. Message: {1}", path, e.Message);
                    }
                }
                string arg = args[0];
                if (!arg.StartsWith("-"))
                    add(arg);
                if (arguments.ContainsKey("f"))
                {
                    string[] files = arguments["f"];
                    foreach (string file in files)
                        add(file);
                }
            }
            Files = fs.ToArray();
        }

        public string[] Files { get; private set; }

        public bool Contains(string name)
        {
            return arguments.ContainsKey(name);
        }

        public int Count { get { return arguments.Count; } }

        public string Get (string name)
        {
            return arguments[name][0];
        }

        public string[] GetArray (string name)
        {
            return arguments[name];
        }
    }

}
