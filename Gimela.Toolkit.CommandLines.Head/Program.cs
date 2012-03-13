using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Head
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new HeadCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
