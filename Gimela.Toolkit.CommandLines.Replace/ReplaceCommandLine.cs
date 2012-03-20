using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Replace
{
  public class ReplaceCommandLine : CommandLine
  {
    #region Fields

    private ReplaceCommandLineOptions options;

    #endregion

    #region Constructors

    public ReplaceCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = ReplaceOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, ReplaceOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, ReplaceOptions.Version);
      }
      else
      {
        StartReplace();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartReplace()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputFile);
        Replace(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void Replace(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        try
        {
          string renamedFile = file.FullName + ".original";
          File.Delete(renamedFile);

          using (StreamReader sr = new StreamReader(file.FullName))
          using (StreamWriter sw = new StreamWriter(renamedFile, false))
          {
            while (!sr.EndOfStream)
            {
              sw.WriteLine(sr.ReadLine().Replace(options.FromText, options.ToText));
            }
          }

          if (options.IsSetOutputFile)
          {
            string outputPath = WildcardCharacterHelper.TranslateWildcardFilePath(options.OutputFile);
            File.Delete(outputPath);
            File.Move(renamedFile, outputPath);
          }
          else
          {
            File.Delete(file.FullName);
            File.Move(renamedFile, file.FullName);
          }
          File.Delete(renamedFile);
        }
        catch (UnauthorizedAccessException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (PathTooLongException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (DirectoryNotFoundException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (NotSupportedException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
        catch (IOException ex)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Operation exception -- {0}, {1}", file.FullName, ex.Message));
        }
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static ReplaceCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      ReplaceCommandLineOptions targetOptions = new ReplaceCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          ReplaceOptionType optionType = ReplaceOptions.GetOptionType(arg);
          if (optionType == ReplaceOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case ReplaceOptionType.InputFile:
              targetOptions.InputFile = commandLineOptions.Arguments[arg];
              break;
            case ReplaceOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = commandLineOptions.Arguments[arg];
              break;
            case ReplaceOptionType.FromText:
              targetOptions.FromText = commandLineOptions.Arguments[arg];
              break;
            case ReplaceOptionType.ToText:
              targetOptions.ToText = commandLineOptions.Arguments[arg];
              break;
            case ReplaceOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case ReplaceOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (string.IsNullOrEmpty(targetOptions.InputFile))
        {
          targetOptions.InputFile = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(ReplaceCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.InputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a input file."));
        }
        if (string.IsNullOrEmpty(checkedOptions.FromText))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a from string."));
        }
        if (string.IsNullOrEmpty(checkedOptions.ToText))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a to string."));
        }
        if (checkedOptions.IsSetOutputFile && string.IsNullOrEmpty(checkedOptions.OutputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad output file path."));
        }
      }
    }

    #endregion
  }
}
