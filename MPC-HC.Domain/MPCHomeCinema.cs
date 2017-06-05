using System;
using System.Net.Http;
using System.Threading.Tasks;
using MPC_HC.Domain.Services;

namespace MPC_HC.Domain
{
    public class MPCHomeCinema : IMPCHomeCinema
    {
        private readonly string _baseUrl;
        private readonly CommandService _commandService;

        public MPCHomeCinema(string baseUrl)
        {
            _baseUrl = baseUrl;
            var requestService = new RequestService(new HttpClient(), _baseUrl, new LogService());
            _commandService = new CommandService(requestService);
        }

        public async Task<Result> PauseAsync()
        {
            return await _commandService.Pause();
        }

        public async Task<Result> PlayAsync()
        {
            return await _commandService.Play();
        }

        public async Task<Result> StopAsync()
        {
            return await _commandService.Stop();
        }

        public async Task<Result> OpenFileAsync(string path)
        {
            return await _commandService.OpenFile(path);
        }

        public async Task<Result> UnMuteAsync()
        {
            return await _commandService.UnMute();
        }

        public async Task<Result> MuteAsync()
        {
            return await _commandService.Mute();
        }
        
        public async Task<Result> ToggleMuteAsync()
        {
            return await _commandService.ToggleMute();
        }
        
        //TODO figure out how to test this...
        public async Task<Result> NextAsync()
        {
            return await _commandService.Next();
        }
        
        //TODO figure out how to test this...
        public async Task<Result> PrevAsync()
        {
            return await _commandService.Prev();
        }

        public async Task<Result> SetVolumeLevel(int soundLevel)
        {
            return  await _commandService.SetSoundLevel(soundLevel);
        }

        public async Task<Result> SetPosition(TimeSpan position)
        {
            return await _commandService.SetPosition(position);
        }

        public async Task<Info> GetInfo()
        {
            return await _commandService.GetInfo();
        }
    }

    public interface IMPCHomeCinema
    {
        Task<Result> PauseAsync();
        Task<Result> PlayAsync();
        Task<Result> StopAsync();
        Task<Result> OpenFileAsync(string path);
        Task<Result> UnMuteAsync();
        Task<Result> MuteAsync();
        Task<Result> ToggleMuteAsync();
        Task<Result> NextAsync();
        Task<Result> PrevAsync();
        Task<Result> SetVolumeLevel(int soundLevel);
        Task<Result> SetPosition(TimeSpan position);
        Task<Info> GetInfo();

    }
}