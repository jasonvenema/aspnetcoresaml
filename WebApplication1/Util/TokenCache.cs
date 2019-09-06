using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Util
{
    public static class TokenCache
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        public static void AddToken(string name, string token)
        {
            if (!_cache.ContainsKey(name))
            {
                _cache.Add(name, token);
            }
            else
            {
                _cache.Remove(name);
                _cache.Add(name, token);
            }
        }

        public static void RemoveToken(string name)
        {
            if (_cache.ContainsKey(name))
            {
                _cache.Remove(name);
            }
        }

        public static string GetToken(string name)
        {
            return _cache.GetValueOrDefault(name, null);
        }

        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}