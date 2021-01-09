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

    [HarmonyPatch(typeof(MFDPTacticalSituationDisplay), "CreateActorIcon")]
    class ActorIconPatch
    {
        public static void Postfix(MFDPTacticalSituationDisplay __instance, ref TSDContactIcon __result)
        {
            if (VTOLAPI.GetPlayersVehicleEnum() == VTOLVehicles.F45A)
            {
                GameObject f45 = VTOLAPI.GetPlayersVehicleGameObject();
                GameObject soiButton = GetChildWithName(f45, "ScaleButton (1)");
                GameObject hvrObj = GetChildWithName(soiButton, "hvrObj");
                tsdTraverse = Traverse.Create(__instance);

                GameObject contactHvrObj = UnityEngine.Object.Instantiate(hvrObj, __result.transform);

                float scaler = 1.3f;
                Vector2 hvrObjOrigSize = contactHvrObj.GetComponent<Image>().rectTransform.sizeDelta;
                contactHvrObj.GetComponent<Image>().rectTransform.sizeDelta *= scaler;

                __result.gameObject.SetActive(false);
                VRInteractable interact = __result.gameObject.AddComponent<VRInteractable>();
                interact.interactableName = "Select";
                interact.button = VRInteractable.Buttons.Trigger;
                interact.radius = 0;
                interact.useRect = true;
                interact.rect.center = Vector3.zero;
                interact.rect.extents = new Vector3(10, 10, 20);
                interact.requireMotion = false;
                interact.toggle = false;
                interact.tapOrHold = false;
                interact.sqrRadius = 0;
                interact.OnInteract = new UnityEvent();
                interact.OnInteract.AddListener(f45.GetComponentInChildren<MFDPortalManager>().PlayInputSound);
                interact.OnInteract.AddListener(delegate { moveCursor(__instance, interact.gameObject); });
             
                VRIHoverToggle hoverToggle = __result.gameObject.AddComponent<VRIHoverToggle>();
                hoverToggle.hoverObj = contactHvrObj;
                __result.gameObject.SetActive(true);
            }


        }

        private static void moveCursor(MFDPTacticalSituationDisplay tsd, GameObject buttonObject)
        {
            Vector3 targetPos = buttonObject.transform.position;
            targetPos = ClampedSelectorPosition(targetPos, tsd.selectorTf);
            tsd.selectorTf.position = targetPos;

            TSDContactIcon snapIcon = (TSDContactIcon)tsdTraverse.Field("snapIcon").GetValue();
            if(snapIcon != buttonObject.GetComponent<TSDContactIcon>())
            {
                tsdTraverse.Method("UnSnapCursor").GetValue();
            }


            if((TSDContactIcon)tsdTraverse.Field("snapIcon").GetValue() != null)
            {
                tsdTraverse.Method("M_OnInputButtonDown").GetValue();
                return;
            }
            tsdTraverse.Field("snapIcon").SetValue(buttonObject.GetComponent<TSDContactIcon>());
            tsdTraverse.Method("M_OnInputButtonDown").GetValue();
        }

        private static Vector3 ClampedSelectorPosition(Vector3 pos, Transform selectorTf)
        {
            RectTransform rectTransform = (RectTransform)selectorTf.parent;
            pos.x = Mathf.Clamp(pos.x, -rectTransform.rect.width / 2f, rectTransform.rect.width / 2f);
            pos.y = Mathf.Clamp(pos.y, -rectTransform.rect.height / 2f, rectTransform.rect.height / 2f);
            return pos;
        }
        private static void selectTarget()
        {

            tsdTraverse.Method("M_OnInputButtonDown").GetValue();
        }

        public static GameObject GetChildWithName(GameObject obj, string name)
        {


            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == name || child.name.Contains(name + "(clone"))
                {
                    return child.gameObject;
                }
            }


            return null;

        }

        private static GameObject selectedTarget;
        private TacticalSituationController controller;
        private static Traverse tsdTraverse;
    }
}






