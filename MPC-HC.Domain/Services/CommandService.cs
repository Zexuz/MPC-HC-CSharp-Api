using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPC_HC.Domain.Helpers;
using MPC_HC.Domain.Interfaces;

namespace MPC_HC.Domain.Services
{
    internal class CommandService
    {
        private readonly IRequestService _requestService;
        private const    int             Margin = 5 * 1000;

        public CommandService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public async Task<Result> OpenFile(string path)
        {
            var encodedPath = Uri.EscapeDataString(path);
            await _requestService.ExcuteGetRequest($"/browser.html?path={encodedPath}");
            var info = await GetInfo();
            return new Result
            {
                Info = info,
                ResultCode = info.FilePath == path ? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> SetPosition(TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(timeSpan), "Can not be negative timespan");

            return await Execute(Command.Volume,info => IsInRange(info.PositionMillisec - timeSpan.TotalMilliseconds, -Margin, Margin),  GetKeyValuePair("position", timeSpan.ToString()));
        }

        public async Task<Result> SetSoundLevel(int soundLevel)
        {
            if (soundLevel < 0 || soundLevel > 100) throw new ArgumentOutOfRangeException(nameof(soundLevel), "Must be betwine 0 and 100");

            return await Execute(Command.Volume,info => info.VolumeLevel == soundLevel, GetKeyValuePair("volume", soundLevel.ToString()));
        }

        public async Task<Result> Play()
        {
            return await Execute(Command.Play, info => info.State == State.Playing);
        }

        public async Task<Result> ToogleFullscreen()
        {
            return await Execute(Command.ToggleFullscreen, info => info.State == State.Playing);
        }

        public async Task<Result> Pause()
        {
            return await Execute(Command.Pause, info => info.State == State.Paused);
        }

        public async Task<Result> Stop()
        {
            return await Execute(Command.Pause, info => info.State == State.Stoped);
        }


        public async Task<Result> UnMute()
        {
            var firstInfo = await GetInfo();
            if (!firstInfo.Muted)
                return new Result
                {
                    Info = firstInfo,
                    ResultCode = ResultCode.Ok
                };

            return await Execute(Command.ToggleMute, info => !info.Muted);
        }

        public async Task<Result> Mute()
        {
            var firstInfo = await GetInfo();
            if (firstInfo.Muted)
                return new Result
                {
                    Info = firstInfo,
                    ResultCode = ResultCode.Ok
                };


            return await Execute(Command.ToggleMute, info => info.Muted);
        }

        public async Task<Result> ToggleMute()
        {
            return await Execute(Command.ToggleMute, info => info.Muted);
        }

        public async Task<Result> Next()
        {
            return await Execute(Command.Next, info => true);
        }

        public async Task<Result> Prev()
        {
            return await Execute(Command.Prev, info => true);
        }


        public async Task<Info> GetInfo()
        {
            var response = await _requestService.ExcuteGetRequest("/variables.html");
            return HtmlParserHelper.ParseHtmlToInfo(response);
        }
        
        private async Task<Result> Execute(Command command,Func<Info,bool> validate, KeyValuePair<string, string>? secondArg = null)
        {
            var info = await GetResult(command, null);
            return new Result
            {
                Info = info,
                ResultCode = validate(info)? ResultCode.Ok : ResultCode.Fail
            };
        }


        private bool IsInRange(double numberToCheck, int bottom, int top)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        private static KeyValuePair<string, string> GetKeyValuePair(string name, string value)
        {
            return new KeyValuePair<string, string>(name, value);
        }

        private async Task<Info> GetResult(Command command, KeyValuePair<string, string>? secondArg)
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                GetKeyValuePair("wm_command", ((int) command).ToString())
            };

            if (secondArg.HasValue) keyValuePairs.Add(secondArg.Value);

            await _requestService.ExcutePostRequest("/command.html", keyValuePairs);

            return await GetInfo();
        }
    }
}