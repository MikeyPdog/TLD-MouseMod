using System;
using Harmony;
using UnityEngine;

namespace MouseMod
{
    [HarmonyPatch(typeof (vp_FPSCamera))]
    [HarmonyPatch("GetMouseDelta")]
    class PatchMouseMovement
    {
        static bool Prepare()
        {
            FileLog.Log("");
            FileLog.Log(DateTime.Now + " ---- Loading Mouse Mod.");
            try
            {
                MouseModGlobals.LoadFromFile();
            }
            catch (Exception e)
            {
                Debug.LogFormat(e.Message);
                FileLog.Log(e.Message);
                throw;
            }

            return true;
        }

        static void Prefix(vp_FPSCamera __instance)
        {
            if (__instance.MouseAcceleration != MouseModGlobals.MouseAcceleration)
            {
                var msg = " ** MouseMod: MouseAcceleration was " + __instance.MouseAcceleration + ". Changing to " +
                          MouseModGlobals.MouseAcceleration;
                FileLog.Log(msg);
                __instance.MouseAcceleration = MouseModGlobals.MouseAcceleration;
            }

            if (__instance.MouseSmoothSteps != MouseModGlobals.MouseSmoothingSteps)
            {
                var msg = " ** MouseMod: MouseSmoothSteps was " + __instance.MouseSmoothSteps + ". Changing to " +
                          MouseModGlobals.MouseSmoothingSteps;
                FileLog.Log(msg);
                __instance.MouseSmoothSteps = MouseModGlobals.MouseSmoothingSteps;
            }
        }
    }
}