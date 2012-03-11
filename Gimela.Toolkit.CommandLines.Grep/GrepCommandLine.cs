using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;
using System.Reflection;

namespace Gimela.Toolkit.CommandLines.Grep
{
  /// <summary>
  /// Global Regular Expression Print
  /// </summary>
  public class GrepCommandLine : CommandLine
  {
    #region Fields

    private GrepCommandLineOptions grepOptions;
    private readonly string executingFile = Assembly.GetExecutingAssembly().Location;

    #endregion

    #region Constructors

    public GrepCommandLine(string[] args)
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

      List<string> singleOptionList = GrepOptions.GetSingleOptions();
      CommandLineOptions options = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      grepOptions = ParseOptions(options);
      CheckOptions(grepOptions);

      if (grepOptions.IsSetHelp)
      {
        RaiseCommandLineUsage(this, GrepOptions.Usage);
      }
      else if (grepOptions.IsSetVersion)
      {
        RaiseCommandLineUsage(this, GrepOptions.Version);
      }
      else
      {
        StartGrep();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartGrep()
    {
      try
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        foreach (var item in grepOptions.FilePaths)
        {
          string path = item.Replace(@"/", @"\\");
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

          if (grepOptions.IsSetDirectory)
          {
            GrepDirectory(path);
          }
          else
          {
            if (WildcardCharacterHelper.IsContainsWildcard(path))
            {
              FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(path)).GetFiles();
              foreach (var file in files)
              {
                Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(path));
                if (r.IsMatch(file.FullName) || r.IsMatch(file.Name))
                {
                  GrepFile(file.FullName);
                }
              }
            }
            else
            {
              GrepFile(path);
            }
          }
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void GrepFile(string path)
    {
      if (IsCanGrepFile(path))
      {
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
          if (!grepOptions.IsSetNoMessages)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "No such file -- {0}", file.FullName));
          }
        }
        else
        {
          MatchFile(path);
        }
      }
    }

    private void GrepDirectory(string path)
    {
      if (IsCanGrepDirectory(path))
      {
        DirectoryInfo directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
          if (!grepOptions.IsSetNoMessages)
          {
            throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
              "No such directory -- {0}", directory.FullName));
          }
        }
        else
        {
          FileInfo[] files = directory.GetFiles();
          foreach (var file in files)
          {
            GrepFile(file.FullName);
          }

          if (grepOptions.IsSetRecursive)
          {
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (var item in directories)
            {
              GrepDirectory(item.FullName);
            }
          }
        }
      }
    }

    private bool IsCanGrepFile(string file)
    {
      bool result = false;

      if (executingFile == file)
      {
        result = false;
      }
      else if (file.ToUpperInvariant().EndsWith(".EXE"))
      {
        result = false;
      }
      else if (grepOptions.IsSetIncludeFiles)
      {
        Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(grepOptions.IncludeFilesPattern));
        if (r.IsMatch(file))
        {
          result = true;
        }
      }
      else if (grepOptions.IsSetExcludeFiles)
      {
        Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(grepOptions.ExcludeFilesPattern));
        if (!r.IsMatch(file))
        {
          result = true;
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    private bool IsCanGrepDirectory(string directory)
    {
      bool result = false;

      if (grepOptions.IsSetExcludeDirectories)
      {
        Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(grepOptions.ExcludeDirectoriesPattern));
        if (!r.IsMatch(directory))
        {
          result = true;
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    private void MatchFile(string path)
    {
      List<string> readText = new List<string>();
      Stream stream = null;
      try
      {
        stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(stream))
        {
          stream = null;
          while (!sr.EndOfStream)
          {
            readText.Add(sr.ReadLine());
          }
        }
      }
      finally
      {
        if (stream != null)
          stream.Dispose();
      }

      int matchingLineCount = 0;
      for (int i = 0; i < readText.Count; i++)
      {
        if (grepOptions.IsSetFixedStrings)
        {
          if (grepOptions.IsSetInvertMatch)
          {
            if (!readText[i].Contains(grepOptions.RegexPattern))
            {
              matchingLineCount++;
              if (!grepOptions.IsSetCount)
              {
                OutputFileData(path, i+1, readText[i]);
              }
            }
          }
          else
          {
            if (readText[i].Contains(grepOptions.RegexPattern))
            {
              matchingLineCount++;
              if (!grepOptions.IsSetCount)
              {
                OutputFileData(path, i+1, readText[i]);
              }
            }
          }
        }
        else
        {
          Regex r = null;
          if (grepOptions.IsSetIgnoreCase)
          {
            r = new Regex(grepOptions.RegexPattern, RegexOptions.IgnoreCase);
          }
          else
          {
            r = new Regex(grepOptions.RegexPattern, RegexOptions.None);
          }

          Match m = r.Match(readText[i]);
          if (grepOptions.IsSetInvertMatch)
          {
            if (!m.Success)
            {
              matchingLineCount++;
              if (!grepOptions.IsSetCount)
              {
                OutputFileData(path, i+1, readText[i]);
              }
            }
          }
          else
          {
            if (m.Success)
            {
              matchingLineCount++;
              if (!grepOptions.IsSetCount)
              {
                OutputFileData(path, i+1, readText[i]);
              }
            }
          }
        }
      }

      if (grepOptions.IsSetFilesWithoutMatch)
      {
        if (matchingLineCount == 0)
        {
          OutputFilesWithoutMatch(path);
        }
      }
      else if (grepOptions.IsSetFilesWithMatchs)
      {
        if (matchingLineCount > 0)
        {
          OutputFilesWithMatch(path);
        }
      }
      else if (grepOptions.IsSetCount)
      {
        OutputFileMatchingLineCount(path, matchingLineCount);
      }
    }

    private void OutputFilesWithoutMatch(string path)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", path, Environment.NewLine));
    }

    private void OutputFilesWithMatch(string path)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", path, Environment.NewLine));
    }

    private void OutputFileMatchingLineCount(string path, int matchingLineCount)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}:{1} matches.{2}", path, matchingLineCount, Environment.NewLine));
    }

    private void OutputFileData(string path, int lineNumber, string lineText)
    {
      if (grepOptions.IsSetWithFileName)
      {
        if (grepOptions.IsSetLineNumber)
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}:{1}:{2}{3}", path, lineNumber, lineText, Environment.NewLine));
        }
        else
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}:{1}{2}", path, lineText, Environment.NewLine));
        }
      }
      else if (grepOptions.IsSetNoFileName)
      {
        if (grepOptions.IsSetLineNumber)
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}:{1}{2}", lineNumber, lineText, Environment.NewLine));
        }
        else
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}{1}", lineText, Environment.NewLine));
        }
      }
      else
      {
        if (grepOptions.IsSetLineNumber)
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}:{1}:{2}{3}", path, lineNumber, lineText, Environment.NewLine));
        }
        else
        {
          RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
            "{0}:{1}{2}", path, lineText, Environment.NewLine));
        }
      }
    }

    #endregion

    #region Parse Options
    
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static GrepCommandLineOptions ParseOptions(CommandLineOptions options)
    {
      if (options == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      GrepCommandLineOptions targetOptions = new GrepCommandLineOptions();

      if (options.Arguments.Count >= 0)
      {
        foreach (var arg in options.Arguments.Keys)
        {
          GrepOptionType optionType = GrepOptions.GetOptionType(arg);
          if (optionType == GrepOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case GrepOptionType.RegexPattern:
              targetOptions.IsSetRegexPattern = true;
              targetOptions.RegexPattern = options.Arguments[arg];
              break;
            case GrepOptionType.File:
              targetOptions.IsSetPath = true;
              targetOptions.FilePaths.Add(options.Arguments[arg]);
              break;
            case GrepOptionType.FixedStrings:
              targetOptions.IsSetFixedStrings = true;
              break;
            case GrepOptionType.IgnoreCase:
              targetOptions.IsSetIgnoreCase = true;
              break;
            case GrepOptionType.InvertMatch:
              targetOptions.IsSetInvertMatch = true;
              break;
            case GrepOptionType.Count:
              targetOptions.IsSetCount = true;
              break;
            case GrepOptionType.FilesWithoutMatch:
              targetOptions.IsSetFilesWithoutMatch = true;
              break;
            case GrepOptionType.FilesWithMatchs:
              targetOptions.IsSetFilesWithMatchs = true;
              break;
            case GrepOptionType.NoMessages:
              targetOptions.IsSetNoMessages = true;
              break;
            case GrepOptionType.WithFileName:
              targetOptions.IsSetWithFileName = true;
              break;
            case GrepOptionType.NoFileName:
              targetOptions.IsSetNoFileName = true;
              break;
            case GrepOptionType.LineNumber:
              targetOptions.IsSetLineNumber = true;
              break;
            case GrepOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              break;
            case GrepOptionType.ExcludeFiles:
              targetOptions.IsSetExcludeFiles = true;
              targetOptions.ExcludeFilesPattern = options.Arguments[arg];
              break;
            case GrepOptionType.ExcludeDirectories:
              targetOptions.IsSetExcludeDirectories = true;
              targetOptions.ExcludeDirectoriesPattern = options.Arguments[arg];
              break;
            case GrepOptionType.IncludeFiles:
              targetOptions.IsSetIncludeFiles = true;
              targetOptions.IncludeFilesPattern = options.Arguments[arg];
              break;
            case GrepOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case GrepOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case GrepOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (options.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetRegexPattern)
        {
          targetOptions.IsSetRegexPattern = true;
          targetOptions.RegexPattern = options.Parameters.First();

          for (int i = 1; i < options.Parameters.Count; i++)
          {
            targetOptions.IsSetPath = true;
            targetOptions.FilePaths.Add(options.Parameters.ElementAt(i));
          }
        }
        else
        {
          if (!targetOptions.IsSetPath)
          {
            targetOptions.IsSetPath = true;
            foreach (var item in options.Parameters)
            {
              targetOptions.FilePaths.Add(item);
            }
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(GrepCommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
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
        if (!options.IsSetPath)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a path for grep."));
        }
        if (options.FilePaths.Count <= 0)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a path for grep."));
        }
      }
    }

    #endregion
  }
}
