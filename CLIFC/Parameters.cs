using System.Collections.Generic;
using CommandLine;

namespace CLIFC
{

    class Options
    {
        [Value(0, Default = null, HelpText = "File to be processed", Required = false, MetaName = "input file")]
        public string File { get; set; }

        [Option('f', Separator =' ', HelpText = "Additions files to be processed.", Required = false)]
        public IEnumerable<string> Files { get; set; }

        [Option('w', Default = false, HelpText = "Watcher mode, app will restart after changing files.", Required = false)]
        public bool Watcher { get; set; }
    }

}
