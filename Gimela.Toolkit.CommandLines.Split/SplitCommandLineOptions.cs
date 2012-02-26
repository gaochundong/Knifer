using System.Collections.Generic;

namespace Gimela.Toolkit.CommandLines.Split
{
  internal class SplitCommandLineOptions
  {
    public SplitCommandLineOptions()
    {
      SuffixLength = 2;
    }

    public int SuffixLength { get; set; }

    public bool IsSetBytes { get; set; }
    public int Bytes { get; set; }

    public bool IsSetLines { get; set; }
    public int Lines { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
