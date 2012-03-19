
namespace Gimela.Toolkit.CommandLines.RemoveDirectory
{
  internal class RemoveDirectoryCommandLineOptions
  {
    public RemoveDirectoryCommandLineOptions()
    {
    }

    public string Directory { get; set; }

    public bool IsSetRecursive { get; set; }

    public string RegexPattern { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
