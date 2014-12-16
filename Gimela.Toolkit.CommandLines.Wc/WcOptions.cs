using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Wc
{
  internal static class WcOptions
  {
    public static readonly ReadOnlyCollection<string> TotalOptions;
    public static readonly ReadOnlyCollection<string> HelpOptions;
    public static readonly ReadOnlyCollection<string> VersionOptions;

    public static readonly IDictionary<WcOptionType, ICollection<string>> Options;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static WcOptions()
    {
      TotalOptions = new ReadOnlyCollection<string>(new string[] { "t", "total" });
      HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
      VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

      Options = new Dictionary<WcOptionType, ICollection<string>>();
      Options.Add(WcOptionType.Total, TotalOptions);
      Options.Add(WcOptionType.Help, HelpOptions);
      Options.Add(WcOptionType.Version, VersionOptions);
    }

    public static List<string> GetSingleOptions()
    {
      List<string> singleOptionList = new List<string>();

      singleOptionList.AddRange(WcOptions.TotalOptions);
      singleOptionList.AddRange(WcOptions.HelpOptions);
      singleOptionList.AddRange(WcOptions.VersionOptions);

      return singleOptionList;
    }

    #region Usage

    public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	wc - print line counts for files

SYNOPSIS

	wc [OPTION] [FILE]...

DESCRIPTION

	Print line counts for each FILE, and a total line for them.
	With no FILE specified, read file paths but not contents from standard input. 

OPTIONS

	-t, --total
	{0}{0}Just print the total line.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	wc a.txt
	Print line counts for 'a.txt'.

AUTHOR

	Written by Onway.

REPORTING BUGS

	Report bugs to <aluohuai@126.com>.

COPYRIGHT

	Copyright (C) 2014 Onway. All Rights Reserved.
", @" ");

    #endregion

    public static WcOptionType GetOptionType(string option)
    {
      WcOptionType optionType = WcOptionType.None;

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