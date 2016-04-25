using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bin2csharp
{
    class bin2csharp
    {
        static void Main(string[] args)
        {
            if (args.Count() < 1 || !File.Exists(args[0]))
            {
                Environment.Exit(1);
            }
            string longFileName = args[0];
            string fileNameNoPath = Path.GetFileName(longFileName);
            byte[] originalBytes = File.ReadAllBytes(longFileName);
            byte[] CompressedBytes = Compress(originalBytes);
            String AsBase64String = Convert.ToBase64String(CompressedBytes);

            string[] parts = AsBase64String.SplitByLength(100).ToArray();
            StringBuilder builder = new StringBuilder();
            foreach (string value in parts)
            {
                builder.Append(value);
            }
            string newStr = builder.ToString();

            string className = fileNameNoPath.Replace('.', '_');

            StringBuilder classDefinition = new StringBuilder();

            classDefinition.AppendLine("");
            classDefinition.AppendLine("using System.Diagnostics;");
            classDefinition.AppendLine("using System.IO;");
            classDefinition.AppendLine("using System.IO.Compression;");
            classDefinition.AppendLine("");
            classDefinition.AppendLine("namespace EmbeddedFiles");
            classDefinition.AppendLine("{");
            classDefinition.AppendLine("    public static class " + className);
            classDefinition.AppendLine("    {");
            classDefinition.AppendLine("        public static bool Write(string _fileName)");
            classDefinition.AppendLine("        {");
            classDefinition.AppendLine("            string uuencodedData =");
            classDefinition.AppendLine("                @\"\" +");


            foreach (string value in parts)
            {
                classDefinition.AppendLine("                @\"" + value + "\" +");
            }


            classDefinition.AppendLine("                @\"\";");
            classDefinition.AppendLine("            byte[] decompressedBytes = Convert.FromBase64String(uuencodedData);");
            classDefinition.AppendLine("            byte[] restoredBytes = Decompress(decompressedBytes);");
            classDefinition.AppendLine("            File.WriteAllBytes(_fileName, restoredBytes);");
            classDefinition.AppendLine("            return File.Exists(_fileName);");
            classDefinition.AppendLine("        }");
            classDefinition.AppendLine("        static byte[] Decompress(byte[] gzip)");
            classDefinition.AppendLine("        {");
            classDefinition.AppendLine("            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))");
            classDefinition.AppendLine("            {");
            classDefinition.AppendLine("                const int size = 4096;");
            classDefinition.AppendLine("                byte[] buffer = new byte[size];");
            classDefinition.AppendLine("                using (MemoryStream memory = new MemoryStream())");
            classDefinition.AppendLine("                {");
            classDefinition.AppendLine("                    int count = 0;");
            classDefinition.AppendLine("                    do");
            classDefinition.AppendLine("                    {");
            classDefinition.AppendLine("                        count = stream.Read(buffer, 0, size);");
            classDefinition.AppendLine("                        if (count > 0)");
            classDefinition.AppendLine("                        {");
            classDefinition.AppendLine("                            memory.Write(buffer, 0, count);");
            classDefinition.AppendLine("                        }");
            classDefinition.AppendLine("                    }");
            classDefinition.AppendLine("                    while (count > 0);");
            classDefinition.AppendLine("                    return memory.ToArray();");
            classDefinition.AppendLine("                }");
            classDefinition.AppendLine("            }");
            classDefinition.AppendLine("        }");
            classDefinition.AppendLine("    }");
            classDefinition.AppendLine("}");

            File.WriteAllText(className + ".cs", classDefinition.ToString());
        }


        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
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

    public static class StringGeneralExtensions
    {
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        { // http://stackoverflow.com/questions/3008718/split-string-into-smaller-strings-by-length-variable
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }

    }


}
