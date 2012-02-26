using System.Collections.Generic;

namespace Gimela.Toolkit.CommandLines.Grep
{
  internal class GrepCommandLineOptions
  {
    public GrepCommandLineOptions()
    {
      this.FilePaths = new List<string>();
    }

    public bool IsSetRegexPattern { get; set; }
    public string RegexPattern { get; set; }

    public bool IsSetPath { get; set; }
    public ICollection<string> FilePaths { get; set; }

    public bool IsSetFixedStrings { get; set; }
    public bool IsSetIgnoreCase { get; set; }
    public bool IsSetInvertMatch { get; set; }
    public bool IsSetCount { get; set; }
    public bool IsSetFilesWithoutMatch { get; set; }
    public bool IsSetFilesWithMatchs { get; set; }
    public bool IsSetNoMessages { get; set; }
    public bool IsSetWithFileName { get; set; }
    public bool IsSetNoFileName { get; set; }
    public bool IsSetLineNumber { get; set; }
    public bool IsSetDirectory { get; set; }
    public bool IsSetRecursive { get; set; }

    public bool IsSetExcludeFiles { get; set; }
    public string ExcludeFilesPattern { get; set; }

    public bool IsSetExcludeDirectories { get; set; }
    public string ExcludeDirectoriesPattern { get; set; }

    public bool IsSetIncludeFiles { get; set; }
    public string IncludeFilesPattern { get; set; }

    public bool IsSetHelp { get; set; }
    public bool IsSetVersion { get; set; }
  }
}
