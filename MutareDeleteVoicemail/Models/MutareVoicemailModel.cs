using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutareDeleteVoicemail.Models
{
    public class MutareVoicemailModel
    {
        public string HumanId { get; set; }
        public string VoicemailId { get; set; }
        public int Deleted { get; set; }
        public string SentDateTime { get; set; }
    }
    public class MutareVoicemailStagedDeleteModel
    {
        public string Id { get; set; }
    }
}
