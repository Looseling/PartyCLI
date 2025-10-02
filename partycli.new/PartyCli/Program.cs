using Newtonsoft.Json;
using PartyCli.Domain.Interfaces;
using PartyCli.Infrastructure.Persistence;

namespace PartyCli
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ISettingsRepository _settings = new JsonSettingsRepository();

        async static Task Main(string[] args)
        {
            var currentState = States.none;
            string name = null;
            int argIndex = 1;

            foreach (string arg in args)
            {
                if (currentState == States.none)
                {
                    if (arg == "server_list")
                    {
                        currentState = States.server_list;
                        if (argIndex >= args.Count())
                        {
                            var serverList = getAllServersListAsync();
                            storeValue("serverlist", serverList, false);
                            log("Saved new server list: " + serverList);
                            displayList(serverList);
                        }
                    }
                    if (arg == "config")
                    {
                        currentState = States.config;
                    }
                }
                else if (currentState == States.config)
                {
                    if (name == null)
                    {
                        name = arg;
                    }
                    else
                    {
                        storeValue(proccessName(name), arg);
                        log("Changed " + proccessName(name) + " to " + arg);
                        name = null;
                    }
                }
                else if (currentState == States.server_list)
                {
                    if (arg == "--local")
                    {
                        var serverlist = await _settings.GetValueAsync("serverlist");
                        if (!String.IsNullOrEmpty(serverlist)) 
                        { 
                            displayList(serverlist);
                        } 
                        else
                        {
                            Console.WriteLine("Error: There are no server data in local storage");
                        }
                    }
                    else if (arg == "--france")
                    {
                        //france == 74
                        //albania == 2
                        //Argentina == 10
                        var query = new VpnServerQuery(null,74,null,null,null, null);
                        var serverList = getAllServerByCountryListAsync(query.CountryId.Value); //France id == 74
                        storeValue("serverlist", serverList, false);
                        log("Saved new server list: " + serverList);
                        displayList(serverList);
                    }
                    else if (arg == "--TCP")
                    {
                        //UDP = 3
                        //Tcp = 5
                        //Nordlynx = 35
                        var query = new VpnServerQuery(5,null,null,null,null, null);
                        var serverList = getAllServerByProtocolListAsync((int)query.Protocol.Value);
                        storeValue("serverlist", serverList, false);
                        log("Saved new server list: " + serverList);
                        displayList(serverList);
                    }
                }
                argIndex = argIndex + 1;
            }

            if(currentState == States.none)
            {
                Console.WriteLine("To get and save all servers, use command: partycli.exe server_list");
                Console.WriteLine("To get and save France servers, use command: partycli.exe server_list --france");
                Console.WriteLine("To get and save servers that support TCP protocol, use command: partycli.exe server_list --TCP");
                Console.WriteLine("To see saved list of servers, use command: partycli.exe server_list --local ");
            }
            Console.Read();
        }

        async static Task storeValue(string name, string value, bool writeToConsole = true)
        {
            try { 
                
                await _settings.SetValueAsync(name, value);
                if (writeToConsole) { 
                Console.WriteLine("Changed " + name + " to " + value);
                }
            }
            catch {
                Console.WriteLine("Error: Couldn't save " + name + ". Check if command was input correctly." );
            }
        }

        static string proccessName(string name)
        {
            name = name.Replace("-", string.Empty);
            return name;
        }

        static string getAllServersListAsync()
        {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers");
                var response = client.SendAsync(request).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                return responseString;
        }

        static string getAllServerByCountryListAsync(int countryId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=35&filters[country_id]=" + countryId);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        static string getAllServerByProtocolListAsync(int vpnProtocol)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=" + vpnProtocol);
            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return responseString;
        }

        static void displayList(string serverListString)
        {
            var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
            Console.WriteLine("Server list: ");
            for (var index = 0; index < serverlist.Count; index++)
            {
                Console.WriteLine("Name: " + serverlist[index].Name);
            }
            Console.WriteLine("Total servers: " + serverlist.Count);
        }

        async static Task log(string action)
        {
            var newLog = new LogModel
            {
                Action = action,
                Time = DateTime.Now
            };
            List<LogModel> currentLog;
            var existingLog = await _settings.GetValueAsync("log");
            if (!string.IsNullOrEmpty(existingLog))
            {
                currentLog = JsonConvert.DeserializeObject<List<LogModel>>(existingLog);
                currentLog.Add(newLog);
            }
            else
            {
                currentLog = new List<LogModel> { newLog };
            }

            storeValue("log", JsonConvert.SerializeObject(currentLog), false);
        }
    }

    internal class VpnServerQuery
    {
             public int? Protocol { get; set; }

            public int? CountryId { get; set;}

            public int? CityId { get; set;}

            public int? RegionId { get; set;}

            public int? SpecificServcerId { get; set;}

            public int? ServerGroupId { get; set;}

            public VpnServerQuery(int? protocol, int? countryId, int? cityId, int? regionId, int? specificServcerId, int? serverGroupId)
            {
                Protocol = protocol;
                CountryId = countryId;
                CityId = cityId;
                RegionId = regionId;
                SpecificServcerId = specificServcerId;
                ServerGroupId = serverGroupId;
            }
    }

    class LogModel
    {
        public string Action { get; set; }
        public DateTime Time { get; set; }
    }

    class ServerModel
    {
        public string Name { get; set; }
        public int Load { get; set; }
        public string Status { get; set; }

    }

    enum States
    {
        none = 0,
        server_list = 1,
        config = 2,
    };
}
