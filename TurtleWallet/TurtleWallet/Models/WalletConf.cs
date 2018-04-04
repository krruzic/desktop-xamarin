using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using TurtleWallet.Utilities;

namespace TurtleWallet.Models
{
    public class WalletConf 
    {
        [IniProperty(PropertyName: "container-file")]
        public string FileName { get; set; }

        [IniProperty(PropertyName: "container-password")]
        public string WalletPassword { get; set; }

        [IniProperty(PropertyName: "rpc-password")]
        public string RPCPassword { get; set; }

        [IniProperty(PropertyName: "bind-port")]
        public int Port { get; set; }

        public WalletConf(string fileName)
        {
            FileName = fileName;
            WalletPassword = "faucetlbangK";
            RPCPassword = "124";
            Port = 8071;
        }

        public WalletConf() { }


        public static WalletConf FromJson(string json) => JsonConvert.DeserializeObject<WalletConf>(json, Converter.Settings);

        public static WalletConf FromIni(string ini) => IniConvert.DeserializeObject<WalletConf>(ini);
    }

    public static class Serialize
    {
        public static string ToJson(this WalletConf self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }

        public static string ToIni(this WalletConf self)
        {
            return IniConvert.SerializeObject(self);
        }
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
