using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TurtleCoinRPC
{
    public class WalletRPCHttpClient
    {
        private HttpClient _client = new HttpClient();
        private Uri _uri;
        private string _password;
        private WalletdRpcRequest EmptyRequest => new WalletdRpcRequest
        {
            Password = _password
        };


        public WalletRPCHttpClient(Uri rpc_url, string password)
        {
            _uri = rpc_url;
            _password = password;
        }


        public async Task<JObject> Save()
        {
            WalletdRpcRequest request = EmptyRequest;
            request.MethodName = "save";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://eu.turtlepool.space:11899/json_rpc");
            string content = JsonConvert.SerializeObject(request); 
            requestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(requestMessage);
        }
    }
}
