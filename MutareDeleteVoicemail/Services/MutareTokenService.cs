using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MutareDeleteVoicemail.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RockLib.Logging;


namespace MutareDeleteVoicemail.Services
{
    public class MutareTokenService : IMutareTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        private static HttpClient _client;
        private string _token;
        private static DateTime _expiration = new DateTime(1900, 1, 1);
        private static object _lock = new object();
        private RockLib.Logging.ILogger _splunkLogger;

        public MutareTokenService(ILogger<TokenService> logger, IApplicationSettings applicationSettings, RockLib.Logging.ILogger splunkLog)
        {
            _configuration = applicationSettings.GetConfiguration();
            _logger = logger;
            _splunkLogger = splunkLog;
            _client = new HttpClient();

            _client.BaseAddress = new Uri(_configuration["MutareAuthToken:url"]);

        }

        public string GetMutareToken()
        {
            if (null != _token && DateTime.Now < _expiration)
            {
                return _token;
            }

            lock (_lock)
            {
                if (null != _token && DateTime.Now < _expiration)
                {
                    return _token;
                }

                _logger.LogInformation("Requesting token");
                var jsondata = new
                {
                    audience = _configuration["MutareAuthToken:audience"],
                    grant_type = _configuration["MutareAuthToken:grant_type"],
                    client_id = _configuration["MutareAuthToken:client_id"],
                    client_secret = _configuration["MutareAuthToken:client_secret"]
                };

                var result = _client.PostAsJsonAsync("", jsondata).Result;

                if (!result.IsSuccessStatusCode)
                {
                    string msg = $"HTTP Error: {result.StatusCode} {result.ReasonPhrase} while getting Mutare API access token";
                    _logger.LogError(msg);
                    _splunkLogger.Error(msg);
                    throw new Exception(msg);
                }
                else
                {
                    _logger.LogInformation("Token request successful");
                    _splunkLogger.Info("Token request successful");
                }

                var str = result.Content.ReadAsStringAsync().Result;
                var tokenModel = JsonConvert.DeserializeObject<BearerTokenModel>(str);

                _token = tokenModel.Access_token;
                _expiration = DateTime.Now.AddMinutes(tokenModel.Expires_in / 60 - 5);
                return _token;

            }
        }
    }
}
