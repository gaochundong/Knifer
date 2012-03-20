
namespace Gimela.Toolkit.CommandLines.Replace
{
  internal class ReplaceCommandLineOptions
  {
    public ReplaceCommandLineOptions()
    {
    }

    public string InputFile { get; set; }

    public bool IsSetOutputFile { get; set; }
    public string OutputFile { get; set; }

    public string FromText { get; set; }
    public string ToText { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
