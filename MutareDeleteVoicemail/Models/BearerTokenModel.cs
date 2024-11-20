using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutareDeleteVoicemail.Models
{
    public class BearerTokenModel
    {
        public string Access_token { get; set; }
        public string Scope { get; set; }
        public string ReadWrite { get; set; }
        public int Expires_in { get; set; }
        public string Token_type { get; set; }
    }
}
