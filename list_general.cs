/*-----------------------------------------------------------------------------------------------------------

             _   _ ____  _____ _____ _   _ _       _______  _______ _____ _   _ ____ ___ ___  _   _ ____
            | | | / ___|| ____|  ___| | | | |     | ____\ \/ /_   _| ____| \ | / ___|_ _/ _ \| \ | / ___|
            | | | \___ \|  _| | |_  | | | | |     |  _|  \  /  | | |  _| |  \| \___ \| | | | |  \| \___ \
            | |_| |___) | |___|  _| | |_| | |___  | |___ /  \  | | | |___| |\  |___) | | |_| | |\  |___) |
             \___/|____/|_____|_|    \___/|_____| |_____/_/\_\ |_| |_____|_| \_|____/___\___/|_| \_|____/

    Useful extensions is a public project to extend functionality of standard C# classes with most frequently
    used functions. 



-----------------------------------------------------------------------------------------------------------*/
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace UsefulExtensions
{

    public static class ListGeneralExtensions
    {


        public static void SortAndClean(this List<string> list)
        {
            list.Sort();
            while (list.Count > 0 && list[0] == "")
            {
                list.RemoveAt(0);
            }
        }


        public static void AddUnique<T>(this List<T> list, T val)
        {
            if (list != null && !list.Contains(val))
            {
                list.Add(val);
            }
        }

        public static object FindApproximate<TValue>(Dictionary<string, TValue> source, string keyORpattern)
        {
            List<string> keys = source.Keys.ToList();
            if (keys==null || keys.Count == 0 )
            {
                return null;
            } else
            {
                if (keys.Contains(keyORpattern))
                {
                    return source[keyORpattern];
                } else
                {
                    foreach (string k in keys)
                    {
                        if (Regex.IsMatch(k, keyORpattern))
                        {
                            return source[keyORpattern];
                        }
                    }
                }
            }
            return null;
        }

        public static void AddIfNew<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key, TValue val)
        {
            if (!source.Keys.Contains(key))
            {
                source.Add(key, val);
            }
        }



    }

    public class Catalogue<T>
    {
        internal Dictionary<string, T> store = new Dictionary<string, T>();
        public Catalogue()
        {
        }

        public T this[string index]
        {
            get
            {
                if (store.Keys.Count == 0)
                {
                    return default(T);
                }
                if (store.Keys.Contains(index))
                {
                    return store[index];
                }
                foreach (string k in store.Keys)
                {
                    //Console.WriteLine(k+" = "+store[k].ToString());
                    if (Regex.IsMatch(k, index))
                    {
                        return store[k];
                    }
                }
                return default(T);
            }
            set
            {
                if (store.Keys.Contains(index))
                {
                    store[index] = value;
                } else
                {
                    store.Add(index, value);
                }
            }
        }
    }
}
