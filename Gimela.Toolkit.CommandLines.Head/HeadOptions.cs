using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Head
{
	internal static class HeadOptions
	{
		public static readonly ReadOnlyCollection<string> FileOptions;
		public static readonly ReadOnlyCollection<string> NumberOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<HeadOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static HeadOptions()
		{
			FileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			NumberOptions = new ReadOnlyCollection<string>(new string[] { "n", "number" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<HeadOptionType, ICollection<string>>();
			Options.Add(HeadOptionType.File, FileOptions);
			Options.Add(HeadOptionType.Number, NumberOptions);
			Options.Add(HeadOptionType.Help, HelpOptions);
			Options.Add(HeadOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(HeadOptions.HelpOptions);
			singleOptionList.AddRange(HeadOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Head v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	head - displays the first ten lines of a file

SYNOPSIS

	head [OPTION] [FILE]

DESCRIPTION

	Displays the first ten lines of a file, 
	unless otherwise stated.

OPTIONS

	-f, --file
	{0}{0}The file that you want to display the x amount 
	{0}{0}of lines of.
	-n, --number
	{0}{0}The number of the you want to display.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	head -15 myfile.txt
	Display the first fifteen lines of myfile.txt.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static HeadOptionType GetOptionType(string option)
		{
			HeadOptionType optionType = HeadOptionType.None;

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
