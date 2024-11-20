using MutareDeleteVoicemail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MutareDeleteVoicemail.Processors
{
    public interface IMutareVoicemailProcessor
    {
        Task<IEnumerable<MutareVoicemailModel>> GetVoicemailDelete();
        Task<HttpResponseMessage> UpdateDeleteFlag(string voicemailID, int deleted);
        //Task<HttpResponseMessage> DeleteVoicemail(string id);
    }
}
