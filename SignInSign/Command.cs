using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SignInSign
{
    internal class Command
    {
        internal static void SetupCmd(CommandArgs args)
        {
            //Obtain the player's current coordinates
            int x = args.Player.TileX;
            int y = args.Player.TileY;

            const string Message = $"[SignInSign]Please enter the following command: \n Reset SignInSign: [c/42B2CE:/gs r] \nSet transfer point: [c/F25E61:/gs s]";

            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage(Message, Microsoft.Xna.Framework.Color.YellowGreen);
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "r":
                case "reload":
                    ReloadCmd(args);
                    return;
                case "s":
                case "set":
                    if (args.Parameters.Count != 1)
                    {
                        args.Player.SendMessage("[SignInSign]The set transfer point command does not require additional parameters \nand will use your current position.", Microsoft.Xna.Framework.Color.Yellow);
                    }
                    else
                    {
                        SignInSign.Config.Teleport_X = x;
                        SignInSign.Config.Teleport_Y = y;
                        args.Player.SendMessage($"Your location has been set to[c/9487D6:SignInSign to Teleport point]£¬Teleport point£º({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);
                        Console.WriteLine($"¡¾SignInSign¡¿Teleport point£º({x}, {y})", Microsoft.Xna.Framework.Color.Yellow);

                        //Ensure that configuration changes are saved
                        SignInSign.Config.Write(Configuration.ConfigPath);
                    }
                    break;
                default:
                    args.Player.SendMessage(Message, Microsoft.Xna.Framework.Color.YellowGreen);
                    return;
            }
        }

        private static void ReloadCmd(CommandArgs args)
        {
            if (args.Player == null || args == null) { return; }
            //Clear the original Tile
            WorldGen.KillTile(Main.spawnTileX, Main.spawnTileY - 3);

            //Set up walls and Tile
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


            Main.sign[newSignID].text = SignInSign.Config.SignText;
            Main.sign[newSignID].x = Main.spawnTileX;
            Main.sign[newSignID].y = Main.spawnTileY - 3;

            //Reload configuration file to save the world and send Spawn coordinates
            TShockAPI.Commands.HandleCommand(TSPlayer.Server, "/save");
            TSPlayer.All.SendTileRect((short)Main.spawnTileX, (short)(Main.spawnTileY - 3), 2, 2);
            TShockAPI.Commands.HandleCommand(args.Player, "/reload");
        }
    }

}