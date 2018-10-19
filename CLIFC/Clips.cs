using System;
using System.IO;
using System.Collections.Generic;
using CLIPSNET;

namespace CLIFC
{
    public class Clips
    {

        private static void PPrint(PrimitiveValue value)//PrintPrimitive
        {
            string data = value.ToString();
            Console.WriteLine(data);
        }

        private CLIPSNET.Environment env;
        private readonly Options options;

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

        private void Load ()
        {
            string[] files = options.Files;
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

        public Clips (string[] args)
        {
            options = new Options(args);
            env = new CLIPSNET.Environment();
            //REPL Mode
            if (options.Count == 0)
            {
                env.CommandLoop();
            }
            else
            {
                //Help
                if (options.Contains("h"))
                    Console.WriteLine("Options:\n" +
                        " -h - this message\n" +
                        " Without parameters - REPL mode\n" +
                        " fileName - Load file and run\n"
                    );
                //Watcher
                if (options.Contains("w"))
                {
                    Watcher watcher = new Watcher(options.Files, () => {
                        Console.WriteLine("Reload.");
                        Run();
                    });
                    watcher.Run();
                }
            }
        }

        public void Run ()
        {
            try
            {
                env.Clear();
                Load();
                env.Run();
            }
            catch (Exception error)
            {
                if (error is CLIPSLoadException)
                {
                    Console.WriteLine("Message: {0}", error.Message);
                    List<CLIPSLineError> errors = (error as CLIPSLoadException).LineErrors;
                    foreach (CLIPSLineError err in errors)
                        Console.WriteLine("Line: {0}, Message: {1}", err.LineNumber, err.Message);
                }
                else
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

    }
}
