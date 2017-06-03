using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MPC_HC.Domain.Interfaces;

namespace MPC_HC.Domain.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpClient _client;
        private readonly ILogService _logService;

        public RequestService(HttpClient client, Uri baseUrl,ILogService logService)
        {
            _client = client;
            _logService = logService;
            _client.BaseAddress = baseUrl;
        }

        public async Task<string> ExcutePostRequest(string path, IEnumerable<KeyValuePair<string, string>> payLoad)
        {
            _logService.Log($"sending post to {path}");
            using (var content = new FormUrlEncodedContent(payLoad))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var response = await _client.PostAsync(path, content);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> ExcuteGetRequest(string path)
        {
            _logService.Log($"sending GET to {path}");
            
            var response = await _client.GetAsync(path);
            return await response.Content.ReadAsStringAsync();
        }
    }
}