using System.IO.Streams;
using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using TShockAPI.Hooks;

namespace SignInSign
{
    public static class Handlers
    {
        public static void InitializeHandlers(TerrariaPlugin registrator)
        {
            ServerApi.Hooks.NetGreetPlayer.Register(registrator, OnNetGreetPlayer);
            ServerApi.Hooks.GamePostInitialize.Register(registrator, OnGamePostInitialize);

            GetDataHandlers.Sign += OnSignChange;

            GeneralHooks.ReloadEvent += OnReload;
        }

        public static void DisposeHandlers(TerrariaPlugin deregistrator)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);
            ServerApi.Hooks.GamePostInitialize.Register(deregistrator, OnGamePostInitialize);

            GetDataHandlers.Sign -= OnSignChange;

            GeneralHooks.ReloadEvent -= OnReload;
        }

        private static void OnReload(ReloadEventArgs e)
        {
            SignInSign.Config = Configuration.Reload();
        }

        private static void OnGamePostInitialize(EventArgs args)
        {
            SignInSign.SignID = Utils.GetSignIdByPos(Main.spawnTileX, Main.spawnTileY - 3);

            if (SignInSign.SignID == -1) Utils.SetupSign();
        }

        public static void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who].IsLoggedIn) return;

            TShock.Players[args.Who].SendData(PacketTypes.SignNew, number: SignInSign.SignID);
        }

        public static void OnSignChange(object? sender, GetDataHandlers.SignEventArgs args)
        {
            // Reading the data
            args.Data.Seek(0, SeekOrigin.Begin);
            int signId = args.Data.ReadInt16();
            int posX = args.Data.ReadInt16();
            int posY = args.Data.ReadInt16();
            string newText = args.Data.ReadString();

            if (signId != SignInSign.SignID) return;

            string password = Utils.ReadPassword(newText);

            if (TShock.UserAccounts.GetUserAccountByName(args.Player.Name) == null)
            {
                TShockAPI.Commands.HandleCommand(args.Player, $"/register {password}");
                TShockAPI.Commands.HandleCommand(args.Player, $"/login {password}");
            }
            else
            {
                TShockAPI.Commands.HandleCommand(args.Player, $"/login {password}");
            }

            Main.sign[signId].text = SignInSign.Config.SignText;
            TSPlayer.All.SendData(PacketTypes.SignNew, number: signId);

            args.Handled = true;
        }
    }
}