using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.RemoveDirectory
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new RemoveDirectoryCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
