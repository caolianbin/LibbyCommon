using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 缓存类
{
    public class MemoryCacheHelper
    {
        private static readonly MemoryCache mCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 判断缓存是否存在
        /// </summary> 
        public static bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            return mCache.TryGetValue(key,out _);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiressSliding">滑动过期时间（如果在过期时间内操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时间</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Set(string key, object value, TimeSpan expiressSliding, TimeSpan expiressAbsoulte) 
        {
            if (string.IsNullOrWhiteSpace(key)) 
            {
                return false;
            }
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            mCache.Set(key,value,new MemoryCacheEntryOptions().SetSlidingExpiration(expiressSliding).SetAbsoluteExpiration(expiressAbsoulte));
            return Exists(key);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresIn">缓存时间</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>

        public static bool Set(string key,object value,TimeSpan expiresIn,bool isSliding = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            if (value == null) 
            {
                throw new ArgumentNullException(nameof(value));
            }
            mCache.Set(key, value, isSliding ? new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn) : new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));
            return Exists(key);
        }
        /// <summary>
        /// 删除缓存
        /// </summary> 
        public static void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));   
            }
            mCache.Remove(key);
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RemoveAll(IEnumerable<string> keys)
        {
            if(keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            keys.ToList().ForEach(item => mCache.Remove(item));
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void RemoveCacheAll() 
        {
            var cheList = GetCacheKeys();
            foreach (var item in cheList)
            {
                    Remove(item);
            }
        }
        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="para"></param>
        public static void RemoveCacheRegex(string para)
        {
            IList<string> lists = SearchCacheRegex(para);
            foreach (var item in lists)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// 搜索匹配到的缓存
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static IList<string> SearchCacheRegex(string para)
        {
            var cacheKeys = GetCacheKeys();
            var cache = cacheKeys.Where(x => Regex.IsMatch(x,para)).ToList();
            return cache.AsReadOnly();
        }
        /// <summary>
        /// 互殴系统所有的缓存key
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCacheKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = mCache.GetType().GetField("_entries", flags).GetValue(mCache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry item in cacheItems)
            {
                keys.Add(item.Key.ToString());
            }
            return keys;
        }

        /// <summary>
        /// 获取缓存
        /// </summary> 
        public static T Get<T>(string key) where T : class 
        {
            if (string.IsNullOrWhiteSpace(key)) 
            {
                return null;
            }
            return mCache.Get(key) as T;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key) 
        {
            if (string.IsNullOrWhiteSpace(key)) 
            {
                return null;
            }
            return mCache.Get(key);
        }
        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDictionary<string, object> GetAll(IEnumerable<string> keys) 
        {
            if(keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(item => dict.Add(item, mCache.Get(item)));
            return dict;
        }


    }
}
