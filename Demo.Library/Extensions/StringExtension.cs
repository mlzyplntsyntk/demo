using System.Text.RegularExpressions;

namespace Demo.Library.Extensions
{
    /**
     * Source : http://predicatet.blogspot.com.tr/2009/04/improved-c-slug-generator-or-how-to.html
     * 
     * We needed a String to Slug converter for our Articles' Friendly URL's.
     * Defined as a String Extension for easy usage.
     */
    public static class StringExtension
    {
        public static string ToSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "_"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
