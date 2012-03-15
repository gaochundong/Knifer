
namespace Gimela.Toolkit.CommandLines.Split
{
  internal class SplitCommandLineOptions
  {
    public SplitCommandLineOptions()
    {
      SuffixLength = 2;
      Directory = ".";
    }

    public bool IsSetFile { get; set; }
    public string File { get; set; }

    public bool IsSetPrefix { get; set; }
    public string Prefix { get; set; }

    public int SuffixLength { get; set; }
    public int Bytes { get; set; }
    public string Directory { get; set; }

    public bool IsSetTimestamp { get; set; }
    public bool IsSetOverwrite { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
