using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestUnityPlugin
{
    internal static class UsefulFuncs
    {
        public static Vector3 GetCrosshairPosition(bool GetGroundPos = false, float MaxDistance = 100f)
        {
            RaycastHit raycastHit = HelperFunctions.LineCheck(MainCamera.instance.transform.position, MainCamera.instance.transform.position + MainCamera.instance.transform.forward * MaxDistance, HelperFunctions.LayerType.TerrainProp, 0f);
            Vector3 vector = MainCamera.instance.transform.position + MainCamera.instance.transform.forward * MaxDistance;
            if (raycastHit.collider != null)
                vector = raycastHit.point;
            return GetGroundPos ? HelperFunctions.GetGroundPos(vector + Vector3.up * 1f, HelperFunctions.LayerType.TerrainProp, 0f) : vector;
        }
    }
}
