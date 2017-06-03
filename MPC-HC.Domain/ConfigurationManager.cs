using System;

namespace MPC_HC.Domain
{
    public class ConfigurationManager

    {
        private static ConfigurationManager _instance;

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception($"You need to call {nameof(Init)} before accessing the instace");
                }
                return _instance;
            }
        }

        public Uri BaseUri { get; }
        public string PathToMPCHC { get; }

        private ConfigurationManager(string baseUri, string pathToMpchc)
        {
            BaseUri = new Uri(baseUri);
            PathToMPCHC = pathToMpchc;
        }

        public void Init(string baseUri, string pathToMpchc)
        {
            _instance = new ConfigurationManager(baseUri, pathToMpchc);
        }
    }
}