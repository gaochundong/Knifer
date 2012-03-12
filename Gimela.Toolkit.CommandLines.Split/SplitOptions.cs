using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Split
{
	internal static class SplitOptions
	{
		public static readonly ReadOnlyCollection<string> SuffixLengthOptions;
		public static readonly ReadOnlyCollection<string> BytesOptions;
		public static readonly ReadOnlyCollection<string> LinesOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<SplitOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static SplitOptions()
		{
			SuffixLengthOptions = new ReadOnlyCollection<string>(new string[] { "a", "suffix-length" });
			BytesOptions = new ReadOnlyCollection<string>(new string[] { "b", "bytes" });
			LinesOptions = new ReadOnlyCollection<string>(new string[] { "l", "lines" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<SplitOptionType, ICollection<string>>();
			Options.Add(SplitOptionType.SuffixLength, SuffixLengthOptions);
			Options.Add(SplitOptionType.Bytes, BytesOptions);
			Options.Add(SplitOptionType.Lines, LinesOptions);
			Options.Add(SplitOptionType.Help, HelpOptions);
			Options.Add(SplitOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(SplitOptions.HelpOptions);
			singleOptionList.AddRange(SplitOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Split v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	split - split a file into pieces

SYNOPSIS

	split [OPTION] [INPUT [PREFIX]]

DESCRIPTION

	Output fixed-size pieces of INPUT to PREFIXaa, PREFIXab, ...; 
	default PREFIX is 'x'.

OPTIONS

	-a, --suffix-length=N
	{0}{0}Use suffixes of length N (default 2).
	-b, --bytes=SIZE
	{0}{0}Put SIZE bytes per output file.
	{0}{0}SIZE may have a multiplier suffix: b for 512, k for 1K, 
	{0}{0}m for 1 Meg.
	-l, --lines=NUMBER
	{0}{0}Put NUMBER lines per output file.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	split -b 22 newfile.txt new
	Split the file 'newfile.txt' into three separate files 
	called newaa, newab and newac each file the size of 22.

	split -l 300 file.txt new
	Split the file 'newfile.txt' into files beginning with 
	the name 'new' each containing 300 lines of text each.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static SplitOptionType GetOptionType(string option)
		{
			SplitOptionType optionType = SplitOptionType.None;

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
