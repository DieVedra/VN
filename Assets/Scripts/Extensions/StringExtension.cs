using System.Linq;

public static class StringExtension
{
    public static string MyCutString(this string text, int skipFirstWordsCount = 2, int skipEndWordsCount = 0, params char[] separator)
    {
        if (text.Length > 0)
        {
            string[] words = text.Split(separator);
            words = words.Skip(skipFirstWordsCount).ToArray();
            words = words.SkipLast(skipEndWordsCount);
            return string.Join(" ", words);
        }
        else
        {
            return text;
        }
    }

    public static string[] SkipLast(this string[] texts, int skipEndWordsCount = 0)
    {
        if (texts.Length > 0 && skipEndWordsCount > 0)
        {
            int count = texts.Length - skipEndWordsCount;
            string[] words = new string[count];
            for (int i = 0; i < count; i++)
            {
                words[i] = texts[i];
            }
            return words;
        }
        else
        {
            return texts;
        }
    }
}