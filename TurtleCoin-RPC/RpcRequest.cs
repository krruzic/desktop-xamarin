using System.Collections.Generic;
using Windows.Data.Json;

namespace TurtleCoinRPC
{
    internal class RpcRequest
    {

        [Newtonsoft.Json.JsonProperty("jsonrpc")]
        public string Version { get; set; } = "2.0";

        /// <summary>
        /// Gets or sets the unique id of the request, used to correlate responses.
        /// </summary>
        /// <remarks>
        /// <para>By default, a unique integer value is provided (in a thread-safe manner).</para>
        /// </remarks>
        /// <value>The identifier.</value>
        [Newtonsoft.Json.JsonProperty("id")]
        public int Id { get; set; } = Utilities.GenerateRpcId();

        /// <summary>
        /// Gets or sets the name of the remote method to be called.
        /// </summary>
        /// <value>The name of the method.</value>
        [Newtonsoft.Json.JsonProperty("method")]
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of arguments to pass to the remote method.
        /// </summary>
        /// <value>The arguments.</value>
        [Newtonsoft.Json.JsonProperty("params")]
        public KeyValuePair<string, dynamic> Arguments { get; set; }
    }
}