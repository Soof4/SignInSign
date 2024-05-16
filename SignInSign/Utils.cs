using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign
{
    public static class Utils
    {
        public static int GetSignIdByPos(int tileX, int tileY)
        {
            for (int i = 0; i < Main.sign.Length; i++)
            {
                if (Main.sign[i] != null && Main.sign[i].x == tileX && Main.sign[i].y == tileY)
                {
                    return i;
                }
            }
            Console.WriteLine($"SignInSign not found at position ({tileX}, {tileY})");
            return -1;
        }

        public static int SpawnSign(int x, int y)
        {
            WorldGen.KillTile(x, y);
            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].wall = WallID.EchoWall;
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].wall = WallID.EchoWall;

            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].active(false);
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].active(false);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].active(false);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].active(false);

            Main.tile[Main.spawnTileX, Main.spawnTileY - 3].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX, Main.spawnTileY - 2].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 3].UseBlockColors(new TileColorCache() { Invisible = true });
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY - 2].UseBlockColors(new TileColorCache() { Invisible = true });

            WorldGen.PlaceSign(Main.spawnTileX, Main.spawnTileY - 3, TileID.Signs, 4);
            WorldGen.PlaceObject(x, y, TileID.Signs, false, 4, -1, -1);

            int newSignID = 999;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.sign[i] == null || Main.sign[i].text == "")
                {
                    Main.sign[i] = new Sign();
                    newSignID = i;
                    break;
                }
            }
            Main.sign[newSignID].text = SignInSign.Config.SignText;
            Main.sign[newSignID].x = x;
            Main.sign[newSignID].y = y;
            TSPlayer.All.SendTileRect((short)x, (short)y);
            return newSignID;
        }

        public static string ReadPassword(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("The password entered cannot be empty.");
            }
            return text.Substring(SignInSign.Config.SignText.Length).Trim();
        }
    }
}