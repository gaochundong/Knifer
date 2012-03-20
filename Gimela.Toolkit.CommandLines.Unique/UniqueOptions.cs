using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Unique
{
	internal static class UniqueOptions
	{
		public static readonly ReadOnlyCollection<string> InputFileOptions;
		public static readonly ReadOnlyCollection<string> OutputFileOptions;
		public static readonly ReadOnlyCollection<string> SortOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<UniqueOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static UniqueOptions()
		{
			InputFileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			OutputFileOptions = new ReadOnlyCollection<string>(new string[] { "o", "output" });
			SortOptions = new ReadOnlyCollection<string>(new string[] { "s", "sort" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<UniqueOptionType, ICollection<string>>();
			Options.Add(UniqueOptionType.InputFile, InputFileOptions);
			Options.Add(UniqueOptionType.OutputFile, OutputFileOptions);
			Options.Add(UniqueOptionType.Sort, SortOptions);
			Options.Add(UniqueOptionType.Help, HelpOptions);
			Options.Add(UniqueOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(UniqueOptions.SortOptions);
			singleOptionList.AddRange(UniqueOptions.HelpOptions);
			singleOptionList.AddRange(UniqueOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Unique v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	uniq - remove duplicate lines from a file

SYNOPSIS

	uniq [OPTION] FILE

DESCRIPTION

	Report or filter out repeated lines in a file.

OPTIONS

	-f, --file=FILE
	{0}{0}The FILE that needs to be filtered.
	-o, --output=FILE
	{0}{0}The FILE represents the output file.
	-s, --sort
	{0}{0}Sort the file content after distinct.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	uniq myfile1.txt -o myfile2.txt
	Removes duplicate lines in the first 'myfile1.txt' and 
	outputs the results to the second file 'myfile2.txt'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static UniqueOptionType GetOptionType(string option)
		{
			UniqueOptionType optionType = UniqueOptionType.None;

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
