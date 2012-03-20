
namespace Gimela.Toolkit.CommandLines.Sort
{
  internal class SortCommandLineOptions
  {
    public SortCommandLineOptions()
    {
    }

    public string InputFile { get; set; }

    public bool IsSetOutputFile { get; set; }
    public string OutputFile { get; set; }

    public bool IsSetReverseOrder { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
