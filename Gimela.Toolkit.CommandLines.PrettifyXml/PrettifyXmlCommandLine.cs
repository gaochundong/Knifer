/*
* [The "BSD Licence"]
* Copyright (c) 2011-2015 Chundong Gao
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
using System.Text;
using System.Xml.Linq;
using Gimela.Toolkit.CommandLines.Foundation;

namespace Gimela.Toolkit.CommandLines.PrettifyXml
{
    public class PrettifyXmlCommandLine : CommandLine
    {
        #region Fields

        private PrettifyXmlCommandLineOptions options;

        #endregion

        #region Constructors

        public PrettifyXmlCommandLine(string[] args)
          : base(args)
        {
        }

        #endregion

        #region ICommandLine Members

        public override void Execute()
        {
            base.Execute();

            List<string> singleOptionList = PrettifyXmlOptions.GetSingleOptions();
            CommandLineOptions cloptions = CommandLineParser.Parse(Arguments.ToArray<string>(), singleOptionList.ToArray());
            options = ParseOptions(cloptions);
            CheckOptions(options);

            if (options.IsSetHelp)
            {
                RaiseCommandLineUsage(this, PrettifyXmlOptions.Usage);
            }
            else if (options.IsSetVersion)
            {
                RaiseCommandLineUsage(this, Version);
            }
            else
            {
                StartPrettifyXml();
            }

            Terminate();
        }

        #endregion

        #region Private Methods

        private void StartPrettifyXml()
        {
            try
            {
                string path = WildcardCharacterHelper.TranslateWildcardFilePath(options.InputDirectory);
                PrettifyXml(path);
            }
            catch (CommandLineException ex)
            {
                RaiseCommandLineException(this, ex);
            }
        }

        private void PrettifyXml(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "No such directory -- {0}", dir.FullName));
            }
            else
            {
                try
                {
                    FileInfo[] files = dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        string unformattedXml = null;

                        using (var stream = file.OpenRead())
                        using (var reader = new StreamReader(stream))
                        {
                            unformattedXml = reader.ReadToEnd();
                        }

                        string formattedXml = XElement.Parse(unformattedXml).ToString();

                        using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                        {
                            writer.AutoFlush = true;
                            writer.Write(formattedXml);
                        }

                        OutputText(string.Format(CultureInfo.CurrentCulture, "Prettify Xml: {0}", file.FullName));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
                }
                catch (PathTooLongException ex)
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
                }
                catch (NotSupportedException ex)
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
                }
                catch (IOException ex)
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Operation exception -- {0}, {1}", dir.FullName, ex.Message));
                }
            }
        }

        #endregion

        #region Parse Options

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static PrettifyXmlCommandLineOptions ParseOptions(CommandLineOptions commandLineOptions)
        {
            if (commandLineOptions == null)
                throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                  "Option used in invalid context -- {0}", "must specify a directory."));

            PrettifyXmlCommandLineOptions targetOptions = new PrettifyXmlCommandLineOptions();

            if (commandLineOptions.Arguments.Count > 0)
            {
                foreach (var arg in commandLineOptions.Arguments.Keys)
                {
                    PrettifyXmlOptionType optionType = PrettifyXmlOptions.GetOptionType(arg);
                    if (optionType == PrettifyXmlOptionType.None)
                        throw new CommandLineException(
                          string.Format(CultureInfo.CurrentCulture, "Option used in invalid context -- {0}",
                          string.Format(CultureInfo.CurrentCulture, "cannot parse the command line argument : [{0}].", arg)));

                    switch (optionType)
                    {
                        case PrettifyXmlOptionType.InputDirectory:
                            targetOptions.InputDirectory = commandLineOptions.Arguments[arg];
                            break;
                        case PrettifyXmlOptionType.Help:
                            targetOptions.IsSetHelp = true;
                            break;
                        case PrettifyXmlOptionType.Version:
                            targetOptions.IsSetVersion = true;
                            break;
                    }
                }
            }

            if (commandLineOptions.Parameters.Count > 0)
            {
                if (string.IsNullOrEmpty(targetOptions.InputDirectory))
                {
                    targetOptions.InputDirectory = commandLineOptions.Parameters.First();
                }
            }

            // set default the current directory
            if (string.IsNullOrEmpty(targetOptions.InputDirectory))
            {
                targetOptions.InputDirectory = @".";
            }

            return targetOptions;
        }

        private static void CheckOptions(PrettifyXmlCommandLineOptions checkedOptions)
        {
            if (!checkedOptions.IsSetHelp && !checkedOptions.IsSetVersion)
            {
                if (string.IsNullOrEmpty(checkedOptions.InputDirectory))
                {
                    throw new CommandLineException(string.Format(CultureInfo.CurrentCulture,
                      "Option used in invalid context -- {0}", "must specify a directory."));
                }
            }
        }

        #endregion
    }
}
