using System;
using System.IO;
using System.Collections.Generic;
using CLIPSNET;
using CommandLine;

namespace CLIFC
{
    public class Clips
    {

        private static void PPrint(PrimitiveValue value)
        {
            string data = value.ToString();
            Console.WriteLine(data);
        }

        private CLIPSNET.Environment env;
        private Action run;

        private void LoadData (string data)
        {
            if (data.Contains("{"))
            {
                data = "}" + data;
                int r = 0;
                int l = data.IndexOf('{');
                do
                {
                    if (l - r > 3)
                    {
                        string res = data.Substring(r + 1, l - r - 1);
                        env.LoadFromString(res);
                    }
                    r = data.IndexOf('}', l + 3);
                    string val = data.Substring(l + 1, r - l - 1);
                    PrimitiveValue log = env.Eval(val);
                    PPrint(log);
                    if (r < data.Length - 1)
                        l = data.IndexOf('{', r);
                    else
                        break;
                } while (l > -1);
                if (r < data.Length - 1)
                {
                    l = data.Length;
                    string res = data.Substring(r + 1, l - r - 1);
                    env.LoadFromString(res);
                }
            }
            else
            {
                env.LoadFromString(data);
            }
        }

        private void Load (string[] files)
        {
            foreach (string file in files)
            {
                try
                {
                    using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (TextReader reader = new StreamReader(fs))
                        {
                            string data = reader.ReadToEnd();
                            LoadData(data);
                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("Error with reading file: {0}, Error: {1}", file, err.Message);
                }
            }
        }

        private void Init (Options options)
        {
            var list = new List<string>();
            if (options.File != null)
                list.Add(options.File);
            if (options.Files != null)
                list.AddRange(options.Files);
            var arr = list.ToArray();
            list = null;
            if (arr.Length == 0)
            {
                run = () =>
                {
                    env.CommandLoop();
                };
            }
            else if (arr.Length > 0)
            {
                run = () =>
                {
                    env.Clear();
                    Load(arr);
                    env.Reset();
                    env.Run();
                };
                if (options.Watcher)
                {
                    Watcher watcher = new Watcher(arr, () =>
                    {
                        Console.WriteLine("Reload.");
                        Run();
                    });
                    watcher.Run();
                }
            }
        }

        public Clips (string[] args)
        {
            env = new CLIPSNET.Environment();
            
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Init);
        }

        public void Run ()
        {
            try
            {
                run?.Invoke();
            }
            catch (Exception error)
            {
                if (error is CLIPSLoadException)
                {
                    Console.WriteLine("Message: {0}", error.Message);
                    List<CLIPSLineError> errors = (error as CLIPSLoadException).LineErrors;
                    foreach (CLIPSLineError err in errors)
                        Console.WriteLine("Line: {0}: {1}", err.LineNumber, err.Message);
                }
                else
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

    }
}
