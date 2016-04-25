using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.IO.Compression;




namespace UsefulExtensions
{
    class Figlet
    {
        private Dictionary<char, string[]> _alphabet;
        private FigletFont _font;

        public Figlet()
        {
            string fontFile = Path.GetTempFileName();
            standard_flf.Write(fontFile);
            _font = new FigletFont(fontFile);
        }

        public Figlet(string flfFontFile)
        {
            _font = new FigletFont(flfFontFile);
        }


        public void LoadFont(string flfFontFile)
        {
            _font = new FigletFont(flfFontFile);
        }

        public string ToAsciiArt(string strText)
        {
            var res = "";
            for (int i = 1; i <= _font.Height; i++)
            {
                foreach (var car in strText)
                {
                    res += this.GetCharacter(car, i);
                }
                res += Environment.NewLine;
            }
            return res;
        }
        public string GetCharacter(char car, int line)
        {
            var start = _font.CommentLines + ((Convert.ToInt32(car) - 32) * _font.Height);
            var temp = _font.Lines[start + line];
            var lineending = temp[temp.Length - 1];
            var rx = new Regex(@"\" + lineending + "{1,2}$");
            temp = rx.Replace(temp, "");
            return temp.Replace(_font.HardBlank, " ");
        }
        public void PrepareAlphabet(string chaine)
        {
            _alphabet = new Dictionary<char, string[]>();
            foreach (var car in chaine)
            {
                var res = "";
                for (int i = 1; i <= _font.Height; i++)
                {
                    res += this.GetCharacter(car, i) + Environment.NewLine;
                }
                _alphabet.Add(car, res.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            }
        }
        public string RecognizeAsciiArt(string asciiArt)
        {
            var chaine = asciiArt.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (chaine.Length < 1)
                return "";
            var maxChaine = chaine[0].Length;
            var posi = 0;
            var res = "";
            while (posi < maxChaine)
            {
                var newposi = posi;
                foreach (var alpha in _alphabet)
                {
                    var decal = chaine.StartIndexOf(alpha.Value, posi, 1);
                    if (decal > 0)//trouvé
                    {
                        newposi = decal;
                        res += alpha.Key;
                        break;
                    }
                }
                if (newposi == posi)
                {
                    if (newposi < maxChaine)
                    {
                        newposi += 1;
                        res += " ";
                    }//non trouvé
                }
                posi = newposi;
            }
            return res;
        }
    }

    public class FigletFont
    {
        public string Signature { get; private set; }
        public string HardBlank { get; private set; }
        public int Height { get; private set; }
        public int BaseLine { get; private set; }
        public int MaxLenght { get; private set; }
        public int OldLayout { get; private set; }
        public int CommentLines { get; private set; }
        public int PrintDirection { get; private set; }
        public int FullLayout { get; set; }
        public int CodeTagCount { get; set; }
        public List<string> Lines { get; set; }

        public FigletFont(string flfFontFile)
        {
            LoadFont(flfFontFile);
        }

        public FigletFont()
        {
            LoadFont();
        }

        private void LoadLines(List<string> fontLines)
        {
            Lines = fontLines;
            var configString = Lines.First();
            var configArray = configString.Split(' ');
            Signature = configArray.First().Remove(configArray.First().Length - 1);
            if (Signature == "flf2a")
            {
                HardBlank = configArray.First().Last().ToString();
                Height = configArray.GetIntValue(1);
                BaseLine = configArray.GetIntValue(2);
                MaxLenght = configArray.GetIntValue(3);
                OldLayout = configArray.GetIntValue(4);
                CommentLines = configArray.GetIntValue(5);
                PrintDirection = configArray.GetIntValue(6);
                FullLayout = configArray.GetIntValue(7);
                CodeTagCount = configArray.GetIntValue(8);
            }
        }

        private void LoadFont()
        {
            using (var stream = this.GetResourceStream("DataMiner.Fonts.standard.flf"))
            {
                LoadFont(stream);
            }
        }

        private void LoadFont(string flfFontFile)
        {
            using (var fso = File.Open(flfFontFile, FileMode.Open))
            {
                LoadFont(fso);
            }
        }

        private void LoadFont(Stream fontStream)
        {
            var _fontData = new List<string>();
            using (var reader = new StreamReader(fontStream))
            {
                while (!reader.EndOfStream)
                {
                    _fontData.Add(reader.ReadLine());
                }
            }
            LoadLines(_fontData);
        }

    }


    public static class ExtensionsForFiglet
    {

        public static Stream GetResourceStream(this object obj, string resourceName)
        {
            var assem = obj.GetType().Assembly;
            return assem.GetManifestResourceStream(resourceName);
        }
        public static int GetIntValue(this string[] arrayStrings, int posi)
        {
            var val = 0;
            if (arrayStrings.Length > posi)
            {
                int.TryParse(arrayStrings[posi], out val);
            }
            return val;
        }
        public static int StartIndexOf(this string[] chaines, string[] findChaines, int posiInChaine, int startErrorPossible)
        {
            int posi = -1;
            var taille = Math.Min(chaines.Length, findChaines.Length);
            for (int i = 0; i < taille; i++)
            {
                var posiEncours = chaines[i].StartsWidthLastIndex(findChaines[i], posiInChaine, startErrorPossible);
                if (posiEncours < 0)
                    return -1;
                posi = Math.Max(posi, posiEncours);
            }
            return posi;
        }

        public static int StartsWidthLastIndex(this string chaine, string findChaine, int posiInChaine, int startErrorPossible)
        {
            var posi = 0;
            if (chaine == findChaine)
                return posi;
            var ok = chaine.Remove(0, posiInChaine).StartsWith(findChaine);
            while (!ok && posi <= startErrorPossible)
            {
                posi++;
                try
                {
                    ok = chaine.Remove(0, posiInChaine + posi).StartsWith(findChaine);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error : " + ex.Message);
                    ok = false;
                }
            }
            return ok ? posiInChaine + posi + findChaine.Length : -1;
        }
    }

    public static class standard_flf
    {
        public static bool Write(string _fileName)
        {
            string uuencodedData =
                @"" +
                @"H4sIAAAAAAAEAJ1Z62vjSBL/rr+iGIaNzSaRJb+PYfBx3C0DB3ewsJ8EHSWWY7OOPdjO7Qzoj7/uerS6W21FiRNs2ep6dL1+1aXN" +
                @"fpOXn2EGU8hHkE0hmyW/X8rDujyt4fEn/LavDgf4x7b8/r3a7+EX+Fbi1x2M0+UY7u7gsTxXazge4F+n8vDnzRnuz7vn5Nvhaf+6" +
                @"rs7w7ff/wL/Ly+5wlyWb3fO+usCp2leaCPL7zDDIcvj76zNky+Uk+W91etmdzzvNbneGbXWqtA7Pu/9VB7gc4eW43m1+wmWr722O" +
                @"h8stlGfYHw/P5vOyrRJcsKtOWotD+VIZHt/35RPpV8LT8eWlOlxgvztU90nyzx/65kHrpm8eN7DZnc5072/JRpsF7uDTS/m8e4LD" +
                @"68tjdfqkZZ70qr3mu9ZstKQnJE5K0K87OG+Pr/s1lPu/yp9neKzgoby5RaLD8a/kMy3SasKnrbbuo5b95ydjgO+n3eFyNnsoAX+9" +
                @"hcfXCzyVh5uLYXN+eT1vq3UyIw7bave8vRiN9Y40p/LpUp2SacfNW63ABXbokp22lvbLU3VYV6dzop1uyF7KH7hz0O5+vmxhUP2Q" +
                @"xa7RzkP4VTPevK6fK9ho3sdTkk2Rw7ralK/7Cymr/VDhxq2rkmyGy8iURj+Pb5J8XgX/qwQUrJIaanlT+m2ghvoGAN02CwZQg/nt" +
                @"D/1nbmk7Ox+8FPRS829+U5qb/kfm+qf7e32npmvzMtegb9f63/JoGCn8TdPrjxSUUadQCgqjCEBq7mk6Uk/fRo1T/B30h6HVH0Z2" +
                @"qlLZjOWtWMWBJh4CLda808KYYKC+AnzRv2l5Ki3SRjWx1cAYokYhn+2b5U2i0Zjgv2uOhV6kcG8FFLxD/13ry6ZEHY0KhmcBtO3U" +
                @"bLcw94u0MZslAHGCeEDM39jbMTaReW/e/nyufKHMC7maF3I1UfA2VyekgA0F0PIZkM8g2Jwiq5Gj2KISs4rMh/7y1cAAaCLbhrfc" +
                @"Zq5mK+QOE2nIzMRcGuwSHDKxAOlt7FywcYYkJaYMO4YSo7liXtZJgZd8Vc1V7encJRMk1DEbkP5GDDjgrcbs5u4QavEUiKuQo+ck" +
                @"30uUWJ67OqRFnOusvhX5LTI/stox1r7tBnaTqbQzTsiCsivIqVbQd0W/l+CFqVksx+6Abcx+L9isUtQS3L5fVsVEGJjE2aitHjiw" +
                @"NQlnAdqMNCs4IjjjbCanUIDlZq454IkxOdbaIRqC6N0ammTsDEHcKUlgbSUP+FYRTzIIhAQZH00ycRSIo0xWedI6UxoaJSVZhV7V" +
                @"1oDde2M8dcpSKA3sonBL6CYqU1C3laRqrhgtwH7UjgwJXMW6khoKetVLTAt9nyHsBlOjhntygaqdsJCdsHHbF13mxsqE+zU4RnoV" +
                @"qWgouhqTu1YIrFfIlqAQysJarwhkfhRCInHIwQ4WI+KR0UOam2VvS4MvIHuLk6WCDkUvdGiqGtgGARh6motWyxAL37ctqShWqJkx" +
                @"9YcqnrkWaCl0byldTeHDi8vC4+Iz8nn57FocG6bMThhJMyLl0A96QmpWQEj+kEagw2zmxUgfbVTdNOEswUJWx5rJ2iwWk4gx2Axs" +
                @"gEK2XrS5qrDvlIxDrsaQBikL1If6W/e91XFevQiB00ViQPatFtryUHTycGBu0ETXbaSm2roTdDoWne7b6NQIs4WcRVlk8pwCIJVV" +
                @"sTK91IvIgUJqSFSOXSdoxFjE9SYo9n0sRm62LUGnxZoC3EIh4JYq3lOLdbgVI+mCQSnHmKMASfNRB6TQtdDGk9oWDY0tFBrD2diD" +
                @"X4i9DfogE5oVWfW2j+9qHwK6muAuaQIB9z7gvNf3QGjqhKOSInPjtGYiIAz7lbQgnCR8IMeIcsLRHnKkD2sC2KgStimsx7WO6Goy" +
                @"wRuVGMt9hKyhdIldepeFz8VlFEGOrxS4LcTovc8gQxujKwLplOCC0cK3JB2o+RiBEPFFawP2G/D4IZpFkkpsGcYTh/YrfLXf8IBA" +
                @"KJHWCBMpY0PkwwkKrhyKS0Oq7MnDnjtajU2LzOmnOvtYIQOpVf3a37a0ftBzTcmuQ3Y/JVvBHyKIVM264O+1SUdO4VWSzUaxqV82" +
                @"y96q5dksp3oOXMIFIIcuQOoVVOlpHpfNxhSN9rx6q4wpaBglMUV7GOjNeWcDYjBJUpq94diLwt5aks+p+haeUlPXNtlsmlDZB6cU" +
                @"1PhL7VxEurNsNnMzQ9Wr4KtZMqedAUGZMUPKbZZ+p4O+OX6nZmecI9lsEfE0jgy48QkvHJWWbEkZ4thDOnAdMC2IANIAV9Wsg7kj" +
                @"RYw6f2Q5HyUCFBLaEmK1N9Bwqlc2zyjqeGAiIxMZmsjYxCt52Ty/0gWCC0Uszd/3fOz0Sw4H6FRy0m2sGlOSjIUol4qxtB6qiBpr" +
                @"+sa0s31htZkRIpNsqQW2FKQQ3cKc0w03LABqz2bKKBqoYkkXzngAhm2Y8KpxNl82qKJ48ORBub98gVGDFXrlNuxN154tsl4oZ1qY" +
                @"W9vDMmmegLWzsRbwQGDA1MZi0kiit4KCscB4WbnTvpZ6k0Rc5L0ZiqGmMEumrXmxqgMuM6cDwhpE9qrjVl7MEzmYN3nSDAR5JOjA" +
                @"AhJJveDu2EQupjo3VSnGMp0A9bVxN8qmqR3PD5sXslw6LC1HbvxqmgwPmZ89lCpc4rfFhtlyJOMG0RBDqNERQ8nVUnl6wjVNmT3W" +
                @"GuTDQ1DuOeiQ7B6TcHmeNDNuU4Q49d9uLbIl45RQpO8gNdGUptwhaYq6P+nULDSTLiQt3iN19tE2Klu6lQUGx+E7pC7wm7IJipXT" +
                @"ViB0OoMpjqYZ2g2rsFBZlsvkvVPhoYmFVZKPRo6/DeS0Z71XSmQ+yhx/m7R7B2ke8XdP0nHEaT1JJ4moWygav7qT32Dkm4+miR0a" +
                @"8Cmxc7mOppTHPKqo31w+jwUf0mBQyNwL6cDfx0KQWRpJkNlkgwvUmvGwQPnNMrNZcuZI4hjzydz3/oELtjP1NUSZGy+2PvQ5Q+SZ" +
                @"Gy+2PvQjjcVLT9JxrD70I5189LyUZ9MEQLBLJy8+CqfH4IV9BI4LZ37qpimDCKNmmvJMLG3LmEdTt8e5LM8W0dTtRbr0XAFF3Zs0" +
                @"H330IJnnTuwoVPjauDg8h+R5Hkzm7GMpOy9sjncB6Th59+kwzydBhvQeueb5NMiQd5DOrmRID9J5kCHvkLq4kiE9SJfuXgeDoX28" +
                @"ffMW6XjktlBkbSeFHkBcZXg04+DwaMzMpM9eeb6Oz6wFNcfSJUkRdAPkygA6H0t/JOWvF5F0RuLWXkTT7sLFj9n4VOvbdpYo6guL" +
                @"K0OMfDx3Di9XliwCLPQfrrq9Ki5fRhsxgcCOZ0D5ZETBS49meF6oq6H+gWcbciZv1ZRJFkGGXiPpfBLtkvsMqfJJtEvuRxrtkvuR" +
                @"RrvkfqTRLrkf6dx+c9plwyFo0QyPgHRh83QlCGkhkgWnKUsOwJEYLHujY1hfpqPe6NgizXqjY4s0/+gEM5+O+yvsT8Tz6aTfk772" +
                @"k5J8Ou2vsC/1/8PGzMO9KwAA" +
                @"";
            byte[] decompressedBytes = Convert.FromBase64String(uuencodedData);
            byte[] restoredBytes = Decompress(decompressedBytes);
            File.WriteAllBytes(_fileName, restoredBytes);
            return File.Exists(_fileName);
        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }


    public static class small_flf
    {
        public static bool Write(string _fileName)
        {
            string uuencodedData =
                @"" +
                @"H4sIAAAAAAAEAJ1aa0/bSBT9jsR/uB+QSiSCsQmBSKhakxiwGuKsk9BFsuSmxZRoQ4KSsKtK/vE743m/7HSLVJswc+bOnXPuHI/z" +
                @"snwJ5kdwAR3wz8G/AP8MziAIOui21z08mLzNl0v4/gvulsVqBf3X+ft7gT7peL1zaLfh+3xbPMN6BZPdfPU83zwfHsSrH8uP52IL" +
                @"8SSB4Xy3WLX9w4OXxc9lsYNNsSxQFwhOfdzdDyD8+Al+r9c5PBgXm7fFdrtAaIstvBabAo37c/FPsYLdGt7Wz4uXX7B7RX97Wa92" +
                @"JzDfwnK9+omvu9fi8KBqsSg2n7awmr8VGOR9Of9B4pvDj/XbW7HawXKxKk4PDw4PHkj7Zzy98fxjCTcfmx1qer1dLz92KIw/ivlm" +
                @"94ra/326KnafUbRer4tjWZApwqr4F97nGzTYrtgcHmw/3t/Xmx1BvI3v8IRRVvDt18XqFOBh/gvmy+0avhewXS5+vu6Wv+CNhfGy" +
                @"3qA/7BAUfGzRfNYv1QAvH8tl+9/F8+7V+7vYrLzt28f2FcGgdiuU3X+K7Ql8/9jBc/GCJrGD9cfuHf2KZj9KpvDjdb76WTxXE4aj" +
                @"P2z/4f8hB3wpoawueXU5zlvV3wBYG9LqGLUjf3lEP6TJEdAb3pq0px/nJcamg6BPT/FH7BdgvwAeGTXlCKRzWeYknPY1vnqQe/Rz" +
                @"HhjkNLS85aEGpDu6I5+iqycmI00o54Mg0DKnIJ/xPQ0py/G/EqS+IGfsmGSipGMegZIxNAIZHzyaXpCvCD2rGuJAqtHQD5kaWQh2" +
                @"RfGrOcm9LCehwjVB8niIIv1S8kXq5VwDGDnhH9knKRaZJJDh5gzVRgQLokYtnimWKmDLiKduILJ8oTY0YyUct2jO0JJ50rzk1fIo" +
                @"vxWaS3QQWWppQfD5ya1zOncaMuqXsWx4emugIZeCBbm6FkbILBzImVZqBmAKEDEDbZ3ZwpFWDmjeeE8t51o4xwi3tc8AehQnbGXl" +
                @"5VG0RElhp4i1jU5MVW/X6IfrTFOHi8GludSGOj/DZ65KBVbOKh3Ro6lBgVtoLBEZd6EpQ798I4uS4eSe5JzYuVQIuL7RGJmAYSio" +
                @"pTRtG6to8qCeVbkIkIrnmH2WKbmSBqAjCHGWLeDZtYjTYDrkpbose7YubbXCNgNRKywzkLZDVirw8rCiUdonTTWd88rtnAGwAdCF" +
                @"D8CGctUvyu0Ss53uJp8ov1FAylLzLqLk8DuJ77Zp8DpSVnsKDy7zRNEEUDJgSVvGZ3X6jXfLzPFAEYAo5blUy5VkWEic0WgJd50U" +
                @"aBilSSpsFGCjSO2NisetQ81+IFVgkPYB4OHbt+ccJHJK5MnL2oxVTocQCFcxtslmyL+xxpla9KV+SlcAqXfmZZ4MoGPIMByCdcfe" +
                @"hZUqk757ROzIEN+XwbAStmLClphlEpQr7sHD4Tlg0+czN8nA1lbxccJ5UFjwKBu9jPsn9WrsWOaNuodRmuAL3R6BYuvmVALg/ohv" +
                @"PGLb0dinFxL4xGl+KvNcM/FCHQy/NCbIiiIjc1M4RgY8aBN/kNmqrrQRlKxoiJqmt27KjrKhmdlhd59YvbHvG2SdcroTW20pz4uw" +
                @"RIxOucejkIPgK0Ru2H6RW3gK8sCu8UUquBfCExMzy9UNwcgg7JkOy4LWeEwtLo2NwNhYre7vrSqQLU6LCcQYdbRhEyArS59ZpejN" +
                @"dWIbrvB5FsfAozDMCfbW7lziidIy+siKqOnAQesg9ZG7Zere4h4qI308C+ca58JqZa4/9YIo7RTdLCLac2TOn7X5DX/qtrNfiIBO" +
                @"Sa/7eMUURPK4REoL8klVd48/kPMHYvPJOJcKANO2125zA283dXoX8ujARGJWYGEZsOMRtabBMjiGatwfeJ+cT4mbrt8YqpHesq9n" +
                @"ki+vyXEI7qyL3++eAYyS9k0ahV9gMg770WHNkZjf9QHi0WOUTqMBRH/1h+FDOI2TETyE6RfBnrra7XcDgH40msIkvhvJRC5LVnvI" +
                @"dkWv7EjL754DjJPZaCD15CYWZTBnVbQ6qiJxnKiOngJ1UASzNI1G/SeGhVVM+AKnVEi8zKKHRrXM+t0LgKdoxAPJebGVzFhZfVpq" +
                @"t8yiVTBdgJs0+YKQbsLUVF9uUZ/fvQSYRP0q7UoimMYZw7gXy9hJ2HHOVOl3rwAGcRil0SSeuHR3xP3RkWy+GEQPpTEZP6Xx3b20" +
                @"mjUP8CVNaFXNeQHlbsG/RGy8jR7iUTyKIEkH8SgcIr4N4n44TVKWaMsOJY4pdFH4l4iyw+h22h4n8Wgaj+5gkMxuhhGEozv0/5+z" +
                @"ZKpSmJvlKmZ+ZiJOTfQC5F8GUJ0rCzZYDpKEDI4A9FReImpPktsp3D+N7yNFFQJKM8OsKyJzGt3Fkylay0HzMpTQbrFlKMsssy0D" +
                @"YvdD2E8TNhsWgXGeqZdv/xIxehDdpVEkskEGPqXV2KN8kkvCJaL0eDibtNHSzyYyq3kWrMe1lcniwVGj718iak9m4yid9NN4PIXp" +
                @"10RJJD2pcya0p3W/R7M5NDdaNGTLCnCFWBz2Z1NEsT4udKQvGVOZvZqEK0TUhxhlXS6MblOAvNyJZLkoBmLiOB4ikK8mG72KhWQV" +
                @"xMNXblhO/+ocRzIYIHkMkikN0ajuevi4qEaDeDgMpR7qhfRvof5Vjws10ckokvii7h3aWF1M0El/NqwpFC6zbB4++VeIf1UV27dI" +
                @"CHFlygkoPwM1Tqv8K8TKx9nwLkzhNg1J+UYzRtgh2k1TtsqM8V51pMU5i+8/0SWrrJV22kTH6NnHuA+Ht9IAEr4Ez473c89T90yC" +
                @"3TszsStpsBlMWNr5HLBS5EkgpyVmYZ8GHUu2GX/OoolWoCmd2PExn4j5hOv3kCKGaO1G0A/H8RTRZBhNUbQQwtd4eg93afhI1c1Y" +
                @"momT4zrf6ffO66GrGnCo7c6MGA3QnXrofpz2Zw+3w+gvgsptuJeVe+Ff1ONP4+EgotBeyaD3C71bD91oOhrgL+vhUyze8CYhiyr8" +
                @"4fFxq7VX+FcufL6UYjFPuKnDZSbnTz5lblVQzwHdp4sqVc99XzG0sATQbXB25gCPTJ4zlhNY8jCOQr6Wd7HgzK8HFOxmbomWVyeg" +
                @"S4mRjdPaGaAb1SXCaE+6YWQqHoStVe3gzCXE2FI+ePWgxc/18iw4c8kvNnMLvOY3orqUF1sTzDOc0xTXQbtUF9uzbHngLblAyABS" +
                @"pukYLuVF03umCZbkUjjaCrcljkdyy2lMcOaS3qi53klvbizvawLfJbykboMRr0XcpxSB75Jg4txguPNognaJMdlzg2nCd8kyaU54" +
                @"E7RLksl+im+Cxw8/s+E0Hg+xm1Sfskl8GTkpYN8zwS98lJrku4RII5xM8TO/zmn8FR6mSTj2PP4iDvsxC6d9lyZnGvEk5uWceo1n" +
                @"XYHv0uPMxj5Bv73xXZqc2SkocRCycs9BApc6Z02Fa+9DwSBwyfTJUc2rLFXM4cfIynddCKpLodP7JB2xcFmE9Ia+6TEP4YOAC3Ly" +
                @"EA452OQ+TMcwMTzHPoeHQdCxYtY6633eqQXBRR1wQ9mrBe7WAe9T9GrRL+vQG0peLfBVHfAeBa8WvFcH3uinG9DPz+zokVzOKEuk" +
                @"Z/Zv0ktL/QyXAvtWYIuXVg/SjJet9DgiOA+sgBb/rBq9mverwblddRYLrdq8Wky76uwuWjd5tcB21TmMtPWdRpt9oSuzWrxzu/w0" +
                @"L52zh3lWg2jZoWgEyS413T8rx24OJLu29vPM4gWaWcDP7bra3zE3ffEl6Ni1Rf1ylnns/N/LMlOrxhFV0LFLSvfIuHyxhzOPbUF1" +
                @"77CDjl1aNoesScv5pjvo2KVlWGNTWjWYdmlZPbFFWjXAdmnpZth4U1qLaReTwwX/j1eQQecSvyN6jCeKCdZP38TjovF1zqBjF5fp" +
                @"g0WRZlXao6/jPM+YuF1Yuu212I7696fBhV1PVr+r2o4mYLuwHEbXsB1N6HZ1ORzu771TDi7sMjOsbVNO9O8vBBd2rSnultvbfb7I" +
                @"ElzYRfb0f/Kgh/sfLZEffRQ0AAA=" +
                @"";
            byte[] decompressedBytes = Convert.FromBase64String(uuencodedData);
            byte[] restoredBytes = Decompress(decompressedBytes);
            File.WriteAllBytes(_fileName, restoredBytes);
            return File.Exists(_fileName);
        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }

    public static class cybermedium_flf
    {
        public static bool Write(string _fileName)
        {
            string uuencodedData =
                @"" +
                @"H4sIAAAAAAAEAO1UXW/aMBR9j5T/cB5AASlxoKMSTcWEtGmfXTWpe1s6J4DTRS1JlYQWJMRvn53YIQSomq17q6mva+fe43O/HNwF" +
                @"J34LA7zBEFYfJz1de7easCSIowwW5mwWLua69iG8uWMZpnH0wJI0jCNMVvjKuM6ln6YsMnHLN9F4KmzTe3/KSJzcmBha/b51NuAA" +
                @"STx3kGbx9HYcPIZk8eCT6A6dCxZFLMlwJb50de29nzEH/VN8Wdyhf3Y2QK/n9AbOySk+fvuha7r2iSUMPp9pPGcQPFOCyziygoLj" +
                @"Z2MOP0j8cGYiDLCKF3j0o8iX5DNkv9ncxITp2nyFmwVLM4LPuI/TjM3ER4QCOUvCaYHOVYM4YURcbr3oEIgQgyzb+VgSNBlbswLH" +
                @"bJejCRCpmOVAAscAuny63QZATgfwxFRIZnvZ7lI5uigdxwUYIv5jSJBxyysuY0xxK5E6oFxSig5HyKE6HHPzazj8vuFz3YDUejN6" +
                @"CzKyNuAIXQnlOATEMV0QowGUR2CbuaUjobzztuO5BJ5loAkpTiu3IbbhtB0jj7rnLfkF1mg0Okhqr3fKYHqE21jclaVRIMGG53iE" +
                @"UnLeiBPPoDAyHAMuByKz4aSoI0K8hpVpEGkpMHSthdZ4X44FW1tIG1xyi/wE4gMhXBhGflAILrd/z98QYW7g34FQYihIyVbXeKGW" +
                @"/0v+kJPka+EpClfFYpfKf83nZTctybXkqw5MdfBMIF5AwoJeC0kqEalpFTFbU7rOV9ERMiCQCpAK19W1qkClpVKkhxH4KyYV7MMI" +
                @"tfW4ArZZq3EoFWsk+SP2tJvFqRKydgoa+QlV+rtwtnTNpfX7UGW675JCcO1jhJQCpIK7lxhatVSuHc/c8agphGNXyIypDO4p/JRX" +
                @"0QPFIUNYXeouHvWgpgDXrnkAqjTWCmNdAdlBUdbUPpAsqUHVBetKrHaiCflwwK5ldKetXGnuShu36vfxnq1kqBI89RKLjSerU5Xo" +
                @"a/++9u9r/754/24rSYkn+/Y/bv4A/EUC/BoOAAA=" +
                @"";
            byte[] decompressedBytes = Convert.FromBase64String(uuencodedData);
            byte[] restoredBytes = Decompress(decompressedBytes);
            File.WriteAllBytes(_fileName, restoredBytes);
            return File.Exists(_fileName);
        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }

}
