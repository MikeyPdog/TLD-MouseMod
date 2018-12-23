using System;
using Harmony;

namespace MouseMod
{
    [HarmonyPatch(typeof (vp_FPSCamera))]
    [HarmonyPatch("GetMouseDelta")]
    class PatchMouseMovement
    {
        static bool Prepare()
        {
            try
            {
                MouseModGlobals.LoadFromFile();
            }
            catch (Exception e)
            {
                MouseModGlobals.Log(e.Message);
                throw;
            }

            return true;
        }

        static void Prefix(vp_FPSCamera __instance)
        {
            if (__instance.MouseAcceleration != MouseModGlobals.MouseAcceleration)
            {
                __instance.MouseAcceleration = MouseModGlobals.MouseAcceleration;
            }

            if (__instance.MouseSmoothSteps != MouseModGlobals.MouseSmoothingSteps)
            {
                __instance.MouseSmoothSteps = MouseModGlobals.MouseSmoothingSteps;
            }
        }
    }
}