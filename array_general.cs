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

    public static class ArrayGeneralExtensions
    {

        public static void Fill<T>(this T[] originalArray, T with)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }
        }


        public static void FillIncrement(this int[] originalArray, int startValue)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = startValue++;
            }
        }

        public static void FillDecrement(this int[] originalArray, int startValue)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = startValue--;
            }
        }

    }

}
