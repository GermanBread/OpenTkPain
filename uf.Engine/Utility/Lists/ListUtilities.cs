// System
using System.Linq;
using System.Collections.Generic;

namespace uf.Utility.Lists
{
    public static class ListUtilities
    {
        public static bool Replace<T>(this List<T> list, T candidate, T replacement) {
            int _index = list.IndexOf(candidate);
            if (_index < 0) return false;
            
            list[_index] = replacement;

            return true;
        }
    }
}