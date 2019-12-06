using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PresampIniHelper
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string inputPath;
            if (args.Length > 0)
            {
                inputPath = args[0];
                Console.WriteLine("Loading input file: " + inputPath);
            }
            else
            {
                Console.WriteLine("Please drop the input file on the icon of this exe to launch conversion.");
                Console.ReadLine();
                return;
            }
            var fileInfo = new FileInfo(inputPath);
            switch (fileInfo.Extension)
            {
                case ".txt":
                    var dictInput = File.ReadAllLines(inputPath);
                    var iniOutput = ConvertDictToIni(dictInput);
                    var outputIniPath = inputPath + ".output.ini";
                    File.WriteAllLines(outputIniPath, iniOutput);
                    break;
                case ".ini":
                    var iniInput = File.ReadAllLines(inputPath);
                    var dictOutput = ConvertIniToDict(iniInput);
                    var outputDictPath = inputPath + ".output.txt";
                    File.WriteAllLines(outputDictPath, dictOutput);
                    break;
                default:
                    Console.WriteLine("Aborted: Unknown file extension: " + fileInfo.Extension);
                    Console.ReadLine();
                    return; ;
            }
        }

        public static string[] ConvertDictToIni(string[] dictInput)
        {
            var vDict = new Dictionary<string, List<string>>();
            var cDict = new Dictionary<string, List<string>>();

            foreach (var row in dictInput)
            {
                var splitted = row.Split(',');
                var whole = splitted[0];
                var c = splitted[1];
                var v = splitted[2];
                AddToDict(vDict, v, whole);
                if (c != v)
                {
                    AddToDict(cDict, c, whole);
                }
            }

            var result = new List<string>();
            result.Add("[VERSION]");
            result.Add("1.7");
            result.Add("[VOWEL]");
            foreach (var item in vDict)
            {
                var values = string.Join(",", item.Value);
                result.Add(item.Key + "=" + item.Key + "=" + values + "=100");
            }
            result.Add("[CONSONANT]");
            foreach (var item in cDict)
            {
                var values = string.Join(",", item.Value);
                result.Add(item.Key + "=" + values + "=0");
            }
            result.Add("[ENDFLAG]");
            result.Add("1");
            return result.ToArray();
        }

        public static string[] ConvertIniToDict(string[] iniInput)
        {
            var input = iniInput.ToList();
            var vDict = new Dictionary<string, string>();
            var vRowIndex = input.IndexOf("[VOWEL]");
            if (vRowIndex == -1)
            {
                throw new InvalidDataException("Could not find [VOWEL] tag.");
            }
            for (var i = vRowIndex + 1; ; i++)
            {
                if (input.Count <= i) break;
                var row = input[i];
                if (row[0] == '[') break;
                var v = row.Split('=')[0];
                var list = row.Split('=')[2].Split(',').ToList();
                foreach (var whole in list)
                {
                    vDict[whole] = v;
                }
            }
            var cDict = new Dictionary<string, string>();
            var cRowIndex = input.IndexOf("[CONSONANT]");
            if (cRowIndex == -1)
            {
                throw new InvalidDataException("Could not find [CONSONANT] tag.");
            }
            for (var i = cRowIndex + 1; ; i++)
            {
                if (input.Count <= i) break;
                var row = input[i];
                if (row[0] == '[') break;
                var c = row.Split('=')[0];
                var list = row.Split('=')[1].Split(',').ToList();
                foreach (var whole in list)
                {
                    cDict[whole] = c;
                }
            }
            var result = new List<string>();
            foreach (var key in vDict.Keys)
            {
                var whole = key;
                var v = vDict[key];
                var c = whole;
                if (cDict.ContainsKey(key))
                {
                    c = cDict[key];
                }
                result.Add(whole + "," + c + "," + v);
            }
            return result.ToArray();
        }

        public static void AddToDict(Dictionary<string, List<string>> dictionary, string key, string value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<string>());
            }
            if (!dictionary[key].Contains(value))
            {
                dictionary[key].Add(value);
            }
        }
    }
}
