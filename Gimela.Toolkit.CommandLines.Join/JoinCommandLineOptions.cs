using System.Collections.Generic;

namespace Gimela.Toolkit.CommandLines.Join
{
  internal class JoinCommandLineOptions
  {
    public JoinCommandLineOptions()
    {
      InputFiles = new List<string>();
    }

    public string OutputFile { get; set; }
    public IList<string> InputFiles { get; private set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
