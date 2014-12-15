using System.Collections.Generic;

namespace Gimela.Toolkit.CommandLines.Wc
{
  internal class WcCommandLineOptions
  {
    public WcCommandLineOptions()
    {
      FilePaths = new List<string>();
    }

    public ICollection<string> FilePaths { get; set; }

    public bool IsSetTotal { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}