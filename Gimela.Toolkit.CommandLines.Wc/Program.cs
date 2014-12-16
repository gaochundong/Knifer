using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Wc
{
  class Program
  {
    static void Main(string[] args)
    {
      using (CommandLine command = new WcCommandLine(args))
      {
        CommandLineBootstrap.Start(command);
      }
    }
  }
}
