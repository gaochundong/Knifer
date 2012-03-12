﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Count
{
	internal static class CountOptions
	{
		public static readonly ReadOnlyCollection<string> DirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<CountOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static CountOptions()
		{
			DirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<CountOptionType, ICollection<string>>();
			Options.Add(CountOptionType.Directory, DirectoryOptions);
			Options.Add(CountOptionType.Recursive, RecursiveOptions);
			Options.Add(CountOptionType.Help, HelpOptions);
			Options.Add(CountOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(CountOptions.RecursiveOptions);
			singleOptionList.AddRange(CountOptions.HelpOptions);
			singleOptionList.AddRange(CountOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Count v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	count - count the number of various file type

SYNOPSIS

	count [OPTIONS] [FOLDER]

DESCRIPTION

	The count utility count the number of various file type 
	in a specified folder.

OPTIONS

	-d, --directory
	{0}{0}Specify a directory, read all files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2012 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static CountOptionType GetOptionType(string option)
		{
			CountOptionType optionType = CountOptionType.None;

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