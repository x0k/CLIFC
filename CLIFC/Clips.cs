using System;
using System.IO;
using System.Linq;
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

        private void Load (string[] files)
        {
            char[] breakChars = { '{', '>' };
            string data = String.Empty;
            foreach (string file in files)
            {
                try
                {
                    FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    TextReader reader = new StreamReader(fs);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            char firstChar = line[0];
                            if (breakChars.Contains(firstChar))
                            {
                                if (data.Length > 0)
                                {
                                    env.LoadFromString(data);
                                    data = String.Empty;
                                }
                                switch (firstChar)
                                {
                                    case '{':
                                        line = line.Substring(1);
                                        int r;
                                        do
                                        { 
                                            r = line.IndexOf('}');
                                            if (r >= 0)
                                            {
                                                data += line.Substring(0, r);
                                            }
                                            else
                                            {
                                                if (line.Length > 0)
                                                {
                                                    if (line.Contains(';'))
                                                        line += "\n";
                                                    data += line;
                                                }
                                                line = reader.ReadLine();
                                            }
                                        } while (r < 0 && line != null);
                                        PrimitiveValue log = env.Eval('(' + data + ')');
                                        PPrint(log);
                                        data = String.Empty;
                                        break;
                                    case '>':
                                        do
                                        {
                                            Console.Write("> ");
                                            line = Console.ReadLine();
                                            if (line.Length == 0 || line == "end")
                                                break;
                                            try
                                            {
                                                PrimitiveValue res = env.Eval(line);
                                                PPrint(res);
                                            }
                                            catch (Exception err)
                                            {
                                                Console.WriteLine("Error: {1}", file, err.Message);
                                            }
                                        } while (true);
                                        break;
                                }
                            }
                            else
                            {
                                if (line.Contains(';'))
                                    line += "\n";
                                data += line;
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("Error with reading file: {0}, Error: {1}", file, err.Message);
                }
            }
            if (data.Length > 0)
            {
                env.LoadFromString(data);
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
