using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SingularToPlural
{
    public static class StringExtensions
    {
        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, Regex> _regexCache = new ConcurrentDictionary<string, Regex>();
        public static string Pluralize(this string word)
        {
            if (_cache.ContainsKey(word)) return _cache[word];

            string replaced;
            if (TryReplace(word, "s$", "s", out replaced)) return UpdateCache(word, replaced);
            if (TryReplace(word, "^(ax|test)is$", "$1es", out replaced)) return UpdateCache(word, replaced);
            if (TryReplace(word, "(octop|vir)i$", "$1i", out replaced)) return UpdateCache(word, replaced);

            if (string.Compare(replaced, word) == 0)
            {
                replaced = Regex.Replace(word, "$", "s");
                return UpdateCache(word, replaced);
            }
            return word;
        }

        private static string UpdateCache(string word, string replaced)
        {
            return _cache.GetOrAdd(word, replaced);
        }

        private static bool TryReplace(string word, string pattern, string replacement, out string replaced)
        {
            Regex rgx = _regexCache.GetOrAdd(pattern, p => new Regex(p, RegexOptions.IgnoreCase));
            replaced = rgx.Replace(word, replacement);
            return rgx.IsMatch(word);
        }
    }
}
