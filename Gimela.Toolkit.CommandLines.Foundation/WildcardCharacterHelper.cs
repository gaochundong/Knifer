using System.Text.RegularExpressions;

namespace Gimela.Toolkit.CommandLines.Foundation
{
  public static class WildcardCharacterHelper
  {
    public static bool IsContainsWildcard(string path)
    {
      bool result = false;

      if (path.Contains(@"*"))
      {
        result = true;
      }
      else if (path.Contains(@"?"))
      {
        result = true;
      }

      return result;
    }

    public static string WildcardToRegex(string pattern)
    {
      return Regex.Escape(pattern).Replace(@"\*", @".*").Replace(@"\?", @".");
    }
  }
}
