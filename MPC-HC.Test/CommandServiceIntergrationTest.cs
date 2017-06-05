using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MPC_HC.Domain;
using MPC_HC.Domain.Helpers;
using MPC_HC.Domain.Interfaces;
using Xunit;


namespace MPC_HC.Test
{
    public class CommandServiceIntergrationTest : IDisposable
    {
        private Process _mediaProcess;
        private readonly IRequestService _requestService;
        private Info _info;

        private readonly MediaPlayerConfig _mediaPlayerConfig;
        private MPCHomeCinema _mpcHomeCinema;

        public CommandServiceIntergrationTest()
        {
            _mediaPlayerConfig = new MediaPlayerConfig("http://localhost:13579",@"D:\Program Files (x86)\MPC-HC\mpc-hc.exe");
//            _requestService = new RequestService(new HttpClient(), "http://localhost:13579", new LogService());
            _mpcHomeCinema = new MPCHomeCinema("http://localhost:13579");
            var x = AsyncHelpers.RunSync(InitMediaPlayer);
            if(x.ResultCode == ResultCode.Fail) throw new Exception("Can't open media file.");
        }

        private async Task<Result> InitMediaPlayer()
        {

            _mediaProcess = Process.Start(_mediaPlayerConfig.PathToMediaPlayerExecutable);
            await Task.Delay(1000);
            return await _mpcHomeCinema.OpenFileAsync(
                "D:\\Downloads\\TorrentDay\\Downloads\\Gravity.Falls.S01-S02.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01.720p.WEB-DL.AAC2.0.H.264-iT00NZ\\Gravity.Falls.S01E01.Tourist.Trapped.720p.WEB-DL.AAC2.0.H264-Reaperza.mkv");
        }

        [Fact]
        public void SetSoundThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(-1));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(-10));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(-54));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(1000));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(120));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _mpcHomeCinema.SetVolumeLevel(101));
        }

        [Fact]
        public async void SetSoundSuccess()
        {
            //there is a 1% chance this test give a success if there's a fail.
            var expectedSoundLevel = new Random().Next(0, 100);
            var res = await _mpcHomeCinema.SetVolumeLevel(expectedSoundLevel);
            
            Assert.Equal(expectedSoundLevel, res.Info.VolumeLevel);
            Assert.Equal(ResultCode.Ok, res.ResultCode);
        }

        [Fact]
        public async void PauseAndPlay()
        {
            var pauseRes = await _mpcHomeCinema.PauseAsync();
            Assert.True(pauseRes.ResultCode == ResultCode.Ok);
            Assert.Equal(State.Paused,pauseRes.Info.State);
            
            var playRes = await _mpcHomeCinema.PlayAsync();
            Assert.True(playRes.ResultCode == ResultCode.Ok);
            Assert.Equal(State.Playing,playRes.Info.State);
        }
        
        [Fact]
        public async void Stop()
        {
            var res = await _mpcHomeCinema.StopAsync();
            Assert.True(res.ResultCode == ResultCode.Ok);
            Assert.Equal(State.Stoped,res.Info.State);
        }
        
        [Fact]
        public async void MuteAndUnMute()
        {
            var info = await _mpcHomeCinema.GetInfo();
            if (info.Muted)
            {
                var res1 = await _mpcHomeCinema.UnMuteAsync();
                Assert.True(res1.ResultCode == ResultCode.Ok);
                Assert.False(res1.Info.Muted);
                var res2 = await _mpcHomeCinema.MuteAsync();
                Assert.True(res2.ResultCode == ResultCode.Ok);
                Assert.True(res2.Info.Muted);
            }
            else
            {
                
                var res2 = await _mpcHomeCinema.MuteAsync();
                Assert.True(res2.ResultCode == ResultCode.Ok);
                Assert.True(res2.Info.Muted);
                
                var res1 = await _mpcHomeCinema.UnMuteAsync();
                Assert.True(res1.ResultCode == ResultCode.Ok);
                Assert.False(res1.Info.Muted);
            }
           
        }
        [Fact]
        public async void ToggleMute()
        {
            var info = await _mpcHomeCinema.GetInfo();
            var res = await _mpcHomeCinema.ToggleMuteAsync();
            Assert.NotEqual(info.Muted,res.Info.Muted);
            Assert.True(res.ResultCode == ResultCode.Ok);
        }
        

        [Fact]
        public async void SetPostition()
        {
            var res = await _mpcHomeCinema.SetPosition(new TimeSpan(0,11,23));
            Assert.True(res.ResultCode == ResultCode.Ok);
        }

        
        public void Dispose()
        {
            _mediaProcess.Kill();
        }
    }
}