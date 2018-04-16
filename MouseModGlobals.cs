using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;

namespace MouseMod
{
    public static class MouseModGlobals
    {
        public static bool MouseAcceleration = true;
        public static int MouseSmoothingSteps = 5;

        public static void LoadFromFile()
        {
            var dic = File.ReadAllLines("mods/MouseMod.txt")
                        .Where(l => !l.StartsWith("/"))
                        .Select(l => l.Split(new[] { '=' }))
                        .Where(arr => arr.Length == 2)
                        .ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            SetGlobal<int>(dic, "MouseSmoothingSteps", x => MouseSmoothingSteps = x);
            SetGlobal<bool>(dic, "MouseAcceleration", x => MouseAcceleration = x);

            var leftoverEntries = dic.Select(kvp => kvp.Key + "=" + kvp.Value).ToArray();
            if (leftoverEntries.Any())
            {
                var leftoverString = string.Join(", ", leftoverEntries);
                FileLog.Log("*** LINES FOUND WITHOUT MATCH :" + leftoverString);
            }

            FileLog.Log("Finished loading MouseMod values.");
        }

        public static void SetGlobal<T>(Dictionary<string, string> dict, string key, Action<T> globalSetter)
        {
            string value;
            if (!dict.TryGetValue(key, out value))
            {
                FileLog.Log("* No entry for '" + key + "' found. Defaulting value.");
                return;
            }

            try
            {
                T val = typeof(T).IsEnum 
                    ? ParseToEnum<T>(value) 
                    : ParseTo<T>(value);

                FileLog.Log("* Setting " + key + " to " + val);
                dict.Remove(key);
                globalSetter(val);
            }
            catch (Exception e)
            {
                FileLog.Log("*** BAD VALUE for '" + key + "' ('" + value + "'). Defaulting value. Full error below:");
                FileLog.Log(e.Message);
            }
        }

        public static T ParseTo<T>(object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static T ParseToEnum<T>(string input)
        {
            return (T)Enum.Parse(typeof(T), input);
        }
    }
}
