using System;

namespace TurtleWallet.Utilities
{
    public static class MyExtensions
    {
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }
    }
}
