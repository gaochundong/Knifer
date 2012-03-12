using System.Text;

namespace Gimela.Toolkit.CommandLines.Checksum
{
  internal class ChecksumCommandLineOptions
  {
    public ChecksumCommandLineOptions()
    {
    }

    public bool IsSetAlgorithm { get; set; }
    public string Algorithm { get; set; }

    public bool IsSetFile { get; set; }
    public string File { get; set; }

    public bool IsSetText { get; set; }
    public string Text { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
