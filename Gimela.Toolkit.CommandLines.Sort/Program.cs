using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Sort
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new SortCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
