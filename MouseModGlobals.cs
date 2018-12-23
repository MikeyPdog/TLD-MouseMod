using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MouseMod
{
    public static class MouseModGlobals
    {
        public static bool MouseAcceleration = true;
        public static int MouseSmoothingSteps = 5;
        private static readonly Version version;

        static MouseModGlobals()
        {
            version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static void LoadFromFile()
        {
            Log("");
            Log(DateTime.Now + " ---- Loading Mouse Mod.");
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
                Log("*** LINES FOUND WITHOUT MATCH :" + leftoverString);
            }
            
			Log("Version " + version + " loaded!");
        }

        public static void SetGlobal<T>(Dictionary<string, string> dict, string key, Action<T> globalSetter)
        {
            string value;
            if (!dict.TryGetValue(key, out value))
            {
                Log("* No entry for '" + key + "' found. Defaulting value.");
                return;
            }

            try
            {
                T val = typeof(T).IsEnum 
                    ? ParseToEnum<T>(value) 
                    : ParseTo<T>(value);

                Log("* Setting " + key + " to " + val);
                dict.Remove(key);
                globalSetter(val);
            }
            catch (Exception e)
            {
                Log("*** BAD VALUE for '" + key + "' ('" + value + "'). Defaulting value. Full error below:");
                Log(e.Message);
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

        public static void Log(string message)
        {
            Debug.Log("[MouseMod] " + message);
        }
    }
}
