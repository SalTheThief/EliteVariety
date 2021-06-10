using UnityEngine;
using MysticsRisky2Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine.Networking;

namespace EliteVariety.CharacterBodies
{
    public class TinkererDrone : BaseCharacterBody
    {
        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "TinkererDroneBody", true);
			prefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            Utils.CopyChildren(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/CharacterBodies/TinkererDrone/TinkererDroneBody.prefab"), prefab);
            bodyName = "TinkererDrone";

            modelBaseTransform = prefab.transform.Find("ModelBase");
            modelTransform = prefab.transform.Find("ModelBase/mdlTinkererDrone");
            meshObject = prefab.transform.Find("ModelBase/mdlTinkererDrone/TinkererDroneMesh").gameObject;
            Prepare();

			HopooShaderToMaterial.Standard.Emission(meshObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial, 1f);
            modelTransform.Find("TinkererDroneArmature/ROOT/Antenna Light").gameObject.AddComponent<Billboard>();

            SetUpChildLocator(new ChildLocator.NameTransformPair[]
            {
                new ChildLocator.NameTransformPair
                {
                    name = "Muzzle",
                    transform = modelTransform.Find("TinkererDroneArmature/ROOT/Arm.l.1/Arm.l.2/Gun.l/Muzzle")
                }
            });

            modelLocator.dontReleaseModelOnDeath = true;
            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;

            // body
            CharacterBody characterBody = prefab.GetComponent<CharacterBody>();
            characterBody.bodyFlags = CharacterBody.BodyFlags.Mechanical;
            characterBody.baseMaxHealth = 200f;
            characterBody.baseRegen = 3f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = 17f;
            characterBody.baseAcceleration = 17f;
            characterBody.baseJumpPower = 0f;
            characterBody.baseDamage = 12f;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = 0f;
            characterBody.baseJumpCount = 1;
            characterBody.aimOriginTransform = modelTransform.Find("TinkererDroneArmature/AimOrigin");
            characterBody.hullClassification = HullClassification.Human;
			characterBody.portraitIcon = Main.AssetBundle.LoadAsset<Texture>("Assets/EliteVariety/CharacterBodies/TinkererDrone/Portrait.png");
			characterBody.bodyColor = new Color32(37, 222, 112, 255);
            characterBody.isChampion = false;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.FlyState));
            AfterCharacterBodySetup();

			// death rewards
            UnlockableDef logUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            logUnlockableDef.cachedName = "Logs." + Main.TokenPrefix + "TinkererDroneBody.0";
            logUnlockableDef.nameToken = "UNLOCKABLE_LOG_" + Main.TokenPrefix.ToUpper() + "TINKERERDRONE";
            logUnlockableDef.displayModelPrefab = prefab;
            logUnlockableDef.hidden = false;
			EliteVarietyContent.Resources.unlockableDefs.Add(logUnlockableDef);

            DeathRewards deathRewards = prefab.AddComponent<DeathRewards>();
            deathRewards.logUnlockableDef = logUnlockableDef;

			// hurtbox
			SetUpHurtBoxGroup(new HurtBoxSetUpInfo[]
			{
				new HurtBoxSetUpInfo
                {
					transform = modelTransform.Find("TinkererDroneArmature/HurtBox"),
					isBullseye = true,
					isMain = true
                },
				new HurtBoxSetUpInfo
				{
					transform = modelTransform.Find("TinkererDroneArmature/ROOT/Arm.l.1/HurtBox (1)")
				},
				new HurtBoxSetUpInfo
				{
					transform = modelTransform.Find("TinkererDroneArmature/ROOT/Arm.l.1/Arm.l.2/HurtBox (2)")
				},
				new HurtBoxSetUpInfo
				{
					transform = modelTransform.Find("TinkererDroneArmature/ROOT/Arm.l.1/Arm.l.2/Gun.l/HurtBox (3)")
				}
			});

            // sfx
            SfxLocator sfxLocator = prefab.AddComponent<SfxLocator>();
            sfxLocator.deathSound = "Play_roboBall_attack2_mini_death";

            // motor
            SetUpRigidbodyMotor();

			// camera pivot
			CameraTargetParams cameraTargetParams = prefab.GetComponent<CameraTargetParams>();
			cameraTargetParams.cameraPivotTransform = prefab.transform.Find("CameraPivot");

			// model panel parameters
			ModelPanelParameters modelPanelParameters = modelTransform.gameObject.AddComponent<ModelPanelParameters>();
			modelPanelParameters.focusPointTransform = modelTransform.Find("FocusPoint");
			modelPanelParameters.cameraPositionTransform = modelTransform.Find("FocusPoint/CameraPosition");
			modelPanelParameters.minDistance = 4f;
			modelPanelParameters.maxDistance = 8f;

			// aim
            // doesn't animate properly for some reason. let's just set the pitch and yaw ranges to 10 for now
			AimAnimator aimAnimator = modelTransform.GetComponent<AimAnimator>();
			aimAnimator.pitchRangeMin = -10f;
			aimAnimator.pitchRangeMax = 10f;
			aimAnimator.yawRangeMin = -10f;
			aimAnimator.yawRangeMax = 10f;
			aimAnimator.pitchGiveupRange = 1f;
			aimAnimator.yawGiveupRange = 1f;
			aimAnimator.giveupDuration = 2f;
			aimAnimator.raisedApproachSpeed = 720f;
			aimAnimator.loweredApproachSpeed = 360f;
			aimAnimator.smoothTime = 0.1f;
			aimAnimator.fullYaw = false;
			aimAnimator.aimType = AimAnimator.AimType.Direct;
			aimAnimator.enableAimWeight = false;

			// state machines
			EntityStateMachine bodyStateMachine = SetUpEntityStateMachine("Body", typeof(TinkererDroneSpawnState), typeof(EntityStates.FlyState));
            EntityStateMachine weaponStateMachine = SetUpEntityStateMachine("Weapon", typeof(EntityStates.Idle), typeof(EntityStates.Idle));

            CharacterDeathBehavior characterDeathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = bodyStateMachine;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(TinkererDroneDeath));
            characterDeathBehavior.idleStateMachine = new EntityStateMachine[] {
                weaponStateMachine
            };
            EliteVarietyContent.Resources.entityStateTypes.Add(typeof(TinkererDroneDeath));

            // skills
            SkillDef skillFire = ScriptableObject.CreateInstance<SkillDef>();
            ((ScriptableObject)skillFire).name = Main.TokenPrefix + "TinkererDroneBodyFire";
            skillFire.skillName = "Fire";
            skillFire.activationStateMachineName = "Weapon";
            skillFire.activationState = new EntityStates.SerializableEntityStateType(typeof(TinkererDroneChargeLaser));
            skillFire.interruptPriority = EntityStates.InterruptPriority.Skill;
            skillFire.baseRechargeInterval = 2f;
            skillFire.baseMaxStock = 1;
            skillFire.rechargeStock = 1;
            skillFire.requiredStock = 1;
            skillFire.stockToConsume = 1;
            skillFire.resetCooldownTimerOnUse = true;
            skillFire.fullRestockOnAssign = true;
            skillFire.dontAllowPastMaxStocks = false;
            skillFire.beginSkillCooldownOnSkillEnd = true;
            skillFire.cancelSprintingOnActivation = true;
            skillFire.forceSprintDuringState = false;
            skillFire.canceledFromSprinting = false;
            skillFire.isCombatSkill = true;
            skillFire.mustKeyPress = false;
            EliteVarietyContent.Resources.skillDefs.Add(skillFire);

            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            ((ScriptableObject)skillFamily).name = Main.TokenPrefix + "TinkererDroneBodyPrimaryFamily";
            skillFamily.variants = new SkillFamily.Variant[]
            {
                new SkillFamily.Variant
                {
                    skillDef = skillFire
                }
            };
            skillFamily.defaultVariantIndex = 0;
			EliteVarietyContent.Resources.skillFamilies.Add(skillFamily);

			GenericSkill primarySkill = prefab.AddComponent<GenericSkill>();
			primarySkill._skillFamily = skillFamily;
			primarySkill.skillName = "Fire";

            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();
            skillLocator.primary = primarySkill;

            // model
            CharacterModel characterModel = modelTransform.GetComponent<CharacterModel>();
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = meshObject.GetComponent<SkinnedMeshRenderer>(),
                    defaultMaterial = meshObject.GetComponent<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = modelTransform.Find("TinkererDroneArmature/ROOT/Booster/BoosterParticles").GetComponent<ParticleSystemRenderer>(),
                    defaultMaterial = modelTransform.Find("TinkererDroneArmature/ROOT/Booster/BoosterParticles").GetComponent<ParticleSystemRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true
                }
            };
			AfterCharacterModelSetUp();

			EliteVarietyContent.Resources.entityStateTypes.Add(typeof(TinkererDroneChargeLaser));
            EliteVarietyContent.Resources.entityStateTypes.Add(typeof(TinkererDroneSpawnState));

            TinkererDroneChargeLaser.chargeEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/CharacterBodies/TinkererDrone/LaserChargeEffect.prefab");
            ScaleParticleSystemDuration scaleParticleSystemDuration = TinkererDroneChargeLaser.chargeEffectPrefab.AddComponent<ScaleParticleSystemDuration>();
            scaleParticleSystemDuration.initialDuration = 0.9f;
            scaleParticleSystemDuration.particleSystems = new ParticleSystem[]
            {
                TinkererDroneChargeLaser.chargeEffectPrefab.GetComponentInChildren<ParticleSystem>()
            };
            TinkererDroneChargeLaser.aimEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/CharacterBodies/TinkererDrone/LaserAimEffect.prefab");

            float tracerEffectDuration = 0.2f;
            TinkererDroneFireLaser.tracerEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/CharacterBodies/TinkererDrone/TracerEffect.prefab");
            EffectComponent effectComponent = TinkererDroneFireLaser.tracerEffectPrefab.AddComponent<EffectComponent>();
            Tracer tracer = TinkererDroneFireLaser.tracerEffectPrefab.AddComponent<Tracer>();
            tracer.startTransform = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("TracerStartPos");
            tracer.beamObject = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line").gameObject;
            tracer.speed = 5000f;
            tracer.headTransform = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("TracerHead");
            tracer.tailTransform = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("TracerTail");
            tracer.length = 100f;
            TinkererDroneFireLaser.tracerEffectPrefab.AddComponent<EventFunctions>();
            MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsTracerOnTailReachedDefaults tracerOnTailReachedDefaults = TinkererDroneFireLaser.tracerEffectPrefab.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsTracerOnTailReachedDefaults>();
            tracerOnTailReachedDefaults.transformsToUnparent = new Transform[]
            {
                TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line"),
                TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line (Faded)")
            };
            tracerOnTailReachedDefaults.destroySelf = true;
            MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsLineWidthOverTime lineWidthOverTime = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line").gameObject.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsLineWidthOverTime>();
            lineWidthOverTime.animationCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(1f, 0f)
                }
            };
            lineWidthOverTime.maxDuration = tracerEffectDuration;
            BeamPointsFromTransforms beamPointsFromTransforms = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line").gameObject.AddComponent<BeamPointsFromTransforms>();
            beamPointsFromTransforms.target = beamPointsFromTransforms.GetComponentInChildren<LineRenderer>();
            beamPointsFromTransforms.pointTransforms = new Transform[]
            {
                tracer.startTransform,
                tracer.tailTransform
            };
            TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line").gameObject.AddComponent<DestroyOnTimer>().duration = tracerEffectDuration;
            lineWidthOverTime = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line (Faded)").gameObject.AddComponent<MysticsRisky2Utils.MonoBehaviours.MysticsRisky2UtilsLineWidthOverTime>();
            lineWidthOverTime.animationCurve = new AnimationCurve
            {
                keys = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(0.1f, 0.5f),
                    new Keyframe(0.2f, 0.9f),
                    new Keyframe(0.3f, 0.4f),
                    new Keyframe(0.4f, 0.7f),
                    new Keyframe(0.5f, 0.3f),
                    new Keyframe(0.6f, 0.5f),
                    new Keyframe(0.7f, 0.2f),
                    new Keyframe(0.8f, 0.3f),
                    new Keyframe(0.9f, 0.1f),
                    new Keyframe(1f, 0f)
                }
            };
            for (var i = 0; i < lineWidthOverTime.animationCurve.length; i++) lineWidthOverTime.animationCurve.SmoothTangents(i, 0f);
            lineWidthOverTime.maxDuration = tracerEffectDuration;
            beamPointsFromTransforms = TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line (Faded)").gameObject.AddComponent<BeamPointsFromTransforms>();
            beamPointsFromTransforms.target = beamPointsFromTransforms.GetComponent<LineRenderer>();
            beamPointsFromTransforms.pointTransforms = new Transform[]
            {
                tracer.headTransform,
                tracer.tailTransform
            };
            TinkererDroneFireLaser.tracerEffectPrefab.transform.Find("Line (Faded)").gameObject.AddComponent<DestroyOnTimer>().duration = tracerEffectDuration;
            EliteVarietyContent.Resources.effectPrefabs.Add(TinkererDroneFireLaser.tracerEffectPrefab);

            TinkererDroneFireLaser.hitEffectPrefab = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/CharacterBodies/TinkererDrone/HitEffect.prefab");
            effectComponent = TinkererDroneFireLaser.hitEffectPrefab.AddComponent<EffectComponent>();
            VFXAttributes vfxAttributes = TinkererDroneFireLaser.hitEffectPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            ShakeEmitter shakeEmitter = TinkererDroneFireLaser.hitEffectPrefab.AddComponent<ShakeEmitter>();
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.wave.amplitude = 2f;
            shakeEmitter.wave.frequency = 20f;
            shakeEmitter.duration = 0.15f;
            shakeEmitter.radius = 25f;
            shakeEmitter.amplitudeTimeDecay = true;
            TinkererDroneFireLaser.hitEffectPrefab.AddComponent<DestroyOnTimer>().duration = 1f;
            EliteVarietyContent.Resources.effectPrefabs.Add(TinkererDroneFireLaser.hitEffectPrefab);
        }

        public class TinkererDroneChargeLaser : EntityStates.BaseState
		{
            public static float baseDuration = 0.9f;
            public static string chargeSoundString = "EliteVariety_Play_tinkerer_drone_laser_charge";
            public static GameObject chargeEffectPrefab;
            public static GameObject aimEffectPrefab;
            public static float maxAimLineWidth = 0.2f;

            public float duration;
            public uint chargeSound;
            public GameObject chargeEffect;
            public GameObject aimEffect;
            public LineRenderer aimEffectLine;
            public Vector3 calculatedAimDirection;

            public override void OnEnter()
            {
                base.OnEnter();

                duration = baseDuration / attackSpeedStat;
                Transform modelTransform = GetModelTransform();
                chargeSound = Util.PlayAttackSpeedSound(chargeSoundString, gameObject, attackSpeedStat);
                if (modelTransform)
                {
                    ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
                    if (childLocator)
                    {
                        Transform muzzle = childLocator.FindChild("Muzzle");
                        if (muzzle)
                        {
                            if (chargeEffectPrefab)
                            {
                                chargeEffect = Object.Instantiate(chargeEffectPrefab, muzzle.position, muzzle.rotation);
                                chargeEffect.transform.parent = muzzle;
                                ScaleParticleSystemDuration component = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
                                if (component) component.newDuration = duration;
                            }
                            if (aimEffectPrefab)
                            {
                                aimEffect = Object.Instantiate(aimEffectPrefab, muzzle.position, muzzle.rotation);
                                aimEffect.transform.parent = muzzle;
                                aimEffectLine = aimEffect.GetComponentInChildren<LineRenderer>();
                            }
                        }
                    }
                }
                if (characterBody)
                {
                    characterBody.SetAimTimer(duration);
                }
            }

            public override void Update()
            {
                base.Update();
                if (aimEffect && aimEffectLine)
                {
                    float maxRayDistance = 1000f;
                    Ray aimRay = GetAimRay();
                    Vector3 muzzlePosition = aimEffect.transform.parent.position;
                    Vector3 aimRayPoint = aimRay.GetPoint(maxRayDistance);
                    calculatedAimDirection = aimRayPoint - muzzlePosition;
                    RaycastHit raycastHit;
                    if (Physics.Raycast(aimRay, out raycastHit, maxRayDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
                    {
                        aimRayPoint = raycastHit.point;
                    }
                    aimEffectLine.SetPosition(0, muzzlePosition);
                    aimEffectLine.SetPosition(1, aimRayPoint);

                    float lineWidth = age / duration * maxAimLineWidth;
                    aimEffectLine.startWidth = lineWidth;
                    aimEffectLine.endWidth = lineWidth;
                }
            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (fixedAge >= duration && isAuthority)
                {
                    TinkererDroneFireLaser fireLaser = new TinkererDroneFireLaser();
                    fireLaser.calculatedAimDirection = calculatedAimDirection;
                    outer.SetNextState(fireLaser);
                    return;
                }
            }

            public override void OnExit()
            {
                AkSoundEngine.StopPlayingID(chargeSound);
                base.OnExit();
                if (chargeEffect) Object.Destroy(chargeEffect);
                if (aimEffect) Object.Destroy(aimEffect);
            }

            public override EntityStates.InterruptPriority GetMinimumInterruptPriority()
            {
                return EntityStates.InterruptPriority.Skill;
            }
        }

        public class TinkererDroneFireLaser : EntityStates.BaseState
        {
            public static float baseDuration = 0.5f;
            public static string attackSoundString = "EliteVariety_Play_tinkerer_drone_laser_fire";
            public static GameObject muzzleFlashPrefab;
            public static GameObject tracerEffectPrefab;
            public static GameObject hitEffectPrefab;
            public static float damageCoefficient = 3f;
            public static float force = 2000f;
            public static float laserRadius = 0.7f;
            public static float procCoefficient = 1f;

            public float duration;
            public Vector3 calculatedAimDirection;

            public override void OnEnter()
            {
                base.OnEnter();

                duration = baseDuration / attackSpeedStat;
                Ray modifiedAimRay = GetAimRay();
                modifiedAimRay.direction = calculatedAimDirection;
                Util.PlaySound(attackSoundString, gameObject);
                if (characterBody)
                {
                    characterBody.SetAimTimer(2f);
                }
                PlayAnimation("Gesture", "Fire", "Fire.playbackRate", duration);
                if (muzzleFlashPrefab) EffectManager.SimpleMuzzleFlash(muzzleFlashPrefab, gameObject, "Muzzle", false);
                if (isAuthority)
                {
                    float maxRayDistance = 1000f;
                    bool isCrit = RollCrit();
                    BulletAttack bulletAttack = new BulletAttack
                    {
                        origin = modifiedAimRay.origin,
                        aimVector = modifiedAimRay.direction,
                        bulletCount = 1,
                        damage = damageStat * damageCoefficient,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.None,
                        force = force,
                        hitEffectPrefab = hitEffectPrefab,
                        HitEffectNormal = false,
                        hitMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask,
                        isCrit = isCrit,
                        maxDistance = maxRayDistance,
                        minSpread = 0f,
                        maxSpread = 0f,
                        muzzleName = "Muzzle",
                        owner = gameObject,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        radius = laserRadius,
                        smartCollision = true,
                        sniper = false,
                        spreadPitchScale = 1f,
                        spreadYawScale = 1f,
                        stopperMask = LayerIndex.world.mask,
                        tracerEffectPrefab = tracerEffectPrefab,
                        weapon = gameObject
                    };
                    bulletAttack.Fire();
                }
            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (fixedAge >= duration && isAuthority)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }

            public override EntityStates.InterruptPriority GetMinimumInterruptPriority()
            {
                return EntityStates.InterruptPriority.Skill;
            }
        }

        public class TinkererDroneDeath : EntityStates.Drone.DeathState
        {
            public override void OnEnter()
            {
                destroyOnImpact = true;
                base.OnEnter();
            }
        }

        public class TinkererDroneSpawnState : EntityStates.BaseState
        {
            public static float duration = 1.5f;
            public static string spawnSoundString = "Play_drone_repair";

            public override void OnEnter()
            {
                base.OnEnter();
                PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
                Util.PlaySound(spawnSoundString, gameObject);
            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (fixedAge >= duration && isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }

            public override EntityStates.InterruptPriority GetMinimumInterruptPriority()
            {
                return EntityStates.InterruptPriority.Death;
            }
        }
    }
}
