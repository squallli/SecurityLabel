using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Do.Lib.Extension
{    
    public static class StringExtension
    {
        public static string StripTags(this string src, string[] reservedTagPool)
        {
            return StringHelper.StripTags(src, reservedTagPool);
        }
        public static string StripTags(this string src)
        {
            return StringHelper.StripTags(src);
        }
        public static string TakePart(this string src, int length, string endStr = null)
        {
            return StringHelper.TakePart(src, length, endStr);
        }
    }

    public static class StringHelper
    {
        
        public static string StripTags(string src, string[] reservedTagPool)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                src,
                String.Format("<(?!{0}).*?>", string.Join("|", reservedTagPool)),
                String.Empty);
        }
        public static string StripTags(string src)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                src,
                "(?is)<.+?>", string.Empty);
        }
        public static string TakePart(string src, int length, string endStr = null)
        {
            if (src.Length <= length)
                return src;
            else
                return src.Substring(0, length) + endStr ?? string.Empty;
        }
    }
}
