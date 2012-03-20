using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Replace
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new ReplaceCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
