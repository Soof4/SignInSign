using System.IO.Streams;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SignInSign;
[ApiVersion(2, 1)]
public class SignInSign : TerrariaPlugin
{
    #region Plugin Info

    public override string Name => "SignInSign";
    public override string Description => "Sign in sign!";
    public override string Author => "Soofa 羽学 少司命";
    public override Version Version => new(1, 0, 3);

    #endregion

    #region Variable
    public static Configuration Config = Configuration.Reload();
    private static int SignID = -1;
    #endregion

    #region Register and uninstall hooks
    public SignInSign(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command("signinsign.setup", Command.SetupCmd, "setupsign", "gs"));
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
        GetDataHandlers.TileEdit.Register(OnEdit);
        GetDataHandlers.Sign.Register(OnSignChange);
        GetDataHandlers.SignRead.Register(OnSignRead);
        GeneralHooks.ReloadEvent += LoadConfig;
        Config = Configuration.Reload();
    }

    public static void DisposeHandlers(TerrariaPlugin deregistrator)
    {
        ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);
        ServerApi.Hooks.GamePostInitialize.Deregister(deregistrator, OnGamePostInitialize);
        GetDataHandlers.TileEdit.UnRegister(OnEdit);
        GetDataHandlers.Sign.UnRegister(OnSignChange);
        GetDataHandlers.SignRead.UnRegister(OnSignRead);
        GeneralHooks.ReloadEvent -= LoadConfig;
    }
    #endregion

    #region Method for creating and reloading configuration files
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        Config = Configuration.Read(Configuration.ConfigPath);
        Config.Write(Configuration.ConfigPath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage("[SignInSign]Reload configuration completed.");
        }
    }
    #endregion

    #region  After game initialization
    private static void OnGamePostInitialize(EventArgs args)
    {
        if (args == null) return;
        SignID = Utils.GetSignIdByPos(Main.spawnTileX, Main.spawnTileY - 3);
        if (SignID == -1)
            SignID = Utils.SpawnSign(Main.spawnTileX, Main.spawnTileY - 3);
    }
    #endregion

    #region Player Join Event
    public static void OnNetGreetPlayer(GreetPlayerEventArgs args)
    {
        TSPlayer player = TShock.Players[args.Who];

        if (args == null || TShock.Players[args.Who] == null || SignID < 0 || SignID >= 999)
            return;

        if (!player.IsLoggedIn || Config.SignEnable1 == player.IsLoggedIn)
            player.SendData(PacketTypes.SignNew, "", SignID, args.Who);
    }
    #endregion

    #region Prevent unauthorized individuals from damaging billboards
    private static void OnEdit(object? sender, GetDataHandlers.TileEditEventArgs e)
    {
        if (e == null || e.Player.HasPermission("sign.edit")) { return; }
        if (Main.tile[e.X, e.Y].type == 55 &&
            Math.Abs(e.X - Main.spawnTileX) < 10 &&
            Math.Abs(e.Y - Main.spawnTileY) < 10 &&
            e.Player.Active)
        {
            e.Player.SendTileSquareCentered(e.X, e.Y, 4);
            e.Player.SendMessage($"{Config.SignText3}", color: Microsoft.Xna.Framework.Color.Yellow);
            e.Handled = true;
        }
    }
    #endregion

    #region Registration and login related
    public static void OnSignChange(object? sender, GetDataHandlers.SignEventArgs args)
    {
        args.Data.Seek(0, SeekOrigin.Begin);
        int signId = args.Data.ReadInt16();
        int posX = args.Data.ReadInt16();
        int posY = args.Data.ReadInt16();
        string newText = args.Data.ReadString();

        if (args.Player == null
            || args == null
            || signId != SignID
            || signId < 0
            || signId >= Main.sign.Length
            || Main.sign[signId] == null
            || Config.SignEnable == false)
            return;

        #region Help players execute login instructions and record passwords
        string password = Utils.ReadPassword(newText);
        if (TShock.UserAccounts.GetUserAccountByName(args.Player.Name) == null)
        {
            TShockAPI.Commands.HandleCommand(args.Player, $"/register {password}");
        }
        else
        {
            TShockAPI.Commands.HandleCommand(args.Player, $"/login {password}");
        }

        if (Config.PassInfo == true)
        {
            TShock.Log.ConsoleInfo($"Player【{args.Player.Name}】The Password for：{password}");//Write a copy to Tshock's own Logs file
            string MiMaPath = Path.Combine(TShock.SavePath, "SignIn_PlayerPassword", ""); //Path for writing logs
            Directory.CreateDirectory(MiMaPath); // Create a log folder
            string FileName = $"[SignInSign] {DateTime.Now.ToString("yyyy-MM-dd")}.txt"; //Add a date to the log name
            File.AppendAllLines(Path.Combine(MiMaPath, FileName), new string[] { DateTime.Now.ToString("u") + $" Player【{args.Player.Name}】The Password for：{password}" }); //Write log
        }
        #endregion

        //Check the area protection, it has no effect yet
        IEnumerable<TShockAPI.DB.Region> region = TShock.Regions.InAreaRegion(posX, posY);
        if (region.Any() && !region.First().Owner.Equals(args.Player.Name)) { return; }

        Main.sign[signId].text = SignInSign.Config.SignText;
        TSPlayer.All.SendData(PacketTypes.SignNew, number: signId);
        args.Handled = true;
    }
    #endregion

    #region  The method for players to trigger by actively clicking on the billboard
    public static void OnSignRead(object? sender, GetDataHandlers.SignReadEventArgs args)
    {
        //When the permission to click on the billboard is false, return without any processing
        if (args.Player == null || Config.SignEnable2 == false) { args.Handled = true; }

        else
        {
            if (Config.SignEnable3 == true)
            {
                args.Player!.SendMessage($"{Config.SignText2}", color: Microsoft.Xna.Framework.Color.Yellow);
            }

            // Read the CommandsOnSignRead list from the configuration and execute each command sequentially
            foreach (var command in Config.CommandsOnSignRead)
            {
                // Execute the command, use TSPlayer here Server executing commands means being executed by the server
                Commands.HandleCommand(TSPlayer.Server, command);
            }

            //Traverse the BUFFID in the configuration file and click to set BUFF
            foreach (var BuffID in Config.BuffID)
            {
                args.Player.SetBuff(BuffID, Config.BuffTime * 3600, false);
            }

            //Traverse the item ID in the configuration file and click to give the item
            foreach (var ItemID in Config.ItemID)
            {
                args.Player.GiveItem(ItemID, Config.ItemStack, 0);
            }

            //When clicking on the billboard to confirm whether the teleportation is true, the player will be teleported to the specified coordinates (only valid for logged in players
            if (Config.Teleport == true || args.Player.IsLoggedIn)
            {
                if (Config.Teleport_X  <= 0 || Config.Teleport_Y <= 0)
                {
                    args.Player!.SendMessage($"[SignInSign] Please use [c/F25E61:/gs s] to set the transfer coordinates, \nthe current coordinates are:{Config.Teleport_X},{Config.Teleport_Y}", color: Microsoft.Xna.Framework.Color.Yellow);
                }
                else { args.Player.Teleport(x: Config.Teleport_X * 16, y: Config.Teleport_Y * 16, style: Config.Style); }
            }
        }
    }
    #endregion
}
