using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Count
{
  public class CountCommandLine : CommandLine
  {
    #region Fields

    private CountCommandLineOptions options;
    private readonly IDictionary<string, int> countSummary;

    #endregion

    #region Constructors

    public CountCommandLine(string[] args)
    {
      this.Arguments = new ReadOnlyCollection<string>(args);
      this.countSummary = new Dictionary<string, int>();
    }

    #endregion

    #region Properties

    public ReadOnlyCollection<string> Arguments { get; private set; }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = CountOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, CountOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, CountOptions.Version);
      }
      else
      {
        StartCount();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Toolkit.CommandLines.Count.CountCommandLine.OutputText(System.String)"),
     SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase"),
     SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FileType")]
    private void StartCount()
    {
      try
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        if (options.IsSetDirectory)
        {
          string path = options.Directory.Replace(@"/", @"\\");
          if (path == @".")
          {
            path = currentDirectory.FullName;
          }
          else if (path.StartsWith(@"." + Path.DirectorySeparatorChar, StringComparison.CurrentCulture))
          {
            path = (currentDirectory.FullName
              + Path.DirectorySeparatorChar
              + path.TrimStart('.', Path.DirectorySeparatorChar)).Replace(@"\\", @"\");
          }

          CountDirectory(path);
        }

        foreach (var item in countSummary.OrderByDescending(t => t.Value).ThenBy(w => w.Key))
        {
          OutputText(string.Format(CultureInfo.CurrentCulture, "FileType: {0,-30}Count: {1}", item.Key.ToLowerInvariant(), item.Value));
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void CountDirectory(string path)
    {
      DirectoryInfo directory = new DirectoryInfo(path);
      if (!directory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", directory.FullName));
      }
      else
      {
        FileInfo[] files = directory.GetFiles();
        foreach (var file in files)
        {
          CountFile(file.FullName);
        }

        if (options.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            CountDirectory(item.FullName);
          }
        }
      }
    }

    private void CountFile(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        if (countSummary.ContainsKey(file.Extension.ToUpperInvariant()))
        {
          countSummary[file.Extension.ToUpperInvariant()]++;
        }
        else
        {
          countSummary.Add(file.Extension.ToUpperInvariant(), 1);
        }
      }
    }

    private void OutputText(string text)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", text, Environment.NewLine));
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static CountCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      CountCommandLineOptions targetOptions = new CountCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          CountOptionType optionType = CountOptions.GetOptionType(arg);
          if (optionType == CountOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case CountOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = commandLineOptions.Arguments[arg];
              break;
            case CountOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case CountOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case CountOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (commandLineOptions.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetDirectory)
        {
          targetOptions.IsSetDirectory = true;
          targetOptions.Directory = commandLineOptions.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(CountCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetDirectory || string.IsNullOrEmpty(checkedOptions.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
      }
    }

    #endregion
  }
}
