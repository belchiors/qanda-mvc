using System;
using System.Text.RegularExpressions;

namespace Qanda.Services;

public static class QuestionServices
{
    public static string GenerateUrl(string str)
    {
        var url = Regex.Replace(str.ToLower(), @"&\w+;", "");
        url = Regex.Replace(url, @"[^a-z0-9\-\s]", "");
        url = url.Replace(' ', '-');
        url = Regex.Replace(url, @"-{2,}", "-");
        url = url.TrimStart('-');
        url = url.TrimEnd('-');
        var words = url.Split('-');
        url = words.Length > 16 ? String.Join('-', words.Take(16)) : String.Join('-', words);
        return url;
    }
}