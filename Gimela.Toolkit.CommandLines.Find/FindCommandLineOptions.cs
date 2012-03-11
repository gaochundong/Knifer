using System.Text;

namespace Gimela.Toolkit.CommandLines.Find
{
  internal class FindCommandLineOptions
  {
    public FindCommandLineOptions()
    {
    }

    public bool IsSetRegexPattern { get; set; }
    public string RegexPattern { get; set; }

    public bool IsSetDirectory { get; set; }
    public string Directory { get; set; }

    public bool IsSetRecursive { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
