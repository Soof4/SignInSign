using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace SignInSign
{
    public class Configuration
    {
        public static string ConfigPath = Path.Combine(TShock.SavePath, "SignInSign.json");

        [JsonProperty("Is the registration and login function enabled", Order = -3)]
        public bool SignEnable = true;
        [JsonProperty("Record role passwords", Order = -3)]
        public bool PassInfo = false;

        [JsonProperty("Display billboards for logged in players", Order =-2)]
        public bool SignEnable1 = true;

        [JsonProperty("Is it allowed to click Sign", Order = -2)]
        public bool SignEnable2 = true;
        [JsonProperty("Click Sign to confirm whether to send a broadcast", Order = -2)]
        public bool SignEnable3 = false;

        //Maybe you should consider the feelings of mobile players,Use the"  :  "symbol to guide them later on
        [JsonProperty("Create content for Sign and reset instructions:/gs r")]
        public string SignText = "Welcome to my server. \nThis server supports chain mining, more crystal towers can be placed and used. NPCs sell more items, and more materials can be converted into low light. There are also RPG professional stores and magic modified bosses waiting for you to experience! \nPlease type in your password below the line：\n";

        [JsonProperty("Click on Sign's broadcast")]
        public string SignText2 = "Enter 2 times in sequence in this Sign：\n[c/F7CCF0:123456]  Register and log in";
        [JsonProperty("Attempting to disrupt Sign's broadcast")]
        public string SignText3 = "This Sign cannot be modified!";
        [JsonProperty("What command to execute by clicking Sign")]
        public string[] CommandsOnSignRead { get; set; } = new string[0];
        [JsonProperty("What BUFF is given by clicking Sign")]
        public int[] BuffID { get; set; } = new int[] {};
        [JsonProperty("Click on SignBUFF duration/minute")]
        public int BuffTime { get; set; } = 10;
        [JsonProperty("What item to give by clicking Sign")]
        public int[] ItemID { get; set; } = new int[] {};
        [JsonProperty("Click Sign to give the quantity of items")]
        public int ItemStack { get; set; } = 1;

        [JsonProperty("Click on Sign to confirm transmission and set instructions:/gs s")]
        public bool Teleport { get; set; } = false;
        [JsonProperty("Click Sign to Teleport_X")]
        public float Teleport_X { get; set; } = 0;
        [JsonProperty("Click Sign to Teleport_Y")]
        public float Teleport_Y { get; set; } = 0;
        [JsonProperty("Click on Sign to style")]
        public byte Style { get; set; } = 10;

        public static Configuration Reload()
        {
            Configuration? c = null;

            if (File.Exists(ConfigPath))
            {
                c = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath));
            }

            if (c == null)
            {
                c = new Configuration();
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(c, Formatting.Indented));
            }

            return c;
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        #region Method for reading configuration files
        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
            {
                var c = new Configuration();
                c.Write(path);
                return c;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf!;
                }
            }
        }
        #endregion
    }
}