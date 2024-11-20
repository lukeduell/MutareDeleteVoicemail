using MutareDeleteVoicemail.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using RockLib.Logging;

namespace MutareDeleteVoicemail.Services
{
    public class TokenService :ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        private static HttpClient _client;
        private string _token;
        private static DateTime _expiration = new DateTime(1900, 1, 1);
        private static object _lock = new object();
        private RockLib.Logging.ILogger _splunkLogger;

        public TokenService(ILogger<TokenService> logger, IApplicationSettings applicationSettings, RockLib.Logging.ILogger splunkLog)
        {
            _configuration = applicationSettings.GetConfiguration();
            _logger = logger;
            _splunkLogger = splunkLog;
            _client = new HttpClient();

            _client.BaseAddress = new Uri(_configuration["AuthToken:url"]);
            
        }

        public string GetToken()
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
                    audience = _configuration["AuthToken:audience"],
                    grant_type = _configuration["AuthToken:grant_type"],
                    client_id = _configuration["AuthToken:client_id"],
                    client_secret = _configuration["AuthToken:client_secret"]
                };

                var result = _client.PostAsJsonAsync("", jsondata).Result;

                if (!result.IsSuccessStatusCode)
                {
                    string msg = $"HTTP Error: {result.StatusCode} {result.ReasonPhrase} while getting Black Box API access token";
                    _splunkLogger.Error(msg);
                    throw new Exception(msg);
                }
                else
                {
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
