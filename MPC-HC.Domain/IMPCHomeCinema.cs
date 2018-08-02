using System;
using System.Threading.Tasks;

namespace MPC_HC.Domain
{
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
        Task<Info>   GetInfo();

    }
}