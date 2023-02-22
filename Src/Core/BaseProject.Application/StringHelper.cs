using System;

namespace BoxApi.Application
{
    public static class StringHelper
    {
        public static bool IsWhiteSpace(this string str)
        {
            if(str != null)
            {
                foreach (char c in str.ToCharArray())
                {
                    if (!Char.IsWhiteSpace(c))
                        return false;
                }
            }

            return true;
        }
    }
}
