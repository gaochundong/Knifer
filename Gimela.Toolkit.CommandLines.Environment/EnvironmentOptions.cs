using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gimela.Toolkit.CommandLines.Environment
{
	internal static class EnvironmentOptions
	{
		public static readonly ReadOnlyCollection<string> HelpOptions;
		public static readonly ReadOnlyCollection<string> VersionOptions;

		public static readonly IDictionary<EnvironmentOptionType, ICollection<string>> Options;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static EnvironmentOptions()
		{
			HelpOptions = new ReadOnlyCollection<string>(new string[] { "h", "help" });
			VersionOptions = new ReadOnlyCollection<string>(new string[] { "v", "version" });

			Options = new Dictionary<EnvironmentOptionType, ICollection<string>>();
			Options.Add(EnvironmentOptionType.Help, HelpOptions);
			Options.Add(EnvironmentOptionType.Version, VersionOptions);
		}

		public static List<string> GetSingleOptions()
		{
			List<string> singleOptionList = new List<string>();

			singleOptionList.AddRange(EnvironmentOptions.HelpOptions);
			singleOptionList.AddRange(EnvironmentOptions.VersionOptions);

			return singleOptionList;
		}

		#region Usage

		public const string Version = @"Environment v1.0";
		public static readonly string Usage = string.Format(CultureInfo.CurrentCulture, @"
NAME

	env - display your environment variables

SYNOPSIS

	env [OPTION]...

DESCRIPTION

	Displays your environment variables.

OPTIONS

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

		public static EnvironmentOptionType GetOptionType(string option)
		{
			EnvironmentOptionType optionType = EnvironmentOptionType.None;

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
