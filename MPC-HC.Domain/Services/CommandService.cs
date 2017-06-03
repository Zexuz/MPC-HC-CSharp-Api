using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPC_HC.Domain.Helpers;
using MPC_HC.Domain.Interfaces;

namespace MPC_HC.Domain.Services
{
    public class CommandService
    {
        private readonly IRequestService _requestService;

        public CommandService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<string> OpenFile(string path)
        {
            var encodedPath = Uri.EscapeDataString(path);
            return await _requestService.ExcuteGetRequest($"/browser.html?path={encodedPath}");
        }

        public async Task<string> SetPosition(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(timeSpan), "Can not be negative timespan");
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = new[]
            {
                GetKeyValuePair("wm_command", ((int)Command.Position).ToString()),
                GetKeyValuePair("position", timeSpan.ToString("hh:mm:ss"))
            };
            return  await _requestService.ExcutePostRequest("/command.html", keyValuePairs);
        }

        public async Task<string> SetSoundLevel(int soundLevel)
        {
            if (soundLevel < 0 || soundLevel > 100) throw new ArgumentOutOfRangeException(nameof(soundLevel), "Must be betwine 0 and 100");
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = new[]
            {
                GetKeyValuePair("wm_command", ((int)Command.Volume).ToString()),
                GetKeyValuePair("volume", soundLevel.ToString())
            };
            return await _requestService.ExcutePostRequest("command.html", keyValuePairs);
        }

        public async Task<int> GetSoundLevel()
        {
            var result = await GetInfo();
            return result.VolumeLevel;
        }

        public async Task<string> Play()
        {
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = new[]
            {
                GetKeyValuePair("wm_command", ((int)Command.Play).ToString())
            };

            return await _requestService.ExcutePostRequest("/command.html", keyValuePairs);
        }

        public async Task<string> Pause()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                GetKeyValuePair("wm_command", ((int)Command.Pause).ToString())
            };

            return await _requestService.ExcutePostRequest("/command.html", keyValuePairs);
        }

        public async Task<Info> GetInfo()
        {
            var response = await _requestService.ExcuteGetRequest("/variables.html");
            return HtmlParserHelper.ParseHtmlToInfo(response);
        }


        private static KeyValuePair<string, string> GetKeyValuePair(string name, string value)
        {
            return new KeyValuePair<string, string>(name, value);
        }
    }
}