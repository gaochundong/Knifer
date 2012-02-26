using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Grep
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new GrepCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
