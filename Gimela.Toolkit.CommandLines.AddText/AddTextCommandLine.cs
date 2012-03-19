using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.AddText
{
  public class AddTextCommandLine : CommandLine
  {
    #region Fields

    private AddTextCommandLineOptions options;

    #endregion

    #region Constructors

    public AddTextCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = AddTextOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, AddTextOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, AddTextOptions.Version);
      }
      else
      {
        StartAddText();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartAddText()
    {
      try
      {
        string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.File);
        AddText(path);
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void AddText(string path)
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
          string text = options.Text;
          if (options.IsSetFromFile)
          {
            string fromFile = WildcardCharacterHelper.TranslateWildcardFilePath(options.FromFile);
            text = File.ReadAllText(fromFile);
          }

          if (options.IsSetTop)
          {
            string renamedFile = file.FullName + ".original";
            File.Delete(renamedFile);
            File.Move(file.FullName, renamedFile);

            char[] buffer = new char[10000];

            using (StreamReader sr = new StreamReader(renamedFile))
            using (StreamWriter sw = new StreamWriter(file.FullName, false))
            {
              sw.Write(text);

              int read;
              while ((read = sr.Read(buffer, 0, buffer.Length)) > 0)
              {
                sw.Write(buffer, 0, read);
              }
            }

            File.Delete(renamedFile);
          }
          else if(options.IsSetBottom)
          {
            using (StreamWriter sw = new StreamWriter(file.FullName, true, System.Text.Encoding.UTF8))
            {
              sw.AutoFlush = true;
              sw.Write(text);
            }
          }
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
    private static AddTextCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      AddTextCommandLineOptions targetOptions = new AddTextCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          AddTextOptionType optionType = AddTextOptions.GetOptionType(arg);
          if (optionType == AddTextOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case AddTextOptionType.File:
              targetOptions.File = commandLineOptions.Arguments[arg];
              break;
            case AddTextOptionType.Text:
              targetOptions.Text = commandLineOptions.Arguments[arg];
              break;
            case AddTextOptionType.FromFile:
              targetOptions.IsSetFromFile = true;
              targetOptions.FromFile = commandLineOptions.Arguments[arg];
              break;
            case AddTextOptionType.Top:
              targetOptions.IsSetTop = true;
              break;
            case AddTextOptionType.Bottom:
              targetOptions.IsSetBottom = true;
              break;
            case AddTextOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case AddTextOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(AddTextCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (string.IsNullOrEmpty(checkedOptions.File))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file."));
        }
        if (checkedOptions.IsSetFromFile)
        {
          if (string.IsNullOrEmpty(checkedOptions.FromFile))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "bad from file path."));
          }
        }
        else
        {
          if (string.IsNullOrEmpty(checkedOptions.Text))
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "Option used in invalid context -- {0}", "must specify the text string."));
          }
        }
        if (checkedOptions.IsSetFromFile && !string.IsNullOrEmpty(checkedOptions.Text))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "conflict options when specified text string."));
        }
        if (!checkedOptions.IsSetTop && !checkedOptions.IsSetBottom)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a text position option."));
        }
        if (checkedOptions.IsSetTop && checkedOptions.IsSetBottom)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "conflict options when specified text position."));
        }
      }
    }

    #endregion
  }
}
