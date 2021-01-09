using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace F45TargetSelection
{

    [HarmonyPatch(typeof(MFDPTacticalSituationDisplay), "UpdateTargetInfo")]
    class TargetInfoPatch
    {
        public static void Postfix(MFDPTacticalSituationDisplay __instance, Actor a)
        {
            Vector3 position = __instance.tsc.weaponManager.actor.position;
            Vector3 position2 = a.position;
            int num = Mathf.RoundToInt(VectorUtils.Bearing(position, position2));
            int num2 = Mathf.RoundToInt(__instance.measurements.ConvertedDistance(Vector3.Distance(position, position2)));
            if (__instance.measurements.distanceMode == MeasurementManager.DistanceModes.Feet || __instance.measurements.distanceMode == MeasurementManager.DistanceModes.Meters)
            {
                num2 /= 1000;
            }
            float num3 = Mathf.RoundToInt(__instance.measurements.ConvertedAltitude(WaterPhysics.GetAltitude(position2)) / 100f) / 10f;
            __instance.braNumsText.text = string.Format("{0}\n{1}\n{2}", num, num2, num3);

        }

        
    }
}






