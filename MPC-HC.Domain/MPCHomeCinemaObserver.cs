using System;
using System.Threading;
using System.Threading.Tasks;

namespace MPC_HC.Domain
{
    public class MPCHomeCinemaObserver
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan UpdateFrequency { get; set; }

        private Info           _oldInfo, _newInfo;
        private IMPCHomeCinema _mpchc;

        private bool   _isRunning;
        private Thread _thread;


        public MPCHomeCinemaObserver(IMPCHomeCinema client)
        {
            _mpchc = client;
            UpdateFrequency = TimeSpan.FromSeconds(1);
        }

        public async Task Start()
        {
            _oldInfo = await _mpchc.GetInfo();
            _thread = new Thread(Run);

            _isRunning = true;
            _thread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _thread.Join(TimeSpan.FromMilliseconds(500));
        }

        private async void Run()
        {
            while (_isRunning)
            {
                _newInfo = await _mpchc.GetInfo();

                if (_newInfo.State != _oldInfo.State)
                {
                    OnPropertyChanged(Property.State);
                }

                if (_newInfo.Position != _oldInfo.Position)
                {
                    OnPropertyChanged(Property.Possition);
                }

                if (_newInfo.FileName != _oldInfo.FileName)
                {
                    OnPropertyChanged(Property.File);
                }

                _oldInfo = _newInfo;
                await Task.Delay(UpdateFrequency);
            }
        }

        private void OnPropertyChanged(Property propertyType)
        {
            var args = new PropertyChangedEventArgs(_oldInfo, _newInfo, propertyType);
            PropertyChanged?.Invoke(this, args);
        }
    }
}