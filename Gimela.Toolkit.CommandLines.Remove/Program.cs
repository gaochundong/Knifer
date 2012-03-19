using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Remove
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new RemoveCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
