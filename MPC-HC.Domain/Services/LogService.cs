using System;
using MPC_HC.Domain.Interfaces;

namespace MPC_HC.Domain.Services
{
    public class LogService:ILogService
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}