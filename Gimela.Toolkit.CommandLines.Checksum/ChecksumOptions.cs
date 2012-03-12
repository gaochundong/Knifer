using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Checksum
{
	internal static class ChecksumOptions
	{
		public static readonly ReadOnlyCollection<string> AlgorithmOptions;
		public static readonly ReadOnlyCollection<string> FileOptions;
		public static readonly ReadOnlyCollection<string> TextOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<ChecksumOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ChecksumOptions()
		{
			AlgorithmOptions = new ReadOnlyCollection<string>(new string[] { "a", "algorithm" });
			FileOptions = new ReadOnlyCollection<string>(new string[] { "f", "file" });
			TextOptions = new ReadOnlyCollection<string>(new string[] { "t", "text" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<ChecksumOptionType, ICollection<string>>();
			Options.Add(ChecksumOptionType.Algorithm, AlgorithmOptions);
			Options.Add(ChecksumOptionType.File, FileOptions);
			Options.Add(ChecksumOptionType.Text, TextOptions);
			Options.Add(ChecksumOptionType.Help, HelpOptions);
			Options.Add(ChecksumOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(ChecksumOptions.HelpOptions);
			singleOptionList.AddRange(ChecksumOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Checksum v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	checksum - checksum and count the bytes in a file

SYNOPSIS

	checksum [OPTION] [FILE]

DESCRIPTION

	Print checksum and byte counts of each FILE.

OPTIONS

	-a, --algorithm=PATTERN
	{0}{0}The checksum algorithm.
	-f, --file
	{0}{0}Specify a file.
	-t, --text
	{0}{0}Specify text, default UTF8 encoding.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	checksum -a crc32 -f a.txt
	In the above command the system would checksum 
	the file 'a.txt'.

	checksum -a crc32 -t '123456789'
	In the above command the system would checksum 
	the text '123456789'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static ChecksumOptionType GetOptionType(string option)
		{
			ChecksumOptionType optionType = ChecksumOptionType.None;

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
