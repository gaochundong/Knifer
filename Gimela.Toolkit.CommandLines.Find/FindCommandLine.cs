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

namespace Gimela.Toolkit.CommandLines.Find
{
  public class FindCommandLine : CommandLine
  {
    #region Fields

    private FindCommandLineOptions findOptions;

    #endregion

    #region Constructors

    public FindCommandLine(string[] args)
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

      List<string> singleOptionList = FindOptions.GetSingleOptions();
      CommandLineOptions options = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      findOptions = ParseOptions(options);
      CheckOptions(findOptions);

      if (findOptions.IsSetHelp)
      {
        RaiseCommandLineUsage(this, FindOptions.Usage);
      }
      else if (findOptions.IsSetVersion)
      {
        RaiseCommandLineUsage(this, FindOptions.Version);
      }
      else
      {
        StartFind();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartFind()
    {
      try
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        if (findOptions.IsSetDirectory)
        {
          string path = findOptions.Directory.Replace(@"/", @"\\");
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

          FindDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }
    
    private void FindDirectory(string path)
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
          FindFile(file.DirectoryName, file.Name);
        }

        if (findOptions.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            FindDirectory(item.FullName);
          }
        }
      }
    }

    private void FindFile(string directoryName, string fileName)
    {
      Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(findOptions.RegexPattern));
      if (r.IsMatch(fileName))
      {
        OutputText(Path.Combine(directoryName, fileName));
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
    private static FindCommandLineOptions ParseOptions(CommandLineOptions options)
    {
      if (options == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      FindCommandLineOptions targetOptions = new FindCommandLineOptions();

      if (options.Arguments.Count >= 0)
      {
        foreach (var arg in options.Arguments.Keys)
        {
          FindOptionType optionType = FindOptions.GetOptionType(arg);
          if (optionType == FindOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case FindOptionType.RegexPattern:
              targetOptions.IsSetRegexPattern = true;
              targetOptions.RegexPattern = options.Arguments[arg];
              break;
            case FindOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = options.Arguments[arg];
              break;
            case FindOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case FindOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case FindOptionType.Version:
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

        if (options.Parameters.Count >= 2)
        {
          if (!targetOptions.IsSetRegexPattern)
          {
            targetOptions.IsSetRegexPattern = true;
            targetOptions.RegexPattern = options.Parameters.ElementAt(1);
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(FindCommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
        if (!options.IsSetDirectory)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (string.IsNullOrEmpty(options.Directory))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a directory."));
        }
        if (!options.IsSetRegexPattern)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify regex pattern for matching."));
        }
        if (string.IsNullOrEmpty(options.RegexPattern))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify regex pattern for matching."));
        }
      }
    }

    #endregion
  }
}
