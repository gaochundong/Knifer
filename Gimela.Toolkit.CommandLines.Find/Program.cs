using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Find
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new FindCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
