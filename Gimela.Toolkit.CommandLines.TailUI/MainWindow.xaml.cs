using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Gimela.Toolkit.CommandLines.Foundation;
using Gimela.Toolkit.CommandLines.Tail;

namespace Gimela.Toolkit.CommandLines.TailUI
{
  public partial class MainWindow : Window
  {
    private const string TailCommand = @"Tail";
    private const string CancelCommand = @"Cancel";

    private TailCommandLine tail = null;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.Filter = "Log files (*.log)|*.log|Text documents (*.txt)|*.txt|All files (*.*)|*.*"; 
      dlg.FilterIndex = 3;

      Nullable<bool> result = dlg.ShowDialog();
      if (result == true)
      {
        tbFileName.Text = dlg.FileName;
        OnTailButtonClick(sender, new RoutedEventArgs());
      }
    }

    private void OnTailButtonClick(object sender, RoutedEventArgs e)
    {
      if (btnTail.Content.ToString() == TailCommand)
      {
        if (string.IsNullOrEmpty(tbFileName.Text))
        {
          MessageBox.Show(this, "Please specify a file for tailing.", this.Title,
            MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
          return;
        }

        btnTail.Content = CancelCommand;
        tbFileName.IsEnabled = false;
        btnOpenFile.IsEnabled = false;
        tbFileData.Document.Blocks.Clear();

        try
        {
          string[] args = new string[] { 
            @"-F",
            tbFileName.Text
          };

          tail = new TailCommandLine(args);
          tail.CommandLineException += new EventHandler<CommandLineExceptionEventArgs>(OnCommandLineException);
          tail.CommandLineDataChanged += new EventHandler<CommandLineDataChangedEventArgs>(OnCommandLineDataChanged);

          ThreadPool.QueueUserWorkItem((WaitCallback)TailExecuter, tail);
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, this.Title,
            MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
      }
      else
      {
        btnTail.Content = TailCommand;
        tbFileName.IsEnabled = true;
        btnOpenFile.IsEnabled = true;

        try
        {
          if (tail != null)
          {
            tail.CommandLineException -= new EventHandler<CommandLineExceptionEventArgs>(OnCommandLineException);
            tail.CommandLineDataChanged -= new EventHandler<CommandLineDataChangedEventArgs>(OnCommandLineDataChanged);
            tail.Terminate();
            tail.Dispose();
            tail = null;
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show(this, ex.Message, this.Title,
            MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
      }
    }

    private void TailExecuter(object state)
    {
      TailCommandLine tcl = (TailCommandLine)state;
      tcl.Execute();

      while (tail != null && tail.IsExecuting) ;
    }

    private void OnCommandLineException(object sender, CommandLineExceptionEventArgs e)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal,
        new Action(() =>
        {
          tbFileData.AppendText(e.Exception.Message);
          tbFileData.ScrollToEnd();
          OnTailButtonClick(sender, new RoutedEventArgs());
        }));
    }

    private void OnCommandLineDataChanged(object sender, CommandLineDataChangedEventArgs e)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal,
        new Action(() =>
        {
          tbFileData.AppendText(e.Data);
          tbFileData.ScrollToEnd();
        }));
    }
  }
}
