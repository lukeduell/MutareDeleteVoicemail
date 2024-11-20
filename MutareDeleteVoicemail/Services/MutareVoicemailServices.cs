using MutareDeleteVoicemail.Models;
using MutareDeleteVoicemail.Processors;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MutareDeleteVoicemail.Services;

namespace MutareDeleteVoicemail.Services
{
    public class MutareVoicemailServices: IMutareVoicemailProcessor
    {
        private readonly HttpClient httpClient;
        private readonly ITokenService _tokenService;
        //private readonly IMutareTokenService _mutaretokenService;
        private readonly string _mutareUrl = "Something for Mutare"; //set this to the hard delete api path
        private readonly string _bbapiIrl = "/Mutare"; //set this to the base url for the api

        //TODO: need to add another token service for Mutare API
        //public MutareVoicemailServices(HttpClient httpClient, ITokenService tokenservice, IMutareTokenService mutaretokenservice)
        public MutareVoicemailServices(HttpClient httpClient, ITokenService tokenservice)
        {
            this.httpClient = httpClient;
            _tokenService = tokenservice;
            //_mutaretokenService = mutaretokenservice;

            string bbtoken = _tokenService.GetToken();
        }

        


        //getting the voicemails to be hard deleted.
        public async Task<IEnumerable<MutareVoicemailModel>> GetVoicemailDelete()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var response = await httpClient.GetAsync($"api/MutareVoicemailEvents");

            //returning object, null is not handled
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<MutareVoicemailModel[]>() ?? Array.Empty<MutareVoicemailModel>(); ;
            }
            else
            {
                //if not found return a empty model
                return Array.Empty<MutareVoicemailModel>();
            } 
            
        }
        public async Task<HttpResponseMessage> UpdateDeleteFlag(string voicemailID, int deleted)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var content = JsonContent.Create(deleted);
            HttpResponseMessage updateResult = await httpClient.PutAsync($"api/MutareVoicemailEvents/{voicemailID}", content);

            return updateResult;
        }

        //public async Task<HttpResponseMessage> DeleteVoicemail(string id)
        //{
        //    //TODO: token needs to be changed to Mutare token
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mutaretokenService.GetMutareToken());
        //    HttpResponseMessage deleteResult = await httpClient.DeleteAsync($"api/{_mutareUrl}/{id}");

        //    return deleteResult;
        //}
    }
}
