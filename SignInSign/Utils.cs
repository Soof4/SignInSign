using Terraria;
using TShockAPI;

namespace SignInSign
{
    public static class Utils
    {
        public static int GetSignIdByPos(int x, int y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.sign[i] != null && Main.sign[i].x == x && Main.sign[i].y == y)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string ReadPassword(string text) {
            return text.Substring(SignInSign.Config.SignText.Length).Trim();
        }
    }
}