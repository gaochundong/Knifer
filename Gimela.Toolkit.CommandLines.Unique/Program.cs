using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Unique
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new UniqueCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
