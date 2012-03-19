using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Remove
{
	internal static class RemoveOptions
	{
    public static readonly ReadOnlyCollection<string> DirectoryOptions;
    public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<RemoveOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RemoveOptions()
		{
      DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "D", "directory" });
      RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "R", "recursive" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<RemoveOptionType, ICollection<string>>();
      Options.Add(RemoveOptionType.Directory, DirectoryOptions);
      Options.Add(RemoveOptionType.Recursive, RecursiveOptions);
			Options.Add(RemoveOptionType.Help, HelpOptions);
			Options.Add(RemoveOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(RemoveOptions.HelpOptions);
			singleOptionList.AddRange(RemoveOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Remove v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	rm - remove files 

SYNOPSIS

	rm [OPTION]... FILE...

DESCRIPTION

	Remove the FILE(s).

OPTIONS

	-d, -D, --directory=DIRECTORY
	{0}{0}Specify a directory, a path name of a starting point 
	{0}{0}in the directory hierarchy.
	-r, -R, --recursive
	{0}{0}Remove the contents of directories recursively.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	rm myfile.txt
	Remove the file myfile.txt without prompting the user.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static RemoveOptionType GetOptionType(string option)
		{
			RemoveOptionType optionType = RemoveOptionType.None;

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
