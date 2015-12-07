/*
* [The "BSD Licence"]
* Copyright (c) 2011-2013 Chundong Gao
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions
* are met:
* 1. Redistributions of source code must retain the above copyright
*    notice, this list of conditions and the following disclaimer.
* 2. Redistributions in binary form must reproduce the above copyright
*    notice, this list of conditions and the following disclaimer in the
*    documentation and/or other materials provided with the distribution.
* 3. The name of the author may not be used to endorse or promote products
*    derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE AUTHOR ''AS IS'' AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
* IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
* INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
* NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
* DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
* THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.CopySame
{
    public class CopySameCommandLine : CommandLine
  {
    #region Fields

    private CopySameCommandLineOptions options;

    #endregion

    #region Constructors

    public CopySameCommandLine(string[] args)
      : base(args)
    {
    }

    #endregion

    #region ICommandLine Members

    public override void Execute()
    {
      base.Execute();

      List<string> singleOptionList = CopySameOptions.GetSingleOptions();
      CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
      options = ParseOptions(cloptions);
      CheckOptions(options);

      if (options.IsSetHelp)
      {
        RaiseCommandLineUsage(this, CopySameOptions.Usage);
      }
      else if (options.IsSetVersion)
      {
        RaiseCommandLineUsage(this, Version);
      }
      else
      {
        StartCopyFiles();
      }

      Terminate();
    }

    #endregion

    #region Private Methods

    private void StartCopyFiles()
    {
      try
      {
        if (options.IsSetSourceFolder && options.IsSetDestinationFolder)
        {
          string sourceFolder = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.SourceFolder);
          string destinationFolder = WildcardCharacterHelper.TranslateWildcardDirectoryPath(options.DestinationFolder);

          CopySameFiles(sourceFolder, destinationFolder);
        }
      }
      catch (CommandLineException ex)
      {
        RaiseCommandLineException(this, ex);
      }
    }

    private void CopySameFiles(string sourceFolder, string destinationFolder)
    {
      DirectoryInfo sourceDirectory = new DirectoryInfo(sourceFolder);
      DirectoryInfo destinationDirectory = new DirectoryInfo(destinationFolder);

      if (!sourceDirectory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", sourceDirectory.FullName));
      }
      if (!destinationDirectory.Exists)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "No such directory -- {0}", destinationDirectory.FullName));
      }

      FileInfo[] destFiles = destinationDirectory.GetFiles();
      foreach (var destFile in destFiles)
      {
        CopyFile(destFile, sourceDirectory);
      }

      if (options.IsSetRecursive)
      {
        DirectoryInfo[] destDirectories = destinationDirectory.GetDirectories();
        foreach (var dest in destDirectories)
        {
          DirectoryInfo src = new DirectoryInfo(Path.Combine(sourceDirectory.FullName, dest.Name));
          CopySameFiles(src.FullName, dest.FullName);
        }
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Gimela.Toolkit.CommandLines.Foundation.CommandLine.OutputText(System.String)")]
    private void CopyFile(FileInfo destFile, DirectoryInfo sourceDirectory)
    {
      try
      {
        FileInfo srcFile = new FileInfo(Path.Combine(sourceDirectory.FullName, destFile.Name));
        if (!srcFile.Exists)
        {
          OutputText(string.Format(CultureInfo.CurrentCulture, @"Cannot find '{0}'.", srcFile.FullName));
        }
        else
        {
          srcFile.CopyTo(destFile.FullName, true);
          OutputText(string.Format(CultureInfo.CurrentCulture, @"'{0}' --> '{1}'", srcFile.FullName, destFile.FullName));
        }
      }
      catch (UnauthorizedAccessException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", destFile.FullName, ex.Message));
      }
      catch (PathTooLongException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", destFile.FullName, ex.Message));
      }
      catch (DirectoryNotFoundException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", destFile.FullName, ex.Message));
      }
      catch (NotSupportedException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", destFile.FullName, ex.Message));
      }
      catch (IOException ex)
      {
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Operation exception -- {0}, {1}", destFile.FullName, ex.Message));
      }
    }

    #endregion

    #region Parse Options

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private static CopySameCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
    {
      if (commandLineOptions == null)
        throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
          "Option used in invalid context -- {0}", "must specify a option."));

      CopySameCommandLineOptions targetOptions = new CopySameCommandLineOptions();

      if (commandLineOptions.Arguments.Count >= 0)
      {
        foreach (var arg in commandLineOptions.Arguments.Keys)
        {
          CopySameOptionType optionType = CopySameOptions.GetOptionType(arg);
          if (optionType == CopySameOptionType.None)
            throw new CommandLineException(
              string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
              string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

          switch (optionType)
          {
            case CopySameOptionType.Source:
              targetOptions.IsSetSourceFolder = true;
              targetOptions.SourceFolder = commandLineOptions.Arguments[arg];
              break;
            case CopySameOptionType.Destination:
              targetOptions.IsSetDestinationFolder = true;
              targetOptions.DestinationFolder = commandLineOptions.Arguments[arg];
              break;
            case CopySameOptionType.Recursive:
              targetOptions.IsSetRecursive = true;
              break;
            case CopySameOptionType.Help:
              targetOptions.IsSetHelp = true;
              break;
            case CopySameOptionType.Version:
              targetOptions.IsSetVersion = true;
              break;
          }
        }
      }

      return targetOptions;
    }

    private static void CheckOptions(CopySameCommandLineOptions checkedOptions)
    {
      if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
      {
        if (!checkedOptions.IsSetSourceFolder || string.IsNullOrEmpty(checkedOptions.SourceFolder))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a source folder."));
        }

        if (!checkedOptions.IsSetDestinationFolder || string.IsNullOrEmpty(checkedOptions.DestinationFolder))
        {
          throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
            "Option used in invalid context -- {0}", "must specify a destination folder."));
        }
      }
    }

    #endregion
  }
}
