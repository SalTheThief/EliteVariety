using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;
using R2API.Networking.Interfaces;
using R2API.Networking;
using System.Collections.Generic;
using RoR2.Orbs;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;
using RoR2.Audio;
using R2API;

namespace EliteVariety.Buffs
{
    public class AffixImpPlane : BaseBuff
    {
        public static GameObject stareCameraEffect;
        public static GameObject stareColliderPrefab;
        public static NetworkSoundEventDef impaleSound;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/ImpPlane/BuffIcon.png");
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixImpPlane";
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            On.RoR2.CharacterBody.Awake += CharacterBody_Awake;

            stareCameraEffect = R2API.PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/ImpPlane/StareCameraEffect.prefab"), Main.TokenPrefix + "ImpPlaneStareCameraEffect", false);
            LocalCameraEffect localCameraEffect = stareCameraEffect.AddComponent<LocalCameraEffect>();
            localCameraEffect.effectRoot = stareCameraEffect.transform.Find("CameraEffect").gameObject;

            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = stareCameraEffect.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            PostProcessDuration ppFadeOut = stareCameraEffect.AddComponent<PostProcessDuration>();
            ppFadeOut.ppVolume = stareCameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>();
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
            DestroyOnTimer destroyOnTimer = stareCameraEffect.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 1f;
            destroyOnTimer.enabled = false;
            tempVFX.exitBehaviours = new MonoBehaviour[]
            {
                ppFadeOut,
                destroyOnTimer
            };
            PostProcessDuration ppFadeIn = stareCameraEffect.AddComponent<PostProcessDuration>();
            ppFadeIn.ppVolume = stareCameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>();
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
                prefab = stareCameraEffect,
                condition = (x) => {
                    EliteVarietyAffixImpPlaneStareController component = x.GetComponent<EliteVarietyAffixImpPlaneStareController>();
                    if (component) return component.staring;
                    return false;
                },
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });

            stareColliderPrefab = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/ImpPlane/StareCollider.prefab"), Main.TokenPrefix + "StareCollider", false);
            stareColliderPrefab.transform.Find("Collider").gameObject.AddComponent<MysticsRisky2UtilsColliderTriggerList>();

            impaleSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            impaleSound.eventName = "EliteVariety_Play_erythrite_impale";
            EliteVarietyContent.Resources.networkSoundEventDefs.Add(impaleSound);
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            buffDef.eliteDef = EliteVarietyContent.Elites.ImpPlane;
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && victimInfo.body && attackerInfo.body.HasBuff(buffDef))
            {
                bool hadBuff = victimInfo.body.HasBuff(Buffs.ImpPlaneImpaled.dotDef.associatedBuff);
                DotController.InflictDot(victimInfo.gameObject, attackerInfo.gameObject, Buffs.ImpPlaneImpaled.dotIndex, 60f * damageInfo.procCoefficient, 1f);
                if (victimInfo.body.HasBuff(Buffs.ImpPlaneImpaled.dotDef.associatedBuff) && !hadBuff)
                {
                    EntitySoundManager.EmitSoundServer(impaleSound.index, victimInfo.gameObject);
                }
            }
        }

        private void CharacterBody_Awake(On.RoR2.CharacterBody.orig_Awake orig, CharacterBody self)
        {
            orig(self);
            self.gameObject.AddComponent<EliteVarietyAffixImpPlaneStareController>();
        }

        public class EliteVarietyAffixImpPlaneStareController : MonoBehaviour
        {
            public CharacterBody body;
            public InputBankTest inputBank;
            public float stareCheckTimer = 0f;
            public float stareCheckTimerDuration = 0.1f;
            public float buffBuildupTimer = 0f;
            public float buffBuildupDuration = 1f;
            public float buffDuration = 1.5f;
            public float buffDecayDuration = 0.3f;
            public bool staring = false;
            public GameObject colliderObject;
            public MysticsRisky2UtilsColliderTriggerList colliderTriggerList;
            public Transform colliderTransform;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
                inputBank = GetComponent<InputBankTest>();

                colliderObject = Object.Instantiate(stareColliderPrefab);
                colliderTransform = colliderObject.transform;
                colliderTriggerList = colliderObject.GetComponentInChildren<MysticsRisky2UtilsColliderTriggerList>();
                AdjustCollider(10f, 1000f);
            }

            public void AdjustCollider(float width, float length)
            {
                colliderTransform.localScale = new Vector3(width, width, length);
            }

            public void FixedUpdate()
            {
                if (body)
                {
                    stareCheckTimer -= Time.fixedDeltaTime;
                    if (stareCheckTimer <= 0f)
                    {
                        stareCheckTimer = stareCheckTimerDuration;

                        staring = IsStaringAtImpPlaneElite();
                    }

                    if (staring)
                    {
                        buffBuildupTimer += Time.fixedDeltaTime;
                        if (buffBuildupTimer >= buffBuildupDuration)
                        {
                            buffBuildupTimer = 0f;

                            if (NetworkServer.active)
                            {
                                BuffDef buffDef = EliteVarietyContent.Buffs.ImpPlaneStare;
                                int c = 1;
                                foreach (CharacterBody.TimedBuff timedBuff in body.timedBuffs)
                                {
                                    if (timedBuff.buffIndex == buffDef.buffIndex)
                                    {
                                        timedBuff.timer = buffDuration + buffDecayDuration * c;
                                        c++;
                                    }
                                }
                                body.AddTimedBuff(buffDef, buffDuration);
                            }
                        }
                    }
                    else
                    {
                        buffBuildupTimer = Mathf.Max(buffBuildupTimer - Time.fixedDeltaTime, 0f);
                    }
                }
            }

            public bool IsStaringAtImpPlaneElite()
            {
                if (inputBank)
                {
                    Ray aimRay = inputBank.GetAimRay();
                    aimRay = CameraRigController.ModifyAimRayIfApplicable(aimRay, gameObject, out _);

                    colliderTransform.position = aimRay.origin;
                    colliderTransform.rotation = Util.QuaternionSafeLookRotation(aimRay.direction, Vector3.up);

                    foreach (Collider collider in colliderTriggerList.RetrieveList())
                    {
                        CharacterBody characterBody = collider.GetComponent<CharacterBody>();
                        if (characterBody && characterBody != body && characterBody.HasBuff(EliteVarietyContent.Buffs.AffixImpPlane))
                        {
                            if (TeamComponent.GetObjectTeam(characterBody.gameObject) != TeamComponent.GetObjectTeam(gameObject))
                            {
                                if (characterBody.healthComponent && characterBody.healthComponent.alive)
                                {
                                    if (!Physics.Linecast(aimRay.origin, collider.transform.position, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                                        return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            public void OnDestroy()
            {
                Object.Destroy(colliderObject);
            }
        }
    }
}
