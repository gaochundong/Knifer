using System;

namespace Gimela.Toolkit.CommandLines.Foundation
{
  public interface ICommandLine : IDisposable
  {
    void Execute();

    void Terminate();

    bool IsExecuting { get; }
  }
}
