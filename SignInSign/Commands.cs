using TShockAPI;
using Terraria;
using Terraria.ID;

namespace SignInSign
{
    public class Commands
    {
        public static void InitializeCommands()
        {
            TShockAPI.Commands.ChatCommands.Add(new Command("signinsign.setup", SetupCmd, "setupsign"));
        }

        private static void SetupCmd(CommandArgs args)
        {
            // Set walls and tiles
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

            // Find an empty sign ID
            int newSignID = -1;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.sign[i] == null || Main.sign[i].text == "")
                {
                    Main.sign[i] = new Sign();
                    newSignID = i;
                    break;
                }
            }

            if (newSignID == -1) newSignID = 999;

            // Put down sign info
            Main.sign[newSignID].text = SignInSign.Config.SignText;
            Main.sign[newSignID].x = Main.spawnTileX;
            Main.sign[newSignID].y = Main.spawnTileY - 3;

            // Save the world and send tile rectangle
            TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");
            TSPlayer.All.SendTileRect((short)Main.spawnTileX, (short)(Main.spawnTileY - 3), 2, 2);
        }
    }
}