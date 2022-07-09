public class TextTraits {
    public bool IsPalindrome {get;}
    public int CharCount {get;}
    public string Organized {get;}

    public TextTraits(string text) {
        IsPalindrome = Utils.IsPalindrome(text);
        CharCount = Utils.CharCount(text);
        Organized = Utils.Organize(text);
    }
}