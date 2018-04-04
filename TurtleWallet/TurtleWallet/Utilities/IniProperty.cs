using System;
using System.Collections.Generic;
using System.Text;

namespace TurtleWallet.Utilities
{
    class IniProperty : Attribute
    {
        public string Value { get; set; }
        public IniProperty(string PropertyName)
        {
            this.Value = PropertyName;
        }
    }
}
