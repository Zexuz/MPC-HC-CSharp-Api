using System;
using MPC_HC.Domain.Interfaces;

namespace MPC_HC.Domain.Services
{
    public class LogService : ILogService
    {
        private readonly bool _isDebugMode;

        public LogService(bool isDebugMode)
        {
            _isDebugMode = isDebugMode;
        }

        public void Log(string message)
        {
            if (_isDebugMode)
                Console.WriteLine(message);
        }
    }
}