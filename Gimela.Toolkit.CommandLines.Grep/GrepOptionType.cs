
namespace Gimela.Toolkit.CommandLines.Grep
{
  internal enum GrepOptionType
  {
    None = 0,
    RegexPattern,
    File,
    FixedStrings,
    IgnoreCase,
    InvertMatch,
    Count,
    FilesWithoutMatch,
    FilesWithMatchs,
    NoMessages,
    WithFileName,
    NoFileName,
    LineNumber,
    Directory,
    ExcludeFiles,
    ExcludeDirectories,
    IncludeFiles,
    Recursive,
    Help,
    Version,
  }
}
