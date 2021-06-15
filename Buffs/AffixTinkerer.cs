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

namespace EliteVariety.Buffs
{
    public class AffixTinkerer : BaseBuff
    {
        public static DeployableSlot deployableSlot;
        public static GameObject linkLinePrefab;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Tinkerer/BuffIcon.png");
        }

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            linkLinePrefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "AffixTinkererLinkLine", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "AffixTinkerer";
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            NetworkingAPI.RegisterMessageType<EliteVarietyAffixTinkererBehavior.EliteVarietyAffixTinkererRecipientBehavior.SyncDroneStatBonus>();
            NetworkingAPI.RegisterMessageType<EliteVarietyAffixTinkererLinkLine.SyncSetTargets>();

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Tinkerer/TinkererDroneLinkLine.prefab"), linkLinePrefab);
            EliteVarietyAffixTinkererLinkLine linkLineComponent = linkLinePrefab.AddComponent<EliteVarietyAffixTinkererLinkLine>();
            linkLineComponent.startVelocity = new Vector3(0f, 15f, 0f);
            linkLineComponent.endVelocity = new Vector3(0f, -5f, 0f);

            deployableSlot = R2API.DeployableAPI.RegisterDeployableSlot(GetTinkererDroneDeployableSameSlotLimit);
        }

        public static int GetTinkererDroneDeployableSameSlotLimit(CharacterMaster self, int deployableCountMultiplier)
        {
            return 2 + (int)(self.hasBody ? self.GetBody().radius : 0) * deployableCountMultiplier;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            buffDef.eliteDef = EliteVarietyContent.Elites.Tinkerer;
        }

        public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                EliteVarietyAffixTinkererBehavior component = self.AddItemBehavior<EliteVarietyAffixTinkererBehavior>(self.HasBuff(buffDef) ? 1 : 0);
                if (component) component.RecalculateDroneStatBonus();
            }
        }

        public class EliteVarietyAffixTinkererBehavior : CharacterBody.ItemBehavior
        {
            private List<CharacterMaster> _droneMasters;
            public List<CharacterMaster> droneMasters {
                get
                {
                    _droneMasters.RemoveAll(x => !x);
                    return _droneMasters;
                }
                set
                {
                    _droneMasters = value;
                }
            }
            public DeployableMinionSpawner droneSpawner;
            public int droneStatBonus = 0;
            public Dictionary<Inventory, List<StolenItemInfo>> stealDictionary;
            public List<ItemTransferOrb> orbsInFlight;
            private List<EliteVarietyAffixTinkererLinkLine> _linkLines;
            public List<EliteVarietyAffixTinkererLinkLine> linkLines
            {
                get
                {
                    _linkLines.RemoveAll(x => !x);
                    return _linkLines;
                }
                set
                {
                    _linkLines = value;
                }
            }
            public int quickerRespawns = 0;
            public float quickerRespawnStopwatch = 0f;
            public float quickerRespawnDuration = 0.3f;

            public class StolenItemInfo
            {
                public ItemIndex itemIndex;
                public int count = 0;
                public NetworkIdentity ownerBodyNetworkIdentity;
            }

            public void Start()
            {
                stealDictionary = new Dictionary<Inventory, List<StolenItemInfo>>();
                orbsInFlight = new List<ItemTransferOrb>();
                droneMasters = new List<CharacterMaster>();
                linkLines = new List<EliteVarietyAffixTinkererLinkLine>();
            }

            public void FixedUpdate()
            {
                if (NetworkServer.active && isActiveAndEnabled)
                {
                    if (body.master)
                    {
                        if (droneSpawner == null)
                        {
                            droneSpawner = new DeployableMinionSpawner(body.master, deployableSlot, new Xoroshiro128Plus(Run.instance.seed ^ (ulong)GetInstanceID()))
                            {
                                respawnInterval = 60f,
                                spawnCard = MysticsRisky2Utils.BaseAssetTypes.BaseCharacterMaster.characterSpawnCards.Find(x => x.name == "EliteVariety_cscTinkererDrone")
                            };
                            droneSpawner.onMinionSpawnedServer += OnMinionSpawnedServer;

                            droneSpawner.respawnStopwatch = 0f;
                            quickerRespawns = body.master.GetDeployableSameSlotLimit(deployableSlot);
                        }
                        else
                        {
                            if (quickerRespawns > 0)
                            {
                                quickerRespawnStopwatch += Time.fixedDeltaTime;
                                if (quickerRespawnStopwatch >= quickerRespawnDuration)
                                {
                                    quickerRespawnStopwatch = 0f;
                                    quickerRespawns--;
                                    droneSpawner.SpawnMinion(droneSpawner.spawnCard, body);
                                }
                            }
                        }
                    }
                }
            }

            public void RecalculateDroneStatBonus()
            {
                int oldDroneStatBonus = droneStatBonus;
                droneStatBonus = 0;
                Inventory myInventory = body.inventory;
                if (myInventory)
                {
                    foreach (ItemIndex itemIndex in ItemCatalog.allItems)
                    {
                        ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                        if (itemDef.ContainsTag(ItemTag.Scrap))
                        {
                            int scrapPower = 0;
                            switch (itemDef.tier)
                            {
                                case ItemTier.Tier1:
                                    scrapPower = 1;
                                    break;
                                case ItemTier.Tier2:
                                case ItemTier.Boss:
                                    scrapPower = 2;
                                    break;
                                case ItemTier.Tier3:
                                    scrapPower = 3;
                                    break;
                            }
                            if (scrapPower != 0) droneStatBonus += myInventory.GetItemCount(itemDef) * scrapPower;
                        }
                    }
                }
                if (oldDroneStatBonus != droneStatBonus)
                {
                    foreach (CharacterMaster droneMaster in droneMasters)
                    {
                        EliteVarietyAffixTinkererRecipientBehavior component = droneMaster.GetComponent<EliteVarietyAffixTinkererRecipientBehavior>();
                        if (component)
                        {
                            component.droneStatBonus = droneStatBonus;
                        }
                        CharacterBody droneBody = droneMaster.GetBody();
                        if (droneBody) Utils.ForceRecalculateStats(droneBody);
                    }
                }
            }

            public void OnMinionSpawnedServer(SpawnCard.SpawnResult obj)
            {
                GameObject spawnedInstance = obj.spawnedInstance;
                if (spawnedInstance)
                {
                    CharacterMaster droneMaster = spawnedInstance.GetComponent<CharacterMaster>();
                    droneMasters.Add(droneMaster);
                    if (droneMaster && droneMaster.inventory)
                    {
                        droneMaster.inventory.GiveItem(EliteVarietyContent.Items.TinkererDroneStatBonus);
                        droneMaster.gameObject.AddComponent<EliteVarietyAffixTinkererRecipientBehavior>();
                    }
                    droneMaster.onBodyStart += DroneMaster_onBodyStart;
                }
            }

            public void DroneMaster_onBodyStart(CharacterBody droneBody)
            {
                GameObject linkLine = Object.Instantiate(linkLinePrefab);
                EliteVarietyAffixTinkererLinkLine linkLineComponent = linkLine.GetComponent<EliteVarietyAffixTinkererLinkLine>();
                linkLineComponent.SetTargets(
                    body.mainHurtBox ? body.mainHurtBox.transform : null,
                    droneBody.mainHurtBox ? droneBody.mainHurtBox.transform : null
                );
                linkLines.Add(linkLineComponent);
            }

            public class EliteVarietyAffixTinkererRecipientBehavior : MonoBehaviour
            {
                private int _droneStatBonus = 0;
                public int droneStatBonus
                {
                    get
                    {
                        return _droneStatBonus;
                    }
                    set
                    {
                        if (_droneStatBonus != value)
                        {
                            _droneStatBonus = value;
                            if (NetworkServer.active) new SyncDroneStatBonus(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                        }
                    }
                }

                public class SyncDroneStatBonus : INetMessage
                {
                    NetworkInstanceId objID;
                    int droneStatBonus;

                    public SyncDroneStatBonus()
                    {
                    }

                    public SyncDroneStatBonus(NetworkInstanceId objID, int droneStatBonus)
                    {
                        this.objID = objID;
                        this.droneStatBonus = droneStatBonus;
                    }

                    public void Deserialize(NetworkReader reader)
                    {
                        objID = reader.ReadNetworkId();
                        droneStatBonus = reader.ReadInt32();
                    }

                    public void OnReceived()
                    {
                        if (NetworkServer.active) return;
                        GameObject obj = Util.FindNetworkObject(objID);
                        if (obj)
                        {
                            EliteVarietyAffixTinkererRecipientBehavior component = obj.GetComponent<EliteVarietyAffixTinkererRecipientBehavior>();
                            if (component)
                            {
                                component.droneStatBonus = droneStatBonus;
                            }
                        }
                    }

                    public void Serialize(NetworkWriter writer)
                    {
                        writer.Write(objID);
                        writer.Write(droneStatBonus);
                    }
                }
            }

            public void OnDestroy()
            {
                orbsInFlight.ForEach(x => OrbManager.instance.ForceImmediateArrival(x));

                Vector3 emitPosition = transform.position;
                if (body) emitPosition = body.corePosition;

                foreach (KeyValuePair<Inventory, List<StolenItemInfo>> keyValuePair in stealDictionary)
                {
                    if (keyValuePair.Key)
                    {
                        foreach (StolenItemInfo stolenItemInfo in keyValuePair.Value)
                        {
                            ItemTransferOrb item = ItemTransferOrb.DispatchItemTransferOrb(emitPosition, keyValuePair.Key, stolenItemInfo.itemIndex, stolenItemInfo.count, null, stolenItemInfo.ownerBodyNetworkIdentity);
                        }
                    }
                }

                foreach (CharacterMaster droneMaster in droneMasters)
                {
                    if (droneMaster.inventory)
                    {
                        droneMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, 10);
                    }
                }

                if (droneSpawner != null) droneSpawner.Dispose();
                droneSpawner = null;

                foreach (EliteVarietyAffixTinkererLinkLine linkLine in linkLines)
                {
                    Object.Destroy(linkLine.gameObject);
                }
            }

            public void StealFrom(Inventory inventory, Vector3 emitPosition, NetworkIdentity networkIdentity)
            {
                List<ItemIndex> itemsThatCanBeStolen = new List<ItemIndex>();
                foreach (ItemIndex itemIndex in ItemCatalog.allItems)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                    if (itemDef) {
                        if (itemDef.canRemove && itemDef.DoesNotContainTag(ItemTag.CannotSteal) && inventory.GetItemCount(itemIndex) > 0)
                        {
                            itemsThatCanBeStolen.Add(itemIndex);
                        }
                    }
                }
                if (itemsThatCanBeStolen.Count > 0)
                {
                    ItemIndex itemToSteal = RoR2Application.rng.NextElementUniform(itemsThatCanBeStolen);
                    ItemDef itemToStealDef = ItemCatalog.GetItemDef(itemToSteal);
                    int stealAmount = 1;
                    inventory.RemoveItem(itemToSteal, stealAmount);

                    ItemDef scrap = RoR2Content.Items.ScrapWhite;
                    switch (itemToStealDef.tier)
                    {
                        case ItemTier.Tier1:
                            scrap = RoR2Content.Items.ScrapWhite;
                            break;
                        case ItemTier.Tier2:
                            scrap = RoR2Content.Items.ScrapGreen;
                            break;
                        case ItemTier.Boss:
                            scrap = RoR2Content.Items.ScrapYellow;
                            break;
                        case ItemTier.Tier3:
                            scrap = RoR2Content.Items.ScrapRed;
                            break;
                    }
                    ItemIndex scrapIndex = scrap.itemIndex;

                    if (!stealDictionary.ContainsKey(inventory)) stealDictionary.Add(inventory, new List<StolenItemInfo>());
                    StolenItemInfo stolenItemInfo = stealDictionary[inventory].FirstOrDefault(x => x.itemIndex == itemToSteal);
                    if (stolenItemInfo == null)
                    {
                        stolenItemInfo = new StolenItemInfo
                        {
                            itemIndex = itemToSteal,
                            ownerBodyNetworkIdentity = networkIdentity
                        };
                        stealDictionary[inventory].Add(stolenItemInfo);
                    }
                    stolenItemInfo.count += stealAmount;

                    ItemTransferOrb item = ItemTransferOrb.DispatchItemTransferOrb(emitPosition, null, itemToSteal, stealAmount, (orb) =>
                    {
                        body.inventory.GiveItem(scrap, stealAmount);
                        orbsInFlight.Remove(orb);

                        foreach (CharacterMaster droneMaster in droneMasters)
                        {
                            CharacterBody droneBody = droneMaster.GetBody();
                            if (droneBody)
                            {
                                ItemTransferOrb item2 = ItemTransferOrb.DispatchItemTransferOrb(body.corePosition, null, scrapIndex, stealAmount, (orb2) =>
                                {
                                    orbsInFlight.Remove(orb2);
                                }, droneBody.networkIdentity);
                            }
                        }
                    }, body.networkIdentity);
                    orbsInFlight.Add(item);
                }
            }
        }

        public class EliteVarietyAffixTinkererLinkLine : MonoBehaviour
        {
            public Transform target1;
            public Transform target2;
            public NetworkInstanceId target1NetId = NetworkInstanceId.Invalid;
            public NetworkInstanceId target2NetId = NetworkInstanceId.Invalid;
            public Vector3 startVelocity = Vector3.zero;
            public Vector3 endVelocity = Vector3.zero;
            public LineRenderer line;

            public void Awake()
            {
                line = GetComponent<LineRenderer>();
            }

            public void SetTargets(Transform target1, Transform target2)
            {
                this.target1 = target1;
                this.target2 = target2;
                
                if (NetworkServer.active)
                {
                    NetworkInstanceId GetNetId(Transform from)
                    {
                        if (from)
                        {
                            NetworkIdentity networkIdentity = from.GetComponent<NetworkIdentity>();
                            if (networkIdentity) return networkIdentity.netId;
                        }
                        return NetworkInstanceId.Invalid;
                    }
                    new SyncSetTargets(gameObject.GetComponent<NetworkIdentity>().netId, GetNetId(target1), GetNetId(target2)).Send(NetworkDestination.Clients);
                }
            }

            public class SyncSetTargets : INetMessage
            {
                NetworkInstanceId objID;
                NetworkInstanceId target1NetId;
                NetworkInstanceId target2NetId;

                public SyncSetTargets()
                {
                }

                public SyncSetTargets(NetworkInstanceId objID, NetworkInstanceId target1NetId, NetworkInstanceId target2NetId)
                {
                    this.objID = objID;
                    this.target1NetId = target1NetId;
                    this.target2NetId = target2NetId;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    target1NetId = reader.ReadNetworkId();
                    target2NetId = reader.ReadNetworkId();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    NetworkHelper.EnqueueOnSpawnedOnClientEvent(objID, (gameObject) =>
                    {
                        EliteVarietyAffixTinkererLinkLine component = gameObject.GetComponent<EliteVarietyAffixTinkererLinkLine>();
                        if (component)
                        {
                            component.target1NetId = target1NetId;
                            component.target2NetId = target2NetId;
                        }
                    });
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(target1NetId);
                    writer.Write(target2NetId);
                }
            }

            public void Update()
            {
                ResolveTargetNetId(ref target1, ref target1NetId);
                ResolveTargetNetId(ref target2, ref target2NetId);
                if (target1 && target2)
                {
                    if (!line.enabled) line.enabled = true;
                    int vertexCount = line.positionCount;
                    for (var i = 0; i < vertexCount; i++)
                    {
                        float progress = (float)i / (float)vertexCount;
                        Vector3 vertexPos = Vector3.Lerp(target1.position + startVelocity * progress, target2.position + endVelocity * (1f - progress), progress);
                        line.SetPosition(i, vertexPos);
                    }
                }
                else
                {
                    if (line.enabled) line.enabled = false;
                }
            }

            public static void ResolveTargetNetId(ref Transform target, ref NetworkInstanceId targetNetId)
            {
                if (!target && targetNetId != NetworkInstanceId.Invalid)
                {
                    GameObject target1Obj = Util.FindNetworkObject(targetNetId);
                    if (target1Obj)
                    {
                        target = target1Obj.transform;
                        targetNetId = NetworkInstanceId.Invalid;
                    }
                }
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
        {
            if (damageInfo.procCoefficient > 0 && attackerInfo.body && attackerInfo.master && victimInfo.body && victimInfo.inventory && attackerInfo.body.HasBuff(buffDef) && Util.CheckRoll(40f * damageInfo.procCoefficient, attackerInfo.master))
            {
                EliteVarietyAffixTinkererBehavior component = attackerInfo.body.GetComponent<EliteVarietyAffixTinkererBehavior>();
                if (component) component.StealFrom(victimInfo.inventory, victimInfo.body.corePosition, victimInfo.body.networkIdentity);
            }
        }
    }
}
