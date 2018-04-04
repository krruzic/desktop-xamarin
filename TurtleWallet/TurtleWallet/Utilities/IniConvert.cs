using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TurtleWallet.Utilities
{
    public static class IniConvert
    {

        public static Dictionary<string, string> Parse(string value)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            var result = value.Split(new[] { '\r', '\n' });
            foreach (string keyvalue in result)
            {
                if (string.IsNullOrWhiteSpace(keyvalue)) continue;

                string[] keyvalues = keyvalue.Split('=');
                string key = keyvalues[0];
                string keyval = keyvalues[1];
                keyValuePairs.Add(key, keyval);
            }
            return keyValuePairs;
        }

        public static T DeserializeObject<T>(string value)
        {
            var nullInstanceOfT = typeof(T);

            object obj = Activator.CreateInstance(typeof(T));
            Dictionary<string, string> data = Parse(value);
            foreach (var prop in obj.GetType().GetProperties())
            {
                bool propSet = false;
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is IniProperty iniProp)
                    {
                        if (data.ContainsKey(iniProp.Value))
                        {
                            prop.SetValue(obj, Convert.ChangeType(data[iniProp.Value], prop.PropertyType), null);
                            propSet = true;
                            break;
                        }
                    }
                }
                if (data.ContainsKey(prop.Name) && !propSet)
                {
                    prop.SetValue(obj, Convert.ChangeType(data[prop.Name], prop.PropertyType), null);
                }
            }
            if (obj is T)
            {
                return (T)obj;
            }
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        public static string SerializeObject(Object value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var prop in value.GetType().GetProperties())
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is IniProperty iniProp)
                        sb.AppendLine(string.Format("{0}={1}", iniProp.Value, prop.GetValue(value, null)));
                    else
                        sb.AppendLine(string.Format("{0}={1}", prop.Name, prop.GetValue(value, null)));
                    break;
                }
            }
            return sb.ToString();
        }
    }
}