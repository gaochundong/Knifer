using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Foundation
{
  public abstract class CommandLine : ICommandLine
  {
    #region Ctors

    protected CommandLine(string[] args)
    {
      this.Arguments = new ReadOnlyCollection<string>(args);
    }

    #endregion

    #region Properties

    public ReadOnlyCollection<string> Arguments { get; private set; }

    #endregion

    #region ICommandLine Members

    public virtual void Execute()
    {
      IsExecuting = true;
    }

    public virtual void Terminate()
    {
      IsExecuting = false;
    }

    public bool IsExecuting { get; protected set; }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    protected virtual void Dispose(bool disposing)
    {
    }

    #endregion

    #region Events

    public event EventHandler<CommandLineUsageEventArgs> CommandLineUsage;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineUsage(object sender, string usage)
    {
      EventHandler<CommandLineUsageEventArgs> handler = CommandLineUsage;
      if (handler != null)
      {
        handler(sender, new CommandLineUsageEventArgs(usage));
      }
    }

    public event EventHandler<CommandLineDataChangedEventArgs> CommandLineDataChanged;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineDataChanged(object sender, string data)
    {
      EventHandler<CommandLineDataChangedEventArgs> handler = CommandLineDataChanged;
      if (handler != null)
      {
        handler(sender, new CommandLineDataChangedEventArgs(data));
      }
    }

    public event EventHandler<CommandLineExceptionEventArgs> CommandLineException;

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    protected virtual void RaiseCommandLineException(object sender, CommandLineException exception)
    {
      EventHandler<CommandLineExceptionEventArgs> handler = CommandLineException;
      if (handler != null)
      {
        handler(sender, new CommandLineExceptionEventArgs(exception));
      }
    }

    #endregion

    #region Output

    protected virtual void OutputText(string text)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", text, Environment.NewLine));
    }

    #endregion
  }
}
