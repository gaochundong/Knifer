
namespace Gimela.Toolkit.CommandLines.AddText
{
  internal class AddTextCommandLineOptions
  {
    public AddTextCommandLineOptions()
    {
    }

    public string File { get; set; }
    public string Text { get; set; }

    public bool IsSetFromFile { get; set; }
    public string FromFile { get; set; }

    public bool IsSetTop { get; set; }
    public bool IsSetBottom { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
