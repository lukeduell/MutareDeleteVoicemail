using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace MutareDeleteVoicemail.Utilities
{
    public class LogSettingsUsed : ILogSettingsUsed
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LogSettingsUsed> _logger;

        public LogSettingsUsed(IApplicationSettings applicationSettings, ILogger<LogSettingsUsed> logger)
        {
            _configuration = applicationSettings.GetConfiguration();
            _logger = logger;
        }

        public void WhatSettingsAreUsed()
        {
            try
            {
                _logger.LogInformation("====================================================");
                _logger.LogInformation("=            Mutare Delete Voicemail               =");
                _logger.LogInformation("====================================================");
                _logger.LogInformation("Starting application with below settings...");

                string appName = _configuration["AppSettings:AppName"];
                string appId = _configuration["AppSettings:AppIdCore"];
                string buildConfiguration = _configuration["AppSettings:BuildConfiguration"];
                string buildNumber = _configuration["AppSettings:BuildNumber"];

                _logger.LogInformation($"Application Name: --------- {appName}");
                _logger.LogInformation($"Application Id: ----------- {appId}");
                _logger.LogInformation($"Build Configuration: ------ {buildConfiguration}");
                _logger.LogInformation($"Build No: ----------------- {buildNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
