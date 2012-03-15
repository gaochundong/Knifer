using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Join
{
	internal static class JoinOptions
	{
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<JoinOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static JoinOptions()
		{
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<JoinOptionType, ICollection<string>>();
			Options.Add(JoinOptionType.OutputFile, OutputFileOptions);
			Options.Add(JoinOptionType.Help, HelpOptions);
			Options.Add(JoinOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(JoinOptions.HelpOptions);
			singleOptionList.AddRange(JoinOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Join v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	join - join lines of two files on a common field

SYNOPSIS

	join [OPTION]... FILE1 FILE2

DESCRIPTION

	The join command forms, a join of the two relations specified 
	by the lines of file1 and file2.

OPTIONS

	-o, --output=FILE
	{0}{0}The output file which should be outputed.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	join -o 'C:\single.log' a.log b.log
	Join the files a.log and b.log into a 'C:\single.log'. 

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static JoinOptionType GetOptionType(string option)
		{
			JoinOptionType optionType = JoinOptionType.None;

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
