using Spooky;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TurtleCoinRPC
{
    /// <summary>
    /// Overrides the implementation included with Spooky; https://github.com/Yortw/Spooky/blob/master/src/Spooky.SharedImplementation/RpcClient.cs
    /// </summary>
    /// <remarks>
    /// <para>The only difference here is we add in the password to RPC requests.</para>
    /// </remarks>
    public class WalletRPC : RpcClient
    {
        public RpcClientOptions _options;
        public string _password;

        public WalletRPC(RpcClientOptions options, string password) : base(options) {
            _password = password;
            _options = options;
        }


        public new async Task<T> Invoke<T>(string methodName)
        {
            var request = new TurtleCoinRpcRequest()
            {
                MethodName = methodName,
                Arguments = { },
                Password = _password
            };

            return await SendRequest<T>(methodName, request).ConfigureAwait(false);
        }

        public new async Task<T> Invoke<T>(string methodName, params object[] arguments)
        {
            var request = new TurtleCoinRpcRequest()
            {
                MethodName = methodName,
                Arguments = arguments,
                Password = _password
            };

            return await SendRequest<T>(methodName, request).ConfigureAwait(false);
        }

        public new async Task<T> Invoke<T>(string methodName, IDictionary<string, object> arguments)
        {

            //Create a request
            var request = new TurtleCoinRpcRequest()
            {
                MethodName = methodName,
                Arguments = arguments,
                Password = _password
            };

            return await SendRequest<T>(methodName, request).ConfigureAwait(false);
        }

        public new async Task<T> Invoke<T>(string methodName, IEnumerable<KeyValuePair<string, object>> arguments)
        {
            //Create a request
            var request = new TurtleCoinRpcRequest()
            {
                MethodName = methodName,
                Arguments = arguments,
                Password = _password
            };

            return await SendRequest<T>(methodName, request).ConfigureAwait(false);
        }

        private async Task<T> SendRequest<T>(string methodName, TurtleCoinRpcRequest request)
        {
            //Serialize request
            using (var stream = new System.IO.MemoryStream())
            {
                _options.Serializer.Serialize(request, stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                if (string.IsNullOrEmpty(request.Password)) throw new TurtleCoinRPCException("RPC Password not set!");
                //SendRequest
                using (var responseStream = await _options.Transport.SendRequest(stream).ConfigureAwait(false))
                {
                    // Deserialize the response
                    var rpcResult = _options.Serializer.Deserialize<T>(responseStream);

                    // Report error if one occurred
                    if (rpcResult.Error != null)
                        throw new RpcException(methodName, rpcResult.Error);

                    //Return the result
                    return rpcResult.Result;
                }
            }
        }
    }
}
