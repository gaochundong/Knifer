
namespace Gimela.Toolkit.CommandLines.Count
{
  internal class CountCommandLineOptions
  {
    public CountCommandLineOptions()
    {
    }

    public bool IsSetDirectory { get; set; }
    public string Directory { get; set; }

    public bool IsSetRecursive { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
