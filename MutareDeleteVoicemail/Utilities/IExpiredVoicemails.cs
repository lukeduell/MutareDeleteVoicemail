using MutareDeleteVoicemail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutareDeleteVoicemail.Utilities
{
    public interface IExpiredVoicemails
    {
        List<string> GetExpiredVoicemails(MutareVoicemailModel[] voicemails);
    }
}
