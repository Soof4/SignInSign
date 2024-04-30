using System.IO.Streams;
using TerrariaApi.Server;
using TShockAPI;

namespace SignInSign
{
    public static class Handlers
    {
        public static void InitializeHandlers(TerrariaPlugin registrator)
        {
            ServerApi.Hooks.NetGreetPlayer.Register(registrator, OnNetGreetPlayer);

            GetDataHandlers.Sign += OnSignChange;
            GetDataHandlers.SignRead += OnSignRead;
        }

        public static void DisposeHandlers(TerrariaPlugin deregistrator)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);

            GetDataHandlers.Sign -= OnSignChange;
            GetDataHandlers.SignRead -= OnSignRead;
        }

        public static void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            if (TShock.Players[args.Who].IsLoggedIn) return;
            TShock.Players[args.Who].SendData(PacketTypes.SignNew, number: SignInSign.Config.SignID);
        }
        public static void OnSignChange(object? sender, GetDataHandlers.SignEventArgs args)
        {
            if (args.Handled) return;

            // Reading the data
            args.Data.Seek(0, SeekOrigin.Begin);
            int signId = args.Data.ReadInt16();
            int posX = args.Data.ReadInt16();
            int posY = args.Data.ReadInt16();
            string newText = args.Data.ReadString();

            if (signId != SignInSign.Config.SignID) return;

            string password = Utils.ReadPassword(newText);

            TShockAPI.Commands.HandleCommand(args.Player, $"/register {password}");

            TSPlayer.All.SendData(PacketTypes.SignNew, number: signId);;

            args.Handled = true;
        }

        public static void OnSignRead(object? sender, GetDataHandlers.SignReadEventArgs args)
        {
            // Read the data
            args.Data.Seek(0, SeekOrigin.Begin);
            int posX = args.Data.ReadInt16();
            int posY = args.Data.ReadInt16();
            int signID = Utils.GetSignIdByPos(posX, posY);


        }

    }

}