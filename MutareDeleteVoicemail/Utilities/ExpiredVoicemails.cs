using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MutareDeleteVoicemail.Models;
using MutareDeleteVoicemail.Services;
using RockLib.Logging;

namespace MutareDeleteVoicemail.Utilities
{
    public class ExpiredVoicemails : IExpiredVoicemails
    {
        private readonly ILogger<TokenService> _logger;
        private readonly RockLib.Logging.ILogger _splunkLog;
        public ExpiredVoicemails(ILogger<TokenService> logger, RockLib.Logging.ILogger splunkLog)
        {
            _logger = logger;
            _splunkLog = splunkLog;
        }
        public List<string> GetExpiredVoicemails(MutareVoicemailModel[] voicemails)
        {
            List<string> stagedDelete = new();
            foreach(var voicemail in voicemails)
            {
                var sentDateTime = DateTime.Parse(voicemail.SentDateTime);
                var conditCalc = DateTime.Now.AddDays(-14) >= sentDateTime;
                if (conditCalc && (voicemail.Deleted == 0))
                {
                    stagedDelete.Add(voicemail.VoicemailId);
                    _splunkLog.Info($"Found voicemail ID: {voicemail.VoicemailId} to stage delete");
                }
            }

            return stagedDelete;
        }
    }
}