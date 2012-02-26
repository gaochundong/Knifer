using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Encode
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new EncodeCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
