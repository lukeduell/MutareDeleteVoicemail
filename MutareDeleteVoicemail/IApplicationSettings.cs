using Microsoft.Extensions.Configuration;

namespace MutareDeleteVoicemail
{
    public interface IApplicationSettings
    {
        IConfiguration GetConfiguration();
    }
}