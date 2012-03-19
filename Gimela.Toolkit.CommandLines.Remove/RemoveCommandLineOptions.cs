using System.Collections.Generic;

namespace Gimela.Toolkit.CommandLines.Remove
{
  internal class RemoveCommandLineOptions
  {
    public RemoveCommandLineOptions()
    {
      Files = new List<string>();
    }

    public IList<string> Files { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
