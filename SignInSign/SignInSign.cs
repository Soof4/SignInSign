using TShockAPI;
using Terraria;
using TerrariaApi.Server;

namespace SignInSign {
    [ApiVersion(2, 1)]
    public class SignInSign : TerrariaPlugin
    {
        #region Plugin Info
        
        public override string Name => "SignInSign";
        public override string Description => "Sign in sign!";
        public override string Author => "Soofa";
        public override Version Version => new Version(0, 0, 1);
        
        #endregion

        public static Configuration Config = Configuration.Reload();
        public SignInSign(Main game) : base(game) { }

        public override void Initialize()
        {
            Commands.InitializeCommands();
            Handlers.InitializeHandlers(this);

            Config = Configuration.Reload();
        }
    }
}
