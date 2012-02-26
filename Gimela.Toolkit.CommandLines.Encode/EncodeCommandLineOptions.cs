using System.Text;

namespace Gimela.Toolkit.CommandLines.Encode
{
  internal class EncodeCommandLineOptions
  {
    public EncodeCommandLineOptions()
    {
    }

    public bool IsSetInputFile { get; set; }
    public string InputFile { get; set; }

    public bool IsSetOutputFile { get; set; }
    public string OutputFile { get; set; }

    public bool IsSetFromEncoding { get; set; }
    public Encoding FromEncoding { get; set; }

    public bool IsSetToEncoding { get; set; }
    public Encoding ToEncoding { get; set; }

    public bool IsSetDirectory { get; set; }
    public string Directory { get; set; }

    public bool IsSetRecursive { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
