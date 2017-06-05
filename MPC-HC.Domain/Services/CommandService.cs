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
        private const int Margin = 5 * 1000;

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

            var info = await GetResult(Command.Position, GetKeyValuePair("position", timeSpan.ToString()));
            return new Result
            {
                Info = info,
                ResultCode = IsInRange(info.PositionMillisec - timeSpan.TotalMilliseconds, -Margin, Margin) ? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> SetSoundLevel(int soundLevel)
        {
            if (soundLevel < 0 || soundLevel > 100) throw new ArgumentOutOfRangeException(nameof(soundLevel), "Must be betwine 0 and 100");

            var info = await GetResult(Command.Volume, GetKeyValuePair("volume", soundLevel.ToString()));
            return new Result
            {
                Info = info,
                ResultCode = info.VolumeLevel == soundLevel ? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> Play()
        {
            var info = await GetResult(Command.Play, null);
            return new Result
            {
                Info = info,
                ResultCode = info.State == State.Playing ? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> Pause()
        {
            var info = await GetResult(Command.Pause, null);
            return new Result
            {
                Info = info,
                ResultCode = info.State == State.Paused ? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> Stop()
        {
            var info = await GetResult(Command.Stop, null);
            return new Result
            {
                Info = info,
                ResultCode = info.State == State.Stoped ? ResultCode.Ok : ResultCode.Fail
            };
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


            var info = await GetResult(Command.ToggleMute, null);
            return new Result
            {
                Info = info,
                ResultCode = !info.Muted? ResultCode.Ok : ResultCode.Fail
            };
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


            var info = await GetResult(Command.ToggleMute, null);
            return new Result
            {
                Info = info,
                ResultCode = info.Muted? ResultCode.Ok : ResultCode.Fail
            };
        }

        public async Task<Result> ToggleMute()
        {
            var info = await GetResult(Command.ToggleMute, null);
            return new Result
            {
                Info = info,
                ResultCode = ResultCode.Ok
            };
        }
        
        public async Task<Result> Next()
        {
            var info = await GetResult(Command.Next, null);
            return new Result
            {
                Info = info,
                ResultCode = ResultCode.Ok
            };
        }
        
        public async Task<Result> Prev()
        {
            var info = await GetResult(Command.Prev, null);
            return new Result
            {
                Info = info,
                ResultCode = ResultCode.Ok
            };
        }
        

        public async Task<Info> GetInfo()
        {
            var response = await _requestService.ExcuteGetRequest("/variables.html");
            return HtmlParserHelper.ParseHtmlToInfo(response);
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