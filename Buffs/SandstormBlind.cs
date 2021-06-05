using UnityEngine;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;
using RoR2;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;

namespace EliteVariety.Buffs
{
    public class SandstormBlind : BaseBuff
    {
        public static float maxVisionRadius = 15f;
        public static GameObject cameraEffect;

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "SandstormBlind";
            buffDef.canStack = false;
            buffDef.isDebuff = true;
            buffDef.buffColor = new Color32(255, 196, 114, 255);

            On.RoR2.CharacterBody.GetVisibilityLevel_CharacterBody += CharacterBody_GetVisibilityLevel_CharacterBody;
            On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += CharacterBody_GetVisibilityLevel_TeamIndex;

            cameraEffect = R2API.PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Misc/SandstormBlindEffect.prefab"), Main.TokenPrefix + "SandstormBlindEffect", false);
            LocalCameraEffect localCameraEffect = cameraEffect.AddComponent<LocalCameraEffect>();
            localCameraEffect.effectRoot = cameraEffect.transform.Find("CameraEffect").gameObject;
            RampFog rampFog = cameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>().sharedProfile.AddSettings<RampFog>();
            rampFog.enabled.Override(true);
            rampFog.fogIntensity.Override(1f);
            rampFog.fogPower.Override(1f);
            rampFog.fogZero.Override(0f);
            rampFog.fogOne.Override(maxVisionRadius * 0.001f);
            rampFog.fogHeightIntensity.Override(0f);
            rampFog.fogHeightStart.Override(-maxVisionRadius * 0.5f);
            rampFog.fogHeightEnd.Override(maxVisionRadius * 0.5f);
            rampFog.fogColorStart.Override(new Color32(255, 234, 206, 0));
            rampFog.fogColorMid.Override(new Color32(255, 221, 178, 127));
            rampFog.fogColorEnd.Override(new Color32(255, 196, 114, 255));

            cameraEffect.transform.Find("CameraEffect/PP").gameObject.layer = LayerIndex.postProcess.intVal;

            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = cameraEffect.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            PostProcessDuration ppFadeOut = cameraEffect.AddComponent<PostProcessDuration>();
            ppFadeOut.ppVolume = cameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>();
            ppFadeOut.ppWeightCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(1f, 0f)
                }
            };
            ppFadeOut.maxDuration = 0.5f;
            ppFadeOut.destroyOnEnd = false;
            ppFadeOut.enabled = false;
            DestroyOnTimer destroyOnTimer = cameraEffect.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 1f;
            destroyOnTimer.enabled = false;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                ppFadeOut,
                destroyOnTimer
            };
            PostProcessDuration ppFadeIn = cameraEffect.AddComponent<PostProcessDuration>();
            ppFadeIn.ppVolume = cameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>();
            ppFadeIn.ppWeightCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, 1f)
                }
            };
            ppFadeIn.maxDuration = 0.2f;
            ppFadeIn.destroyOnEnd = false;
            ppFadeIn.enabled = false;
            tempVFX.enterBehaviours = new MonoBehaviour[]
            {
                ppFadeIn
            };
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = cameraEffect,
                condition = (x) => x.HasBuff(buffDef),
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }

        private RoR2.VisibilityLevel CharacterBody_GetVisibilityLevel_CharacterBody(On.RoR2.CharacterBody.orig_GetVisibilityLevel_CharacterBody orig, RoR2.CharacterBody self, RoR2.CharacterBody observer)
        {
            if (observer.HasBuff(buffDef) && Vector3.Distance(observer.corePosition, self.corePosition) > maxVisionRadius) return RoR2.VisibilityLevel.Invisible;
            return orig(self, observer);
        }

        private VisibilityLevel CharacterBody_GetVisibilityLevel_TeamIndex(On.RoR2.CharacterBody.orig_GetVisibilityLevel_TeamIndex orig, CharacterBody self, TeamIndex observerTeam)
        {
            var teamMembers = TeamComponent.GetTeamMembers(observerTeam);
            if (teamMembers.Count > 0)
            {
                bool atLeastOneTeamMemberCanSee = false;
                foreach (TeamComponent teamMember in teamMembers)
                {
                    if (teamMember.body && (!teamMember.body.isPlayerControlled || LocalUserManager.localUsersList.Any(x => x.cachedBody && x.cachedBody == teamMember.body)))
                    {
                        if (teamMember.body.HasBuff(buffDef))
                        {
                            if (Vector3.Distance(teamMember.body.corePosition, self.corePosition) <= maxVisionRadius) atLeastOneTeamMemberCanSee = true;
                        }
                        else atLeastOneTeamMemberCanSee = true;
                    }
                    if (atLeastOneTeamMemberCanSee) break;
                }
                if (!atLeastOneTeamMemberCanSee) return VisibilityLevel.Invisible;
            }
            return orig(self, observerTeam);
        }
    }
}