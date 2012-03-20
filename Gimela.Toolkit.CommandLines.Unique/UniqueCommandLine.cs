using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Unique
{
  public class UniqueCommandLine : CommandLine
  {
    #region Fields

    private UniqueCommandLineOptions options;

    #endregion

    #region Constructors

    public UniqueCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = UniqueOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, UniqueOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, UniqueOptions.Version);
      }
      else
      {
        StartUnique();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartUnique()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputFile);
        Unique(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void Unique(string path)
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

          IList<string> readText = new List<string>();
          using (StreamReader sr = new StreamReader(file.FullName))
          {
            while (!sr.EndOfStream)
            {
              readText.Add(sr.ReadLine());
            }
          }

          List<string> uniqueText = readText.Distinct().ToList();
          if (options.IsSetSort)
          {
            uniqueText.Sort();
          }

          using (StreamWriter sw = new StreamWriter(renamedFile, false))
          {
            foreach (var item in uniqueText)
            {
              sw.WriteLine(item);
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
    private static UniqueCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      UniqueCommandLineOptions targetOptions = new UniqueCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          UniqueOptionType optionType = UniqueOptions.GetOptionType(arg);
          if (optionType == UniqueOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case UniqueOptionType.InputFile:
              targetOptions.InputFile = commandLineOptions.Arguments[arg];
              break;
            case UniqueOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = commandLineOptions.Arguments[arg];
              break;
            case UniqueOptionType.Sort:
              targetOptions.IsSetSort = true;
              break;
            case UniqueOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case UniqueOptionType.Version:
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

    private static void CheckOptions(UniqueCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.InputFile))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file."));
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
