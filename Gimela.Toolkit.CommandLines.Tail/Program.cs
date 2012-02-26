using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Tail
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new TailCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
