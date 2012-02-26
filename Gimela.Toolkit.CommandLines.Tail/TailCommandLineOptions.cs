
namespace Gimela.Toolkit.CommandLines.Tail
{
  internal class TailCommandLineOptions
  {
    internal TailCommandLineOptions()
    {
      OutputLines = 20;
      SleepInterval = 1;
    }

    internal bool IsSetRetry { get; set; }
    internal bool IsSetFollow { get; set; }
    internal string File { get; set; }

    internal long OutputLines { get; set; }
    internal long SleepInterval { get; set; }

    internal bool IsSetHelp { get; set; }
    internal bool IsSetVersion { get; set; }
  }
}
