﻿/*
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.CopySame
{
  internal static class CopySameOptions
  {
    public static readonly ReadOnlyCollection<string> FromOptions;
    public static readonly ReadOnlyCollection<string> ToOptions;
    public static readonly ReadOnlyCollection<string> RecursiveOptions;
    public static readonly ReadOnlyCollection<string> HelpOptions;
    public static readonly ReadOnlyCollection<string> VersionOptions;

    public static readonly IDictionary<CopySameOptionType, ICollection<string>> Options;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static CopySameOptions()
    {
      FromOptions = new ReadOnlyCollection<string>(new string[] { "f", "from", "s", "src", "source" });
      ToOptions = new ReadOnlyCollection<string>(new string[] { "t", "to", "d", "dest", "destination" });
      RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
      HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
      VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

      Options = new Dictionary<CopySameOptionType, ICollection<string>>();
      Options.Add(CopySameOptionType.Source, FromOptions);
      Options.Add(CopySameOptionType.Destination, ToOptions);
      Options.Add(CopySameOptionType.Recursive, RecursiveOptions);
      Options.Add(CopySameOptionType.Help, HelpOptions);
      Options.Add(CopySameOptionType.Version, VersionOptions);
    }

    public static List<string> GetSingleOptions()
    {
      List<string> singleOptionList = new List<string>();

      singleOptionList.AddRange(CopySameOptions.RecursiveOptions);
      singleOptionList.AddRange(CopySameOptions.HelpOptions);
      singleOptionList.AddRange(CopySameOptions.VersionOptions);

      return singleOptionList;
    }

    #region Usage

    public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

  copysame - copy files from source to dest with same name

SYNOPSIS

  copysame [OPTIONS] [SOURCE_FOLDER] [DESTINATION_FOLDER]

DESCRIPTION

  The copysame utility copies the files which have same name
  from source folder to destination folder.

OPTIONS

  -f, --from, -s, --src, --source
  {0}{0}Specify a source folder.
  -t, --to, -d, --dest, --destination
  {0}{0}Specify a destination folder.
  -r, --recursive
  {0}{0}Read all files under each directory recursively.
  -h, --help 
  {0}{0}Display this help and exit.
  -v, --version
  {0}{0}Output version information and exit.

EXAMPLES

  copysame -r -f . -t 'c:\logs1'
  Copy files with same name from current folder to 'c:\logs1'.

AUTHOR

  Written by Chundong Gao.

REPORTING BUGS

  Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

  Copyright (C) 2011-2013 Chundong Gao. All Rights Reserved.
", @" ");

    #endregion

    public static CopySameOptionType GetOptionType(string option)
    {
      CopySameOptionType optionType = CopySameOptionType.None;

      foreach (var pair in Options)
      {
        foreach (var item in pair.Value)
        {
          if (item == option)
          {
            optionType = pair.Key;
            break;
          }
        }
      }

      return optionType;
    }
  }
}
