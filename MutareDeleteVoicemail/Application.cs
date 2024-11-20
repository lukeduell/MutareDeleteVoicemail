using MutareDeleteVoicemail.Services;
using Microsoft.Extensions.Logging;
using MutareDeleteVoicemail.Models;
using MutareDeleteVoicemail.Processors;
using MutareDeleteVoicemail.Utilities;
using Microsoft.EntityFrameworkCore;
using RockLib.Logging;

namespace MutareDeleteVoicemail
{
    public class Application
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<Application> _logger;
        private readonly IMutareVoicemailProcessor _mutareGetVoicemail;
        private readonly IExpiredVoicemails _deleteVoicemailCalc;
        private readonly IMutareVoicemailProcessor _mutareDeleteVoicemail;
        private readonly RockLib.Logging.ILogger _splunkLogger;

        public Application(RockLib.Logging.ILogger splunkLog, ITokenService tokenService, ILogger<Application> logger, 
            IMutareVoicemailProcessor mutareGetVoicemailProcessor, IExpiredVoicemails deleteVoicemailCalc, IMutareVoicemailProcessor mutareDeleteVoicemailProcessor, RockLib.Logging.ILogger splunkLogger)
        {
            _tokenService = tokenService;
            _logger = logger;
            _mutareGetVoicemail = mutareGetVoicemailProcessor;
            _deleteVoicemailCalc = deleteVoicemailCalc;
            _mutareDeleteVoicemail = mutareDeleteVoicemailProcessor;
            _splunkLogger = splunkLog;
        }
        public async Task Run()
        {
            try
            {
                //get the voicemails to be deleted
                _splunkLogger.Info("Getting voicemails to be deleted");
                var alldeletevoicemail = await _mutareGetVoicemail.GetVoicemailDelete();

                _splunkLogger.Info("Calculating voicemails to be deleted");
                List<string> deletevoicemailreturn = _deleteVoicemailCalc.GetExpiredVoicemails(alldeletevoicemail.ToArray());

                if (deletevoicemailreturn.Count == 0)
                {
                    _splunkLogger.Info("No voicemails to delete");
                    return;
                }

                _splunkLogger.Info("Calling mutare endpoint to delete staged voicemails.");
                //TODO: Call the delete mutare voicemail api

                //for (int i = 0; i< deletevoicemailreturn.Length; i++)
                //{
                //    await _mutareDeleteVoicemail.DeleteVoicemail(deletevoicemailreturn[i]);
                //}

                _splunkLogger.Info("Setting staged voicemail flaggs to 1");
                foreach (var voicemail in deletevoicemailreturn)
                {
                    await _mutareDeleteVoicemail.UpdateDeleteFlag(voicemail, 1);
                }

                _splunkLogger.Info("Finished all tasks");
            }
            catch (Exception ex)
            {
                _splunkLogger.Error(ex.Message);
            }
            _splunkLogger.Info("Done doing the done did do");
        }
    }
}
