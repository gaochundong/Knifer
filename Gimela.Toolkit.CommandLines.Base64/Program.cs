using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Base64
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new Base64CommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}