using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Count
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new CountCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
