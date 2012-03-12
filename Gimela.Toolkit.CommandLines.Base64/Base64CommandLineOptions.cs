using System.Text;

namespace Gimela.Toolkit.CommandLines.Base64
{
  internal class Base64CommandLineOptions
  {
    public Base64CommandLineOptions()
    {
    }

    public bool IsSetDecode { get; set; }

    public bool IsSetEncoding { get; set; }
    public Encoding Encoding { get; set; }

    public bool IsSetText { get; set; }
    public string Text { get; set; }

    public bool IsSetFile { get; set; }
    public string File { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
