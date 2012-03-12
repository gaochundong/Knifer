using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.Checksum
{
  public class ChecksumCommandLine : CommandLine
  {
    #region Fields

    private ChecksumCommandLineOptions options;

    #endregion

    #region Constructors

    public ChecksumCommandLine(string[] args)
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

      List<string> singleOptionList = ChecksumOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, ChecksumOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, ChecksumOptions.Version);
      }
      else
      {
        StartChecksum();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartChecksum()
    {
      try
      {
        if (options.IsSetText)
        {
          ChecksumText(options.Algorithm.ToUpperInvariant(), options.Text);
        }
        else if (options.IsSetFile)
        {
          ChecksumFile(options.Algorithm.ToUpperInvariant(), options.File);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void ChecksumText(string algorithm, string text)
    {
      Checksum(algorithm, Encoding.UTF8.GetBytes(text));
    }

    private void ChecksumFile(string algorithm, string path)
    {
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such file -- {0}", file.FullName));
      }
      else
      {
        using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          byte[] readBytes = new byte[fs.Length];
          fs.Read(readBytes, 0, (int)fs.Length);
          Checksum(algorithm, readBytes);
        }
      }
    }

    private void Checksum(string algorithm, byte[] data)
    {
      if (algorithm == @"CRC32")
      {
        long crc = CRC32CryptoServiceProvider.Compute(data);
        OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", crc));
      }
      else if (algorithm == @"CRC64")
      {
        ulong crc = CRC64CryptoServiceProvider.Compute(data);
        OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", crc));
      }
      else if (algorithm == @"MD5")
      {
        using (MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
        }
      }
      else if (algorithm == @"SHA1")
      {
        using (SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
        }
      }
      else if (algorithm == @"SHA256")
      {
        using (SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
        }
      }
      else if (algorithm == @"SHA384")
      {
        using (SHA384CryptoServiceProvider hasher = new SHA384CryptoServiceProvider())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
        }
      }
      else if (algorithm == @"SHA512")
      {
        using (SHA512CryptoServiceProvider hasher = new SHA512CryptoServiceProvider())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
        }
      }
      else if (algorithm == @"RIPEMD160")
      {
        using (RIPEMD160 hasher = RIPEMD160Managed.Create())
        {
          byte[] list = hasher.ComputeHash(data);
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < list.Length; i++)
          {
            sb.Append(list[i].ToString("x2", CultureInfo.CurrentCulture));
          }
          OutputText(string.Format(CultureInfo.CurrentCulture, "{0}", sb.ToString()));
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
    private static ChecksumCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      ChecksumCommandLineOptions targetOptions = new ChecksumCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          ChecksumOptionType optionType = ChecksumOptions.GetOptionType(arg);
          if (optionType == ChecksumOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case ChecksumOptionType.Algorithm:
              targetOptions.IsSetAlgorithm = true;
              if (commandLineOptions.Arguments[arg].ToUpperInvariant() == @"CRC32"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"CRC64"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"MD5"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"SHA1"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"SHA256"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"SHA384"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"SHA512"
                || commandLineOptions.Arguments[arg].ToUpperInvariant() == @"RIPEMD160"
                )
              {
                targetOptions.Algorithm = commandLineOptions.Arguments[arg];
              }
              else
              {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "invalid algorithm, support CRC32, CRC64, MD5, SHA1, SHA256, SHA384, SHA512, RIPEMD160."));
              }
              break;
            case ChecksumOptionType.File:
              targetOptions.IsSetFile = true;
              targetOptions.File = commandLineOptions.Arguments[arg];
              break;
            case ChecksumOptionType.Text:
              targetOptions.IsSetText = true;
              targetOptions.Text = commandLineOptions.Arguments[arg];
              break;
            case ChecksumOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case ChecksumOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(ChecksumCommandLineOptions options)
    {
      if (!options.IsSetHelp && !options.IsSetVersion)
      {
        if (!options.IsSetAlgorithm || string.IsNullOrEmpty(options.Algorithm))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a algorithm."));
        }
        if (!options.IsSetFile && !options.IsSetText)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a file or a text."));
        }
        if (options.IsSetFile && options.IsSetText)
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "can only specify a file or a text."));
        }
        if (options.IsSetFile && string.IsNullOrEmpty(options.File))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad file path."));
        }
        if (options.IsSetText && string.IsNullOrEmpty(options.Text))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "bad text format."));
        }
      }
    }

    #endregion
  }
}
