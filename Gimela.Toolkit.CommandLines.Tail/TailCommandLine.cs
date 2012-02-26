using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Tail
{
  public class TailCommandLine : CommandLine
  {
    #region Fields

    private TailCommandLineOptions tailOptions;
    private Timer monitoringFileTimer = null;
    private bool isMonitoring = false;
    private FileSystemWatcher fileSystemWatcher = null;
    private long previousSeekPosition = 0;
    private int maxReadBytes = 1024 * 16;
    private long readBytesCount = 0;
    private Encoding encoding = Encoding.ASCII;
    private byte[] newLine = Encoding.ASCII.GetBytes("\n");
    private long readLineCount = 0;
    
    #endregion

    #region Constructors

    public TailCommandLine(string[] args)
    {
      this.Arguments = new ReadOnlyCollection<string>(args);
    }

    #endregion

    #region Properties

    public ReadOnlyCollection<string> Arguments { get; private set; }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = TailOptions.GetSingleOptions();
      CommandLineOptions options = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      tailOptions = ParseOptions(options);

      if (tailOptions.IsSetHelp)
      {
        RaiseCommandLineUsage(this, TailOptions.Usage);
        Terminate();
      }
      else if (tailOptions.IsSetVersion)
      {
        RaiseCommandLineUsage(this, TailOptions.Version);
        Terminate();
      }
      else
      {
        StartWatch();
      }
    }

    public override void Terminate()
    {
      base.Terminate();
      StopWatch();
    }

    #endregion

    #region Private Methods

    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    private void StartWatch()
    {
      try
      {
        FileInfo targetFile = new FileInfo(tailOptions.File);

        previousSeekPosition = 0;

        fileSystemWatcher = new FileSystemWatcher();
        fileSystemWatcher.IncludeSubdirectories = false;
        fileSystemWatcher.Path = targetFile.DirectoryName;
        fileSystemWatcher.Filter = targetFile.Name;

        fileSystemWatcher.Created += new FileSystemEventHandler(OnFileCreated);
        fileSystemWatcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
        fileSystemWatcher.Renamed += new RenamedEventHandler(OnFileRenamed);
        fileSystemWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
        fileSystemWatcher.EnableRaisingEvents = true;

        if (targetFile.Exists)
        {
          while (targetFile.Length > readBytesCount && readLineCount < tailOptions.OutputLines)
          {
            ReadFile();
          }
        }
        else
        {
          if (!tailOptions.IsSetRetry)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "No such file -- {0}", tailOptions.File));
          }
        }

        if (tailOptions.IsSetFollow)
        {
          if (monitoringFileTimer == null)
          {
            monitoringFileTimer = new Timer((TimerCallback)OnMonitoringTimerCallback, tailOptions.File,
                TimeSpan.Zero, TimeSpan.FromSeconds(tailOptions.SleepInterval));
          }
        }
        else
        {
          Terminate();
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    private void StopWatch()
    {
      if (fileSystemWatcher != null)
      {
        fileSystemWatcher.EnableRaisingEvents = false;
        fileSystemWatcher.Dispose();
        fileSystemWatcher = null;
      }

      if (monitoringFileTimer != null)
      {
        monitoringFileTimer.Dispose();
        monitoringFileTimer = null;
      }
    }

    private void RestartWatch()
    {
      StopWatch();
      StartWatch();
    }

    private void ReadFile()
    {
      lock (this)
      {
        if (this.previousSeekPosition == 0)
        {
          ReadFirstTime();
        }
        else
        {
          ReadContinue();
        }
      }
    }

    private void ReadFirstTime()
    {
      byte[] readBytes = new byte[maxReadBytes];
      int numReadBytes = 0;

      this.previousSeekPosition = 0;
      using (FileStream fs = new FileStream(tailOptions.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        if (fs.Length > 0)
        {
          CheckFileEncoding(fs);
        }

        this.previousSeekPosition = 0;
        if (fs.Length > maxReadBytes)
        {
          this.previousSeekPosition = fs.Length - maxReadBytes;
        }

        this.previousSeekPosition = fs.Seek(this.previousSeekPosition, SeekOrigin.Begin);
        numReadBytes = fs.Read(readBytes, 0, (int)maxReadBytes);
        this.previousSeekPosition += numReadBytes;
      }

      if (numReadBytes > 0)
      {
        byte[] newLineBuffer = new byte[newLine.Length];
        byte[] reverseBytes = new byte[maxReadBytes];
        int reverseCount = 0;
        bool isNewLineEqual = true;

        for (int i = numReadBytes - 1; i >= 0; i--)
        {
          Buffer.BlockCopy(readBytes, i, reverseBytes, reverseCount, 1);
          reverseCount++;
          readBytesCount++;

          if (newLineBuffer.Length == 1)
          {
            Buffer.BlockCopy(readBytes, i, newLineBuffer, 0, newLineBuffer.Length);
          }
          else if (newLineBuffer.Length > 1 && numReadBytes - i >= newLineBuffer.Length)
          {
            Buffer.BlockCopy(readBytes, i, newLineBuffer, 0, newLineBuffer.Length);
          }
          isNewLineEqual = true;
          for (int b = 0; b < newLine.Length; b++)
          {
            if (newLineBuffer[b] != newLine[b])
            {
              isNewLineEqual = false;
              break;
            }
          }
          if (isNewLineEqual)
          {
            readLineCount++;
            if (readLineCount - 1 >= tailOptions.OutputLines)
            {
              break;
            }
          }
        }

        if (reverseCount > 0)
        {
          byte[] transferBuffer = new byte[reverseCount];
          int transferIndex = 0;
          for (int i = reverseCount - 1; i >= 0; i--)
          {
            Buffer.BlockCopy(reverseBytes, i, transferBuffer, transferIndex, 1);
            transferIndex++;
          }

          string data = encoding.GetString(transferBuffer, 0, reverseCount);
          RaiseCommandLineDataChanged(this, data + Environment.NewLine);
        }
      }
    }

    private void ReadContinue()
    {
      byte[] readBytes = new byte[maxReadBytes];
      int numReadBytes = 0;

      using (FileStream fs = new FileStream(tailOptions.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        if (fs.Length > this.previousSeekPosition)
        {
          long willReadCount = 0;
          if (fs.Length - this.previousSeekPosition >= maxReadBytes)
          {
            willReadCount = maxReadBytes;
          }
          else
          {
            willReadCount = fs.Length - this.previousSeekPosition;
          }

          this.previousSeekPosition = fs.Seek(this.previousSeekPosition, SeekOrigin.Begin);
          numReadBytes = fs.Read(readBytes, 0, (int)willReadCount);
          this.previousSeekPosition += numReadBytes;
        }
        else if (fs.Length < this.previousSeekPosition)
        {
          this.previousSeekPosition = 0;
        }
      }

      if (numReadBytes > 0)
      {
        byte[] newLineBuffer = new byte[newLine.Length];
        bool isNewLineEqual = true;

        for (int i = 0; i < numReadBytes; i++)
        {
          if (newLineBuffer.Length == 1)
          {
            Buffer.BlockCopy(readBytes, i, newLineBuffer, 0, newLineBuffer.Length);
          }
          else if (newLineBuffer.Length > 1)
          {
            if (numReadBytes - i >= newLineBuffer.Length)
            {
              Buffer.BlockCopy(readBytes, i, newLineBuffer, 0, newLineBuffer.Length);
            }
            else
            {
              Buffer.BlockCopy(readBytes, i, newLineBuffer, 0, 1);
            }
          }
          isNewLineEqual = true;
          for (int b = 0; b < newLine.Length; b++)
          {
            if (newLineBuffer[b] != newLine[b])
            {
              isNewLineEqual = false;
              break;
            }
          }
          if (isNewLineEqual)
          {
            readLineCount++;
          }

          readBytesCount++;
        }

        string data = encoding.GetString(readBytes, 0, numReadBytes);
        RaiseCommandLineDataChanged(this, data);
      }
    }

    private void CheckFileEncoding(FileStream fs)
    {
      byte[] checkEncodingBuffer = new byte[3];

      fs.Read(checkEncodingBuffer, 0, 3);

      if (checkEncodingBuffer[0] == 0xEF
        && checkEncodingBuffer[1] == 0xBB
        && checkEncodingBuffer[2] == 0xBF)
      {
        // UTF-8 : EF BB BF
        encoding = Encoding.UTF8;
        newLine = encoding.GetBytes(Environment.NewLine);
      }
      else if (checkEncodingBuffer[0] == 0xFF
        && checkEncodingBuffer[1] == 0xFE)
      {
        // Unicode Little Endian : FF FE
        encoding = Encoding.Unicode;
        newLine = encoding.GetBytes(Environment.NewLine);
      }
      else if (checkEncodingBuffer[0] == 0xFE
        && checkEncodingBuffer[1] == 0xFF)
      {
        // Unicode Big Endian : FE FF
        encoding = Encoding.BigEndianUnicode;
        newLine = encoding.GetBytes(Environment.NewLine);
      }
    }

    private void OnMonitoringTimerCallback(object state)
    {
      if (!isMonitoring)
      {
        isMonitoring = true;

        FileInfo targetFile = new FileInfo(state.ToString());
        if (targetFile.Exists)
        {
          if (tailOptions.IsSetFollow)
          {
            if (targetFile.Length > this.previousSeekPosition)
            {
              ReadFile();
            }
          }
          else
          {
            if (targetFile.Length > readBytesCount && readLineCount < tailOptions.OutputLines)
            {
              ReadFile();
            }
          }
        }

        isMonitoring = false;
      }
    }

    private void OnFileCreated(object source, FileSystemEventArgs e)
    {
      ReadFile();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
      ReadFile();
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
      RestartWatch();
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
      RestartWatch();
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static TailCommandLineOptions ParseOptions(CommandLineOptions options)
    {
      if (options == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      TailCommandLineOptions targetOptions = new TailCommandLineOptions();

      if (options.Arguments.Count > 0)
      {
        foreach (var arg in options.Arguments.Keys)
        {
          TailOptionType optionType = TailOptions.GetOptionType(arg);
          if (optionType == TailOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case TailOptionType.Retry:
              targetOptions.IsSetRetry = true;
              break;
            case TailOptionType.Follow:
              targetOptions.IsSetFollow = true;
              targetOptions.File = options.Arguments[arg];
              break;
            case TailOptionType.FollowRetry:
              targetOptions.IsSetFollow = true;
              targetOptions.IsSetRetry = true;
              targetOptions.File = options.Arguments[arg];
              break;
            case TailOptionType.OutputLines:
              long outputLines = 0;
              if (!long.TryParse(options.Arguments[arg], out outputLines))
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid output lines option value."));
              }
              if (outputLines <= 0)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid output lines option value."));
              }
              targetOptions.OutputLines = outputLines;
              break;
            case TailOptionType.SleepInterval:
              long sleepInterval = 0;
              if (!long.TryParse(options.Arguments[arg], out sleepInterval))
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid sleep interval option value."));
              }
              if (sleepInterval <= 0)
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid sleep interval option value."));
              }
              targetOptions.SleepInterval = sleepInterval;
              break;
            case TailOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case TailOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (!targetOptions.IsSetHelp && !targetOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(targetOptions.File))
        {
          if (options.Parameters.Count <= 0)
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "must follow a file."));
          if (options.Parameters.Count > 1)
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "can only follow one file."));

          targetOptions.File = options.Parameters.First();
        }
        else
        {
          if (options.Parameters.Count > 0)
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "can only follow one file."));
        }

        if (targetOptions.IsSetRetry && !targetOptions.IsSetFollow)
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "keep trying to open a file should follow a file name explicitly."));
      }

      return targetOptions;
    }

    #endregion

    #region IDisposable Members

    [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        StopWatch();
      }
    }

    #endregion
  }
}
