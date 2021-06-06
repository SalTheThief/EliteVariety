using Mono.Cecil.Cil;
using MonoMod.Cil;
using MysticsRisky2Utils;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Audio;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EliteVariety.Buffs
{
    public class AffixSandstorm : BaseBuff
    {
        public static GameObject sandstormPrefab;
        public static GameObject vehiclePrefab;

        public override Sprite LoadSprite(string assetName)
        {
            return Main.AssetBundle.LoadAsset<Sprite>("Assets/EliteVariety/Elites/Sandstorm/BuffIcon.png");
        }

        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            sandstormPrefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "AffixSandstormStorm", true);
            sandstormPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            vehiclePrefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "AffixSandstormVehicle", true);
        }

        public override void OnLoad()
		{
			base.OnLoad();
			buffDef.name = "AffixSandstorm";
			On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            GenericGameEvents.OnHitEnemy += GenericGameEvents_OnHitEnemy;

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Sandstorm/Sandstorm.prefab"), sandstormPrefab);
            NetworkedBodyAttachment networkedBodyAttachment = sandstormPrefab.AddComponent<NetworkedBodyAttachment>();
            networkedBodyAttachment.shouldParentToAttachedBody = true;
            networkedBodyAttachment.forceHostAuthority = false;
            TeamFilter teamFilter = sandstormPrefab.AddComponent<TeamFilter>();
            EliteVarietySandstormBehavior sandstormBehavior = sandstormPrefab.AddComponent<EliteVarietySandstormBehavior>();
            sandstormBehavior.colliderListComponent = sandstormPrefab.transform.Find("Collision").gameObject.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsColliderTriggerList>();
            sandstormBehavior.indicator = sandstormPrefab.transform.Find("Indicator");
            sandstormBehavior.collision = sandstormPrefab.transform.Find("Collision");
            sandstormPrefab.transform.Find("Collision").gameObject.layer = LayerIndex.world.intVal;
            sandstormPrefab.gameObject.layer = LayerIndex.fakeActor.intVal;

            DestroyOnTimer destroyOnTimer = sandstormPrefab.transform.Find("Indicator").gameObject.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 10f;
            destroyOnTimer.enabled = false;

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Sandstorm/Vehicle.prefab"), vehiclePrefab);
            vehiclePrefab.AddComponent<NetworkTransform>();
            VehicleSeat vehicleSeat = vehiclePrefab.AddComponent<VehicleSeat>();
            vehicleSeat.passengerState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterVehicleSeated));
            vehicleSeat.seatPosition = vehiclePrefab.transform.Find("SeatPosition");
            vehicleSeat.exitPosition = vehiclePrefab.transform.Find("SeatPosition");
            vehicleSeat.ejectOnCollision = false;
            vehicleSeat.hidePassenger = true;
            vehicleSeat.exitVelocityFraction = 0.5f;
            vehicleSeat.enterVehicleContextString = "";
            vehicleSeat.exitVehicleContextString = "ELITEVARIETY_SANDSTORM_VEHICLE_EXIT_CONTEXT";
            vehicleSeat.disablePassengerMotor = false;
            vehicleSeat.isEquipmentActivationAllowed = true;

            CameraTargetParams cameraTargetParams = vehiclePrefab.AddComponent<CameraTargetParams>();
            CharacterCameraParams characterCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            characterCameraParams.name = Main.TokenPrefix + "ccpSandstormVehicle";
            characterCameraParams.minPitch = -30f;
            characterCameraParams.maxPitch = 30f;
            characterCameraParams.wallCushion = 0f;
            characterCameraParams.pivotVerticalOffset = 1f;
            characterCameraParams.standardLocalCameraPos = new Vector3(0f, 2f, -40f);
            cameraTargetParams.cameraParams = characterCameraParams;
            cameraTargetParams.cameraPivotTransform = vehiclePrefab.transform.Find("CameraPivot");
            cameraTargetParams.fovOverride = -1;

            vehiclePrefab.AddComponent<EliteVarietySandstormVehicleBehavior>();

            NetworkingAPI.RegisterMessageType<EliteVarietySandstormBehavior.SyncRadius>();

            SceneCamera.onSceneCameraPreRender += EliteVarietySandstormBehavior.OnSceneCameraPreRender;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            buffDef.eliteDef = EliteVarietyContent.Elites.Sandstorm;
        }

        public void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active) self.AddItemBehavior<EliteVarietyAffixSandstormBehavior>(self.HasBuff(buffDef) && self.healthComponent && self.healthComponent.alive ? 1 : 0);
        }

        public class EliteVarietyAffixSandstormBehavior : CharacterBody.ItemBehavior
        {
            public GameObject sandstormObject;
            public EliteVarietySandstormBehavior sandstormBehavior;

            public void FixedUpdate()
            {
                if (!sandstormObject)
                {
                    sandstormObject = Object.Instantiate(sandstormPrefab);
                    sandstormBehavior = sandstormObject.GetComponent<EliteVarietySandstormBehavior>();
                    sandstormBehavior.radius = 10f + body.bestFitRadius;
                    sandstormObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(gameObject);
                    sandstormObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                }
            }

            public void OnDisable()
            {
                if (sandstormBehavior) sandstormBehavior.SelfDestruct();
            }

            public void Dash()
            {
                if (sandstormBehavior)
                {
                    sandstormBehavior.Dash();
                }
            }
        }

        public class EliteVarietySandstormBehavior : MonoBehaviour, INetworkedBodyAttachmentListener
        {
            public NetworkedBodyAttachment networkedBodyAttachment;
            public TeamFilter teamFilter;

            public Transform indicator;
            public Transform collision;

            public float damage = 0.1f;
            public float dashDamage = 0.2f;
            public float procCoefficient = 0.1f;
            public float dashProcCoefficient = 1f;
            public Vector3 force = Vector3.zero;
            public Vector3 dashForce = Vector3.up * 1000f;
            public float tickFrequency = 10f;
            public float tickStopwatch = 0f;
            private float _radius = 0f;
            public float radius {
                get
                {
                    return _radius;
                }
                set
                {
                    _radius = value;
                    if (NetworkServer.active) new SyncRadius(gameObject.GetComponent<NetworkIdentity>().netId, value).Send(NetworkDestination.Clients);
                }
            }
            public float indicatorScaleVelocity;
            public MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsColliderTriggerList colliderListComponent;
            public VehicleSeat vehicleSeat;
            public bool dashActive
            {
                get
                {
                    return vehicleSeat && vehicleSeat.hasPassenger && networkedBodyAttachment && vehicleSeat.currentPassengerBody == networkedBodyAttachment.attachedBody;
                }
            }
            public CameraTargetParams.AimRequest aimRequest;
            public List<CharacterBody> bodiesHitThisTick;
            public MaterialPropertyBlock materialPropertyBlock;
            public List<IndicatorRendererRecolorInfo> indicatorRendererRecolorInfos;

            public struct IndicatorRendererRecolorInfo
            {
                public Renderer renderer;
                public Color originalColor;
                public Color changedColor;
            }

            public void Awake()
            {
                networkedBodyAttachment = GetComponent<NetworkedBodyAttachment>();
                teamFilter = GetComponent<TeamFilter>();
                bodiesHitThisTick = new List<CharacterBody>();
                materialPropertyBlock = new MaterialPropertyBlock();
                indicatorRendererRecolorInfos = new List<IndicatorRendererRecolorInfo>();
            }

            public void Start()
            {
                foreach (Renderer renderer in indicator.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.material)
                    {
                        Color originalColor = renderer.material.color;
                        Color changedColor = renderer.material.color;
                        changedColor.a *= 0.1f;
                        indicatorRendererRecolorInfos.Add(new IndicatorRendererRecolorInfo
                        {
                            renderer = renderer,
                            originalColor = originalColor,
                            changedColor = changedColor
                        });
                    }
                }
            }

            public void FixedUpdate()
            {
                if (indicator)
                {
                    float num = Mathf.SmoothDamp(indicator.localScale.x, radius, ref indicatorScaleVelocity, 0.05f);
                    indicator.localScale = new Vector3(num, num, num);
                }
                if (collision)
                {
                    collision.localScale = new Vector3(radius, radius, radius);
                }

                tickStopwatch += Time.fixedDeltaTime;
                if (tickStopwatch >= (1f / tickFrequency / (networkedBodyAttachment.attachedBody ? networkedBodyAttachment.attachedBody.attackSpeed : 1f)))
                {
                    tickStopwatch = 0f;
                    Tick();
                }
            }

            public void Tick()
            {
                if (!NetworkServer.active || !networkedBodyAttachment || !networkedBodyAttachment.attachedBody || !networkedBodyAttachment.attachedBodyObject) return;

                float currentTickDamage = (!dashActive ? damage : dashDamage) * networkedBodyAttachment.attachedBody.damage;
                bool currentTickCrit = networkedBodyAttachment.attachedBody.RollCrit();
                Vector3 currentTickForce = !dashActive ? force : dashForce;
                Vector3 alignedForce = transform.forward * currentTickForce.x + transform.up * currentTickForce.y + transform.right * currentTickForce.z;
                bodiesHitThisTick.Clear();
                foreach (Collider collider in colliderListComponent.RetrieveList())
                {
                    CharacterBody characterBody = collider.GetComponent<CharacterBody>();
                    if (characterBody && !bodiesHitThisTick.Contains(characterBody))
                    {
                        bodiesHitThisTick.Add(characterBody);
                        if (TeamComponent.GetObjectTeam(characterBody.gameObject) != teamFilter.teamIndex)
                        {
                            if (characterBody.healthComponent && characterBody.healthComponent.alive)
                            {
                                DamageInfo damageInfo = new DamageInfo
                                {
                                    attacker = networkedBodyAttachment.attachedBody.gameObject,
                                    inflictor = gameObject,
                                    position = transform.position,
                                    crit = currentTickCrit,
                                    damage = currentTickDamage,
                                    damageColorIndex = DamageColorIndex.Default,
                                    force = Vector3.zero,
                                    procCoefficient = (!dashActive ? procCoefficient : dashProcCoefficient),
                                    damageType = DamageType.Generic,
                                    procChainMask = default(ProcChainMask)
                                };
                                characterBody.healthComponent.TakeDamage(damageInfo);
                                if (alignedForce != Vector3.zero) characterBody.healthComponent.TakeDamageForce(alignedForce, true, true);
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, characterBody.gameObject);
                                GlobalEventManager.instance.OnHitAll(damageInfo, characterBody.gameObject);
                            }
                        }
                    }
                }
            }

            public void Dash()
            {
                if (!NetworkServer.active || !networkedBodyAttachment || !networkedBodyAttachment.attachedBody || !networkedBodyAttachment.attachedBodyObject) return;

                GameObject vehicle = Object.Instantiate(vehiclePrefab, networkedBodyAttachment.attachedBody.corePosition, transform.rotation);
                vehicleSeat = vehicle.GetComponent<VehicleSeat>();
                vehicleSeat.AssignPassenger(networkedBodyAttachment.attachedBodyObject);
                NetworkServer.Spawn(vehicle);
            }

            public void OnEnable()
            {
                InstanceTracker.Add<EliteVarietySandstormBehavior>(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove<EliteVarietySandstormBehavior>(this);
                if (aimRequest != null) aimRequest.Dispose();
            }

            private bool selfDestructed = false;
            public void SelfDestruct()
            {
                if (!selfDestructed)
                {
                    selfDestructed = true;
                    if (indicator)
                    {
                        indicator.parent = null;
                        foreach (ParticleSystem particleSystem in indicator.GetComponentsInChildren<ParticleSystem>())
                        {
                            ParticleSystem.EmissionModule emission = particleSystem.emission;
                            emission.enabled = false;
                        }
                        DestroyOnTimer destroyOnTimer = indicator.GetComponent<DestroyOnTimer>();
                        if (destroyOnTimer) destroyOnTimer.enabled = true;
                    }
                    Object.Destroy(gameObject);
                }
            }

            public void OnAttachedBodyDiscovered(NetworkedBodyAttachment networkedBodyAttachment, CharacterBody attachedBody)
            {
                transform.position = attachedBody.footPosition;
            }

            public class SyncRadius : INetMessage
            {
                NetworkInstanceId objID;
                float radius;

                public SyncRadius()
                {
                }

                public SyncRadius(NetworkInstanceId objID, float radius)
                {
                    this.objID = objID;
                    this.radius = radius;
                }

                public void Deserialize(NetworkReader reader)
                {
                    objID = reader.ReadNetworkId();
                    radius = reader.ReadSingle();
                }

                public void OnReceived()
                {
                    if (NetworkServer.active) return;
                    NetworkHelper.EnqueueOnSpawnedOnClientEvent(objID, (gameObject) =>
                    {
                        EliteVarietySandstormBehavior component = gameObject.GetComponent<EliteVarietySandstormBehavior>();
                        if (component) component.radius = radius;
                    });
                }

                public void Serialize(NetworkWriter writer)
                {
                    writer.Write(objID);
                    writer.Write(radius);
                }
            }

            public static void OnSceneCameraPreRender(SceneCamera sceneCamera)
            {
                if (sceneCamera.cameraRigController)
                {
                    if (sceneCamera.cameraRigController.enableFading)
                    {
                        foreach (EliteVarietySandstormBehavior sandstorm in InstanceTracker.GetInstancesList<EliteVarietySandstormBehavior>())
                        {
                            if (sandstorm.networkedBodyAttachment && sandstorm.networkedBodyAttachment.attachedBody && sandstorm.networkedBodyAttachment.attachedBody == sceneCamera.cameraRigController.targetBody)
                            {
                                foreach (IndicatorRendererRecolorInfo indicatorRendererRecolorInfo in sandstorm.indicatorRendererRecolorInfos)
                                {
                                    if (indicatorRendererRecolorInfo.renderer)
                                    {
                                        indicatorRendererRecolorInfo.renderer.GetPropertyBlock(sandstorm.materialPropertyBlock);
                                        sandstorm.materialPropertyBlock.SetColor("_Color", !sandstorm.dashActive ? indicatorRendererRecolorInfo.changedColor : indicatorRendererRecolorInfo.originalColor);
                                        indicatorRendererRecolorInfo.renderer.SetPropertyBlock(sandstorm.materialPropertyBlock);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class EliteVarietySandstormVehicleBehavior : MonoBehaviour, ICameraStateProvider
        {
            public VehicleSeat vehicleSeat;
            public Rigidbody rigidbody;
            public float initialSpeed = 0f;
            public float speed = 30f;
            public float selfDestructTimer = 3f;
            public float acceleration = 200f;
            public bool directionLock = true;
            public Vector3 direction;

            public bool aiControlled = false;
            public Vector3 aiTargetPosition;
            public Vector3 aiClosestPointToTarget = Vector3.zero;
            public float aiMaxDistanceUntilCancel = 15f;
            public float aiClosestDistance = float.MaxValue;

            public void Awake()
            {
                vehicleSeat = GetComponent<VehicleSeat>();
                vehicleSeat.onPassengerEnter += OnPassengerEnter;
                vehicleSeat.onPassengerExit += OnPassengerExit;
                rigidbody = GetComponent<Rigidbody>();
            }

            public void FixedUpdate()
            {
                if (selfDestructTimer > 0)
                {
                    selfDestructTimer -= Time.fixedDeltaTime;

                    if (vehicleSeat && vehicleSeat.currentPassengerInputBank && vehicleSeat.currentPassengerBody)
                    {
                        Vector3 chosenDirection = direction;
                        if (!directionLock)
                        {
                            Ray originalAimRay = vehicleSeat.currentPassengerInputBank.GetAimRay();
                            originalAimRay = CameraRigController.ModifyAimRayIfApplicable(originalAimRay, gameObject, out _);
                            chosenDirection = originalAimRay.direction;
                        }
                        rigidbody.MoveRotation(Quaternion.LookRotation(chosenDirection));
                        Vector3 targetVelocity = new Vector3(chosenDirection.x, 0f, chosenDirection.z) * speed * Mathf.Max(vehicleSeat.currentPassengerBody.moveSpeed / 7f, 1f);
                        Vector3 currentVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
                        Vector3 velocityChange = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                        rigidbody.AddForce(velocityChange - currentVelocity, ForceMode.VelocityChange);

                        // ai tweaks
                        if (aiControlled)
                        {
                            bool cancel = false;

                            // if we pass the target position, cancel the dash so that we don't go too far from the enemy
                            float distanceToTargetPosition = Vector3.Distance(aiTargetPosition, transform.position);
                            if (distanceToTargetPosition < aiClosestDistance)
                            {
                                aiClosestDistance = distanceToTargetPosition;
                                aiClosestPointToTarget = transform.position;
                            }
                            float distanceToClosestPoint = Vector3.Distance(transform.position, aiClosestPointToTarget);
                            if (distanceToClosestPoint > aiMaxDistanceUntilCancel) cancel = true;

                            // don't launch self off a cliff
                            CharacterMotor characterMotor = vehicleSeat.currentPassengerBody.characterMotor;
                            if (characterMotor && characterMotor.isGrounded)
                            {
                                Vector3 estimatedNextPoint = transform.position + new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z) * (Time.fixedDeltaTime + 0.5f);
                                bool groundUnderneath = Physics.Raycast(estimatedNextPoint + Vector3.up * 0.5f, Vector3.down, 30f, LayerIndex.world.mask);
                                if (!groundUnderneath) cancel = true;
                            }

                            if (cancel) vehicleSeat.EjectPassenger(vehicleSeat.NetworkpassengerBodyObject);
                        }
                    }

                    if (NetworkServer.active && selfDestructTimer <= 0)
                    {
                        Object.Destroy(gameObject);
                    }
                }
            }

            public void OnPassengerEnter(GameObject passenger)
            {
                CharacterBody passengerBody = passenger.GetComponent<CharacterBody>();
                if (!vehicleSeat.currentPassengerInputBank || !passengerBody) return;
                Vector3 aimDirection = vehicleSeat.currentPassengerInputBank.aimDirection;
                rigidbody.rotation = Quaternion.LookRotation(aimDirection);
                rigidbody.velocity = aimDirection * initialSpeed * Mathf.Max(passengerBody.moveSpeed / 7f, 1f);
                direction = aimDirection;
                if (NetworkServer.active) passengerBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
                transform.localScale = new Vector3(passengerBody.bestFitRadius, passengerBody.bestFitRadius, passengerBody.bestFitRadius);

                foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                {
                    if (cameraRigController.target == passenger)
                    {
                        cameraRigController.SetOverrideCam(null, 0f);
                        cameraRigController.SetOverrideCam(this, 0.5f);
                    }
                }

                if (!passengerBody.isPlayerControlled && passengerBody.master)
                {
                    RoR2.CharacterAI.BaseAI ai = passengerBody.master.aiComponents.FirstOrDefault();
                    if (ai && ai.currentEnemy != null)
                    {
                        if (ai.currentEnemy.GetBullseyePosition(out Vector3 enemyPosition))
                        {
                            aiControlled = true;
                            aiTargetPosition = enemyPosition;
                        }
                    }
                }
            }

            public void OnPassengerExit(GameObject passenger)
            {
                foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                {
                    if (cameraRigController.target == passenger)
                    {
                        cameraRigController.SetOverrideCam(this, 0f);
                        cameraRigController.SetOverrideCam(null, 0.5f);
                    }
                }
                if (NetworkServer.active)
                {
                    CharacterBody passengerBody = passenger.GetComponent<CharacterBody>();
                    if (passengerBody) passengerBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
                    Object.Destroy(gameObject);
                }
            }

            public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
            {
            }

            public bool IsUserLookAllowed(CameraRigController cameraRigController)
            {
                return true;
            }

            public bool IsUserControlAllowed(CameraRigController cameraRigController)
            {
                return true;
            }

            public bool IsHudAllowed(CameraRigController cameraRigController)
            {
                return true;
            }
        }

        private void GenericGameEvents_OnHitEnemy(DamageInfo damageInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo attackerInfo, MysticsRisky2UtilsPlugin.GenericCharacterInfo victimInfo)
		{
            if (damageInfo.procCoefficient > 0f && attackerInfo.body && attackerInfo.body.HasBuff(buffDef) && victimInfo.body)
            {
                victimInfo.body.AddTimedBuff(EliteVarietyContent.Buffs.SandstormBlind, 4f * damageInfo.procCoefficient);
            }
		}
	}
}
