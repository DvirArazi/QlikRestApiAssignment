static class Utils {
    public static bool IsPalindrome(string text) {
        for (int i = 0; i < text.Length / 2; i++) {
            if (text[i] != text[text.Length - 1 - i]) {
                return false;
            }
        }

        return true;
    }

    public static int CharCount(string text) {
        return text.Length;
    }

    public static string Organize(string text) {
        string rtn = "";

        for (char c = (char)0; c < 'Z'-'A'; c++) {
            void inner(char index) {
                for (int i = 0; i < text.Count((val)=>val==index); i++) {
                    rtn += index;
                }
            }

            inner((char)('A' + c));
            inner((char)('a' + c));
        }

        return rtn;
    }
}