using DotnetIgnoreCliTool.Cli;
using PowerArgs;
using System;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                await Args.InvokeActionAsync<CommandLineEntryPoint>(args);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}