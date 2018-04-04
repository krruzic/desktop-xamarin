using System;
using System.Collections.Generic;
using System.Text;

namespace TurtleCoinRPC
{
    internal class WalletdRpcRequest : RpcRequest
    {
        [Newtonsoft.Json.JsonProperty("password")]
        public string Password { get; set; }
    }
}
