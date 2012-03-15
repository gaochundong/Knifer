using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Split
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new SplitCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
