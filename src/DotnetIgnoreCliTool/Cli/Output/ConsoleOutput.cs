using System;

namespace DotnetIgnoreCliTool.Cli.Output
{
    public class ConsoleOutput : IOutput
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}