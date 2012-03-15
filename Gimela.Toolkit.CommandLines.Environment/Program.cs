using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Environment
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new EnvironmentCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
