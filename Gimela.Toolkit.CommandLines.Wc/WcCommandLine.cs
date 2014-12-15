using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Wc
{
  public class WcCommandLine : CommandLine
  {
    #region Fields

    private WcCommandLineOptions options;

    private string formatString = "{0,12}\t{1}{2}";

    #endregion

    #region Constructors

    public WcCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = WcOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, WcOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartWc();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartWc()
    {
      int totalLine = 0;
      int errorCnt = 0;
      try
      {
        if (options.FilePaths.Count == 0)
        {
          while (true)
          {
            string inLine = Console.ReadLine();
            if (inLine == null)
            {
              break;
            }
            else if (inLine.Trim() == "")
            {
              continue;
            }
            options.FilePaths.Add(inLine.Trim());
          }
        }

        foreach (var filePath in options.FilePaths)
        {
          int lineCnt = CountLine(filePath);
          if (lineCnt == -1)
          {
            ++errorCnt;
          }
          else
          {
            totalLine += lineCnt;
          }
        }

        OutputFormatText(formatString, totalLine, "total", Environment.NewLine);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private int CountLine(string filePath)
    {
      StreamReader sr = null;
      try
      {
        int lineCount = 0;
        sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        while (!sr.EndOfStream)
        {
          sr.ReadLine();
          ++lineCount;
        }
        if (!options.IsSetTotal)
        {
          OutputFormatText(formatString, lineCount, filePath, Environment.NewLine);
        }
        return lineCount;
      }
      catch (IOException ex)
      {
        if (!options.IsSetTotal)
        {
          OutputFormatText(formatString, 0, ex.Message, Environment.NewLine);
        }
        return -1;
      }
      finally
      {
        if (sr != null)
        {
          sr.Dispose();
        }
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static WcCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      WcCommandLineOptions targetOptions = new WcCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          WcOptionType optionType = WcOptions.GetOptionType(arg);
          if (optionType == WcOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case WcOptionType.Total:
              targetOptions.IsSetTotal = true;
              break;
            case WcOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case WcOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      foreach (var param in commandLineOptions.Parameters)
      {
        targetOptions.FilePaths.Add(param);
      }

      return targetOptions;
    }

    #endregion

    #region Version

    public override string Version
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        Assembly assembly = Assembly.GetExecutingAssembly();

        sb.AppendLine();

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "{0} {1}",
          Process.GetCurrentProcess().ProcessName,
          FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion));

        sb.AppendLine();

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Company  : {0}",
          FileVersionInfo.GetVersionInfo(assembly.Location).CompanyName));

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Product  : {0}",
          FileVersionInfo.GetVersionInfo(assembly.Location).ProductName));

        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Copyright: {0}",
          FileVersionInfo.GetVersionInfo(assembly.Location).LegalCopyright));

        return sb.ToString();
      }
    }

    #endregion
  }
}