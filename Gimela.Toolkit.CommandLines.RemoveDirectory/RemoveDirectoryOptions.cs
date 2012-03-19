using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.RemoveDirectory
{
	internal static class RemoveDirectoryOptions
	{
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<RemoveDirectoryOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RemoveDirectoryOptions()
		{
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "D", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "R", "recursive" });
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<RemoveDirectoryOptionType, ICollection<string>>();
			Options.Add(RemoveDirectoryOptionType.Directory, DirectoryOptions);
			Options.Add(RemoveDirectoryOptionType.Recursive, RecursiveOptions);
			Options.Add(RemoveDirectoryOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(RemoveDirectoryOptionType.Help, HelpOptions);
			Options.Add(RemoveDirectoryOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(RemoveDirectoryOptions.RecursiveOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.HelpOptions);
			singleOptionList.AddRange(RemoveDirectoryOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"RemoveDirectory v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	removedir - remove directories 

SYNOPSIS

	removedir [OPTION]... DIRECTORY...

DESCRIPTION

	Remove the DIRECTORY(ies).

OPTIONS

	-d, -D, --directory=DIRECTORY
	{0}{0}Specify a directory, a path name of a starting point 
	{0}{0}in the directory hierarchy.
	-r, -R, --recursive
	{0}{0}Remove the contents of directories recursively.
	-e, --regex=PATTERN
	{0}{0}Directory name matches regular expression pattern. 
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	removedir -d . -r -e object
	Remove all the 'object' directories in the current directory
	and any subdirectory.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static RemoveDirectoryOptionType GetOptionType(string option)
		{
			RemoveDirectoryOptionType optionType = RemoveDirectoryOptionType.None;

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
