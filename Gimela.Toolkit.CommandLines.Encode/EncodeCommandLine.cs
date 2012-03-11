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

namespace Gimela.Toolkit.CommandLines.Encode
{
  public class EncodeCommandLine : CommandLine
  {
    #region Fields

    private EncodeCommandLineOptions encodeOptions;

    #endregion

    #region Constructors

    public EncodeCommandLine(string[] args)
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

      List<string> singleOptionList = EncodeOptions.GetSingleOptions();
      CommandLineOptions options = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      encodeOptions = ParseOptions(options);
      CheckOptions(encodeOptions);

      if (encodeOptions.IsSetHelp)
      {
        RaiseCommandLineUsage(this, EncodeOptions.Usage);
      }
      else if (encodeOptions.IsSetVersion)
      {
        RaiseCommandLineUsage(this, EncodeOptions.Version);
      }
      else
      {
        StartEncode();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartEncode()
    {
      try
      {
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);

        if (encodeOptions.IsSetInputFile)
        {
          string path = encodeOptions.InputFile.Replace(@"/", @"\\");
          if (path.StartsWith(@"." + Path.DirectorySeparatorChar, StringComparison.CurrentCulture))
          {
            path = (currentDirectory.FullName
              + Path.DirectorySeparatorChar
              + path.TrimStart('.', Path.DirectorySeparatorChar)).Replace(@"\\", @"\");
          }

          if (WildcardCharacterHelper.IsContainsWildcard(path))
          {
            FileInfo[] files = new DirectoryInfo(Path.GetDirectoryName(path)).GetFiles();
            foreach (var file in files)
            {
              Regex r = new Regex(WildcardCharacterHelper.WildcardToRegex(path));
              if (r.IsMatch(file.FullName) || r.IsMatch(file.Name))
              {
                EncodeFile(file.FullName);
              }
            }
          }
          else
          {
            EncodeFile(path);
          }
        }

        if (encodeOptions.IsSetDirectory)
        {
          string path = encodeOptions.Directory.Replace(@"/", @"\\");
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

          EncodeDirectory(path);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void EncodeFile(string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        if (encodeOptions.IsSetOutputFile)
        {
          ConvertFile(path, encodeOptions.OutputFile);
        }
        else
        {
          ConvertFile(path, path);
        }
      }
    }

    private void EncodeDirectory(string path)
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
          EncodeFile(file.FullName);
        }

        if (encodeOptions.IsSetRecursive)
        {
          DirectoryInfo[] directories = directory.GetDirectories();
          foreach (var item in directories)
          {
            EncodeDirectory(item.FullName);
          }
        }
      }
    }

    private void ConvertFile(string fromFile, string toFile)
    {
      string text = string.Empty;
      Stream fileReadStream = null;
      Stream fileWriteStream = null;

      try
      {
        fileReadStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileReadStream, encodeOptions.FromEncoding))
        {
          fileReadStream = null;
          text = sr.ReadToEnd();
        }
      }
      finally
      {
        if (fileReadStream != null)
          fileReadStream.Dispose();
      }

      File.Delete(toFile);

      try
      {
        fileWriteStream = new FileStream(toFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        using (StreamWriter sw = new StreamWriter(fileWriteStream, encodeOptions.ToEncoding))
        {
          fileWriteStream = null;
          sw.AutoFlush = true;
          sw.Write(text);
          sw.Flush();
        }
      }
      finally
      {
        if (fileWriteStream != null)
          fileWriteStream.Dispose();
      }

      OutputFileInformation(string.Format(CultureInfo.CurrentCulture,
        "From {0,-20} file : {1}", encodeOptions.FromEncoding.EncodingName, fromFile));
      OutputFileInformation(string.Format(CultureInfo.CurrentCulture,
        "To   {0,-20} file : {1}", encodeOptions.ToEncoding.EncodingName, toFile));
    }

    private void OutputFileInformation(string information)
    {
      RaiseCommandLineDataChanged(this, string.Format(CultureInfo.CurrentCulture,
        "{0}{1}", information, Environment.NewLine));
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "BigEndianUnicode"), 
     SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static EncodeCommandLineOptions ParseOptions(CommandLineOptions options)
    {
      if (options == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      EncodeCommandLineOptions targetOptions = new EncodeCommandLineOptions();

      if (options.Arguments.Count >= 0)
      {
        foreach (var arg in options.Arguments.Keys)
        {
          EncodeOptionType optionType = EncodeOptions.GetOptionType(arg);
          if (optionType == EncodeOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case EncodeOptionType.InputFile:
              targetOptions.IsSetInputFile = true;
              targetOptions.InputFile = options.Arguments[arg];
              break;
            case EncodeOptionType.OutputFile:
              targetOptions.IsSetOutputFile = true;
              targetOptions.OutputFile = options.Arguments[arg];
              break;
            case EncodeOptionType.FromEncoding:
              targetOptions.IsSetFromEncoding = true;
              if (options.Arguments[arg].ToUpperInvariant() == @"ASCII")
              {
                targetOptions.FromEncoding = Encoding.ASCII;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF7")
              {
                targetOptions.FromEncoding = Encoding.UTF7;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF8")
              {
                targetOptions.FromEncoding = Encoding.UTF8;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UNICODE")
              {
                targetOptions.FromEncoding = Encoding.Unicode;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF32")
              {
                targetOptions.FromEncoding = Encoding.UTF32;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"BIGENDIANUNICODE")
              {
                targetOptions.FromEncoding = Encoding.BigEndianUnicode;
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, BigEndianUnicode."));
              }
              break;
            case EncodeOptionType.ToEncoding:
              targetOptions.IsSetToEncoding = true;
              if (options.Arguments[arg].ToUpperInvariant() == @"ASCII")
              {
                targetOptions.ToEncoding = Encoding.ASCII;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF7")
              {
                targetOptions.ToEncoding = Encoding.UTF7;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF8")
              {
                targetOptions.ToEncoding = Encoding.UTF8;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UNICODE")
              {
                targetOptions.ToEncoding = Encoding.Unicode;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"UTF32")
              {
                targetOptions.ToEncoding = Encoding.UTF32;
              }
              else if (options.Arguments[arg].ToUpperInvariant() == @"BIGENDIANUNICODE")
              {
                targetOptions.ToEncoding = Encoding.BigEndianUnicode;
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid encoding, support ASCII, UTF7, UTF8, UTF32, Unicode, BigEndianUnicode."));
              }
              break;
            case EncodeOptionType.Directory:
              targetOptions.IsSetDirectory = true;
              targetOptions.Directory = options.Arguments[arg];
              break;
            case EncodeOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case EncodeOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case EncodeOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      if (options.Parameters.Count > 0)
      {
        if (!targetOptions.IsSetInputFile)
        {
          targetOptions.IsSetInputFile = true;
          targetOptions.InputFile = options.Parameters.First();
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(EncodeCommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
        if (!options.IsSetInputFile && !options.IsSetDirectory)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a input file or a directory."));
        }
        if (!options.IsSetFromEncoding)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify the input file current encoding."));
        }
        if (!options.IsSetToEncoding)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify the output file target encoding."));
        }
        if (options.IsSetInputFile && WildcardCharacterHelper.IsContainsWildcard(options.InputFile) 
          && options.IsSetOutputFile)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "output file path has been set, so can only set one input file."));
        }
        if (options.IsSetDirectory && options.IsSetOutputFile)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "output file path has been set, so can not set a input directory."));
        }
      }
    }

    #endregion
  }
}
