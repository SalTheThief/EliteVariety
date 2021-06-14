using UnityEngine;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;
using RoR2;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using R2API;

namespace EliteVariety.Buffs
{
    public class ImpPlaneImpaled : BaseBuff
    {
        public static DotController.DotIndex dotIndex;
        public static DotController.DotDef dotDef;

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "ImpPlaneImpaled";
            buffDef.canStack = false;
            buffDef.isDebuff = true;
            buffDef.buffColor = new Color32(249, 0, 70, 255);

            dotDef = new DotController.DotDef
            {
                associatedBuff = buffDef,
                damageCoefficient = 5f,
                damageColorIndex = DamageColorIndex.Bleed,
                interval = 5f
            };
            dotIndex = DotAPI.RegisterDotDef(dotDef, new DotAPI.CustomDotBehaviour((self, dotStack) => {
                DotController.DotStack oldDotStack = self.dotStackList.FirstOrDefault(x => x.dotIndex == dotStack.dotIndex);
                if (oldDotStack != null)
                {
                    self.RemoveDotStackAtServer(self.dotStackList.IndexOf(dotStack));
                }
                dotStack.damage = Mathf.Min(self.victimHealthComponent.fullCombinedHealth * 0.2f, dotStack.damage);
            }));

            GameObject debuffedVFX = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/ImpPlane/ImpaledVFX.prefab"), Main.TokenPrefix + "ImpaledVFX", false);
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            tempVFX.rotateWithParent = true;
            GameObject vfxOrigin = debuffedVFX.transform.Find("Origin").gameObject;
            ObjectScaleCurve fadeOut = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeOut.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(-20f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 0f, Mathf.Tan(160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeOut.useOverallCurveOnly = true;
            fadeOut.enabled = false;
            fadeOut.timeMax = 0.3f;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                fadeOut
            };
            ObjectScaleCurve fadeIn = vfxOrigin.AddComponent<ObjectScaleCurve>();
            fadeIn.overallCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f, Mathf.Tan(180f * Mathf.Deg2Rad), Mathf.Tan(70f * Mathf.Deg2Rad)),
                    new Keyframe(1f, 1f, Mathf.Tan(-160f * Mathf.Deg2Rad), 0f)
                }
            };
            fadeIn.useOverallCurveOnly = true;
            fadeIn.enabled = false;
            fadeIn.timeMax = 0.3f;
            tempVFX.enterBehaviours = new MonoBehaviour[]
            {
                fadeIn
            };
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => x.HasBuff(buffDef),
                radius = CustomTempVFXManagement.DefaultBestFitRadiusCall,
                child = "Chest"
            });
        }
    }
}
