
namespace Gimela.Toolkit.CommandLines.Head
{
  internal class HeadCommandLineOptions
  {
    public HeadCommandLineOptions()
    {
      Number = 10;
    }

    public bool IsSetFile { get; set; }
    public string File { get; set; }

    public long Number { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
