
namespace Gimela.Toolkit.CommandLines.Unique
{
  internal class UniqueCommandLineOptions
  {
    public UniqueCommandLineOptions()
    {
    }

    public string InputFile { get; set; }

    public bool IsSetOutputFile { get; set; }
    public string OutputFile { get; set; }

    public bool IsSetSort { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
