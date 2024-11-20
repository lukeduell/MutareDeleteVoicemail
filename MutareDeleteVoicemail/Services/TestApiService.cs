using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MutareDeleteVoicemail.Utilities;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using MutareDeleteVoicemail.Processors;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MutareDeleteVoicemail.Services
{
    public class TestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public TestApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("BlackBoxApi:BaseUrl");
        }

        // Example method to call an API endpoint
        public async Task<string> GetSomeDataAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}endpoint");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
