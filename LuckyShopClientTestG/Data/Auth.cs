using Newtonsoft.Json;
using System.IO;

namespace LuckyShopClientTestG.Model
{
    public class Auth
    {
        public string serverName { get; set; }
        public string port { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        // Parameterless constructor for deserialization
        public Auth()
        {
        }

        public Auth(string pathToJson)
        {
            string jsonString = File.ReadAllText(pathToJson);
            Set(JsonConvert.DeserializeObject<Auth>(jsonString));
        }

        private void Set(Auth auth)
        {
            this.serverName = auth.serverName;
            this.port = auth.port;
            this.databaseName = auth.databaseName;
            this.userName = auth.userName;
            this.password = auth.password;
        }


    }


}
