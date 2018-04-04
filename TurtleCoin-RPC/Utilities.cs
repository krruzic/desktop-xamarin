using System;
using System.Collections.Generic;
using System.Text;

namespace TurtleCoinRPC
{
    public static class Utilities
    {
        private static readonly Random getrandom = new Random();

        public static int GenerateRpcId()
        {
            lock (getrandom)
            {
                return getrandom.Next(0, 999999999);
            }
        }
    }
}
