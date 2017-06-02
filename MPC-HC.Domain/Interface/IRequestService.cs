using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPC_HC.Domain.Interfaces
{
    public interface IRequestService
    {
        Task<string> ExcutePostRequest(string path, IEnumerable<KeyValuePair<string, string>> payLoad);
        Task<string> ExcuteGetRequest(string path);
    }
}