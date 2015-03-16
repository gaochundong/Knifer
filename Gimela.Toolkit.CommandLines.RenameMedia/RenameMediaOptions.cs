﻿/*
 * [The "BSD Licence"]
 * Copyright (c) 2011-2015 Chundong Gao
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

namespace Gimela.Toolkit.CommandLines.RenameMedia
{
	internal static class RenameMediaOptions
	{
		public static readonly ReadOnlyCollection<string> RegexPatternOptions;
		public static readonly ReadOnlyCollection<string> InputDirectoryOptions;
		public static readonly ReadOnlyCollection<string> RecursiveOptions;
		public static readonly ReadOnlyCollection<string> PrefixOptions;
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<RenameMediaOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RenameMediaOptions()
		{
			RegexPatternOptions = new ReadOnlyCollection<string>(new string[] { "e", "regex" });
			InputDirectoryOptions = new ReadOnlyCollection<string>(new string[] { "d", "dir", "directory" });
			RecursiveOptions = new ReadOnlyCollection<string>(new string[] { "r", "recursive" });
			PrefixOptions = new ReadOnlyCollection<string>(new string[] { "p", "prefix" });
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<RenameMediaOptionType, ICollection<string>>();
			Options.Add(RenameMediaOptionType.RegexPattern, RegexPatternOptions);
			Options.Add(RenameMediaOptionType.InputDirectory, InputDirectoryOptions);
			Options.Add(RenameMediaOptionType.Recursive, RecursiveOptions);
			Options.Add(RenameMediaOptionType.Prefix, PrefixOptions);
			Options.Add(RenameMediaOptionType.Help, HelpOptions);
			Options.Add(RenameMediaOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(RenameMediaOptions.RecursiveOptions);
			singleOptionList.AddRange(RenameMediaOptions.HelpOptions);
			singleOptionList.AddRange(RenameMediaOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	renamem - rename image and video files

SYNOPSIS

	renamem [OPTION] [REGEX] [INPUT_DIRECTORY]

DESCRIPTION

	Renamem will rename the specified medias specified format.

OPTIONS

	-e PATTERN, --regex=PATTERN
	{0}{0}Use PATTERN as the file matched pattern.
	-d, --dir, --directory=DIRECTORY
	{0}{0}The input directory, read files in this directory.
	-r, --recursive
	{0}{0}Read all files under each directory recursively.
	-p PREFIX, --prefix=PREFIX
	{0}{0}Use PREFIX as the leading prefix of file name.
	-h, --help 
	{0}{0}Display this help and exit.
	-v, --version
	{0}{0}Output version information and exit.

EXAMPLES

	renamem.exe -e '*.jpg' -d 'C:\Media' -p 'image'
	Search all files in directory 'C:\Media', match the pattern '*.jpg',
	and rename the files with modified time such as 'image_20150316202020.jpg'.

	renamem.exe -e '*.mpg' -d 'C:\Media' -p 'video'
	Search all files in directory 'C:\Media', match the pattern '*.mpg',
	and rename the files with modified time such as 'video_20150316202020.mpg'.

AUTHOR

	Written by Chundong Gao.

REPORTING BUGS

	Report bugs to <gaochundong@gmail.com>.

COPYRIGHT

	Copyright (C) 2011-2015 Chundong Gao. All Rights Reserved.
", @" ");

		#endregion

		public static RenameMediaOptionType GetOptionType(string option)
		{
			RenameMediaOptionType optionType = RenameMediaOptionType.None;

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
