using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MPC_HC.Domain;
using MPC_HC.Domain.Helpers;
using MPC_HC.Domain.Interfaces;
using MPC_HC.Domain.Services;
using Xunit;


namespace MPC_HC.Test
{
    public class CommandServiceIntergrationTest : IDisposable
    {
        private Process _mediaProcess;
        private readonly IRequestService _requestService;
        private Info _info;

        private readonly MediaPlayerConfig _mediaPlayerConfig;

        public CommandServiceIntergrationTest()
        {
            _mediaPlayerConfig = new MediaPlayerConfig("http://localhost:13579",@"D:\Program Files (x86)\MPC-HC\mpc-hc.exe");
            _requestService = new RequestService(new HttpClient(), "http://localhost:13579", new LogService());
            AsyncHelpers.RunSync(InitMediaPlayer);
        }

        private async Task<string> InitMediaPlayer()
        {
            var commandService = new CommandService(_requestService);

            _mediaProcess = Process.Start(_mediaPlayerConfig.PathToMediaPlayerExecutable);
            await Task.Delay(1000);
            _info = await commandService.GetInfo();
            return await commandService.OpenFile(
                "D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01E01.Tourist.Trapped.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv");
        }

        [Fact]
        public void SetSoundThrowsArgumentOutOfRangeException()
        {
            var commandService = new CommandService(_requestService);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-1));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-10));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(-54));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(1000));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(120));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await commandService.SetSoundLevel(101));
        }

        [Fact]
        public async void SetSoundSuccess()
        {
            var expectedSoundLevel = 10;
            var commandService = new CommandService(_requestService);
            if (_info.VolumeLevel == expectedSoundLevel)
                expectedSoundLevel = 100;
            await commandService.SetSoundLevel(expectedSoundLevel);
            var soundLevel = await commandService.GetSoundLevel();
            Assert.Equal(expectedSoundLevel, soundLevel);
        }

        [Fact]
        public async void PauseAndPlay()
        {
            var commandService = new CommandService(_requestService);

            var info = await commandService.GetInfo();
            if (info.State == State.Playing)
                await commandService.Pause();

            var info2 = await commandService.GetInfo();
            Assert.True(info2.State == State.Paused);

            await commandService.Play();
            var info3 = await commandService.GetInfo();
            Assert.True(info3.State == State.Playing);
        }

        public void Dispose()
        {
            _mediaProcess.Kill();
        }
    }
}