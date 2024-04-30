using Newtonsoft.Json;
using TShockAPI;

namespace SignInSign
{
    public class Configuration
    {
        public static string ConfigPath = Path.Combine(TShock.SavePath, "SignInSignConfig.json");
        public string SignText = "Please type in your password below the line. \n------------------------------------------\n";
        public int SignID = -1;

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

        public void Write()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}