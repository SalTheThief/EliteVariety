using MysticsRisky2Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace EliteVariety.Buffs
{
    public class AffixBuffing : BaseBuff
    {
        public static GameObject buffAuraPrefab;
        public static NetworkSoundEventDef specialBuffTriggerSound;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Buffing/BuffIcon.png");
        }

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            buffAuraPrefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "AffixBuffingBuffAura", true);
            NetworkingAPI.RegisterMessageType<EliteVarietyAffixBuffingAura.SyncSpecialWardToggle>();
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixBuffing";
            On.RoR2.EliteCatalog.Init += (orig) =>
            {
                orig();
                buffDef.eliteDef = EliteVarietyContent.Elites.Buffing;
            };
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref BuffCatalog.eliteBuffIndices, buffDef.buffIndex);
            };
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Buffing/BuffAura.prefab"), buffAuraPrefab);
            EliteVarietyAffixBuffingAura auraComponent = buffAuraPrefab.AddComponent<EliteVarietyAffixBuffingAura>();
            auraComponent.rangeIndicator = buffAuraPrefab.transform.Find("RangeIndicator");
            NetworkedBodyAttachment networkedBodyAttachment = buffAuraPrefab.AddComponent<NetworkedBodyAttachment>();
            networkedBodyAttachment.shouldParentToAttachedBody = true;
            networkedBodyAttachment.forceHostAuthority = false;
            TeamFilter teamFilter = buffAuraPrefab.AddComponent<TeamFilter>();
            BuffWard AddBuffWard()
            {
                BuffWard newBuffWard = buffAuraPrefab.AddComponent<BuffWard>();
                newBuffWard.radius = 15f;
                newBuffWard.interval = 0.5f;
                newBuffWard.buffDuration = 0.7f;
                newBuffWard.floorWard = false;
                newBuffWard.expires = false;
                newBuffWard.invertTeamFilter = false;
                newBuffWard.animateRadius = false;
                newBuffWard.requireGrounded = false;
                newBuffWard.rangeIndicator = buffAuraPrefab.transform.Find("RangeIndicator");
                return newBuffWard;
            }
            BuffWard buffWardRegular = AddBuffWard();
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                buffWardRegular.buffDef = RoR2Content.Buffs.Warbanner;
            };
            auraComponent.regularWard = buffWardRegular;
            BuffWard buffWardSpecial = AddBuffWard();
            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();
                buffWardSpecial.buffDef = RoR2Content.Buffs.TeamWarCry;
            };
            auraComponent.specialWard = buffWardSpecial;
            buffWardSpecial.enabled = false;

            specialBuffTriggerSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            specialBuffTriggerSound.eventName = "Play_teamWarCry_activate";
            EliteVarietyContent.Resources.networkSoundEventDefs.Add(specialBuffTriggerSound);
        }

        public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<EliteVarietyAffixBuffingBehavior>(self.HasBuff(buffDef) ? 1 : 0);
        }

        public class EliteVarietyAffixBuffingAura : MonoBehaviour
        {
            public BuffWard regularWard;
            public BuffWard specialWard;
            public Transform rangeIndicator;

            public void SpecialWardToggle(bool enableSpecialWard)
            {
                regularWard.enabled = !enableSpecialWard;
                specialWard.enabled = enableSpecialWard;

                if (NetworkServer.active)
                {
                    new SyncSpecialWardToggle(gameObject.GetComponent<NetworkIdentity>().netId, enableSpecialWard).Send(NetworkDestination.Clients);
                }
            }

            public class SyncSpecialWardToggle : INetMessage
            {
                NetworkInstanceId objID;
                bool enableSpecialWard;

                public SyncSpecialWardToggle()
                {
                }

                public SyncSpecialWardToggle(NetworkInstanceId objID, bool enableSpecialWard)
                {
                    this.objID = objID;
                    this.enableSpecialWard = enableSpecialWard;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    enableSpecialWard = reader.ReadBoolean();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    GameObject obj = Util.FindNetworkObject(objID);
                    if (obj)
                    {
                        EliteVarietyAffixBuffingAura component = obj.GetComponent<EliteVarietyAffixBuffingAura>();
                        if (component)
                        {
                            component.SpecialWardToggle(enableSpecialWard);
                        }
                    }
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(enableSpecialWard);
                }
            }

            public void FixedUpdate()
            {
                if (rangeIndicator && !rangeIndicator.gameObject.activeSelf) rangeIndicator.gameObject.SetActive(true);
            }

            public void OnEnable()
            {
                if (rangeIndicator) rangeIndicator.gameObject.SetActive(true);
            }

            public void OnDisable()
            {
                if (rangeIndicator) rangeIndicator.gameObject.SetActive(false);
            }
        }

        public class EliteVarietyAffixBuffingBehavior : CharacterBody.ItemBehavior
        {
            public GameObject buffAura;
            public float specialWardDuration = 0f;
            public float baseRadius = 15f;
            public float extraRadius = 15f;
            public float extraRadiusDuration = 0f;
            public bool extraRadiusActive = false;
            public float effectDelayTimer = 0f;
            public EliteVarietyAffixBuffingAura auraComponent;

            public void FixedUpdate()
            {
                if (NetworkServer.active)
                {
                    if (!buffAura)
                    {
                        buffAura = Object.Instantiate<GameObject>(buffAuraPrefab);
                        buffAura.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                        foreach (BuffWard buffWard in buffAura.GetComponents<BuffWard>())
                        {
                            buffWard.Networkradius = baseRadius + body.radius;
                        }
                        auraComponent = buffAura.GetComponent<EliteVarietyAffixBuffingAura>();
                        buffAura.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                    }

                    bool flag = specialWardDuration > 0;
                    if (flag != auraComponent.specialWard.enabled)
                    {
                        auraComponent.SpecialWardToggle(flag);
                    }

                    flag = extraRadiusDuration > 0;
                    if (flag != extraRadiusActive)
                    {
                        extraRadiusActive = flag;
                        foreach (BuffWard buffWard in buffAura.GetComponents<BuffWard>())
                        {
                            buffWard.Networkradius = baseRadius + body.radius + (flag ? extraRadius : 0f);
                        }
                    }
                }

                specialWardDuration -= Time.fixedDeltaTime;
                extraRadiusDuration -= Time.fixedDeltaTime;
                effectDelayTimer -= Time.fixedDeltaTime;
            }

            public void TriggerSpecialWard(float duration)
            {
                if (effectDelayTimer <= 0f && duration > 0 && specialWardDuration <= 0)
                {
                    effectDelayTimer = 10f;

                    EntitySoundManager.EmitSoundServer(specialBuffTriggerSound.index, body.gameObject);

                    Vector3 corePosition = body.corePosition;
                    EffectData effectData = new EffectData
                    {
                        origin = corePosition
                    };
                    effectData.SetNetworkedObjectReference(body.gameObject);
                    EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/TeamWarCryActivation"), effectData, true);
                }
                specialWardDuration = Mathf.Max(duration, specialWardDuration);
            }

            public void TriggerExtraWardRadius(float duration)
            {
                extraRadiusDuration = Mathf.Max(duration, extraRadiusDuration);
            }


            public void OnDisable()
            {
                if (buffAura) Object.Destroy(buffAura);
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && attackerInfo.body.HasBuff(buffDef))
            {
                EliteVarietyAffixBuffingBehavior component = attackerInfo.body.GetComponent<EliteVarietyAffixBuffingBehavior>();
                if (component) component.TriggerSpecialWard(4f * damageInfo.procCoefficient);
            }
        }
    }
}
