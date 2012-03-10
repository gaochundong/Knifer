using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Count
{
  public class CountCommandLine : CommandLine
  {
    #region Fields

    private CountCommandLineOptions countOptions;
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
      CommandLineOptions options = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      countOptions = ParseOptions(options);
      CheckOptions(countOptions);

      if (countOptions.IsSetHelp)
      {
        RaiseCommandLineUsage(this, CountOptions.Usage);
      }
      else if (countOptions.IsSetVersion)
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

    private void StartCount()
    {
      try
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        if (countOptions.IsSetDirectory)
        {
          string path = countOptions.Directory.Replace(@"/", @"\\");
          if (path.StartsWith(@"." + Path.DirectorySeparatorChar, StringComparison.CurrentCulture))
          {
            path = currentDirectory.FullName
              + Path.DirectorySeparatorChar
              + path.TrimStart('.', Path.DirectorySeparatorChar);
          }

          CountDirectory(path);
        }

        foreach (var item in countSummary.OrderByDescending(t => t.Value).ThenBy(w => w.Key))
        {
          OutputText(string.Format("FileType: {0,-30}Count: {1}", item.Key.ToLowerInvariant(), item.Value));
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

        if (countOptions.IsSetRecursive)
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

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BigEndianUnicode"), 
     SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static CountCommandLineOptions ParseOptions(CommandLineOptions options)
    {
      if (options == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      CountCommandLineOptions targetOptions = new CountCommandLineOptions();

      if (options.Arguments.Count >= 0)
      {
        foreach (var arg in options.Arguments.Keys)
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
              targetOptions.Directory = options.Arguments[arg];
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

      if (options.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetDirectory)
        {
          targetOptions.IsSetDirectory = true;
          targetOptions.Directory = options.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(CountCommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
        if (!options.IsSetDirectory || string.IsNullOrEmpty(options.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
      }
    }

    #endregion
  }
}
