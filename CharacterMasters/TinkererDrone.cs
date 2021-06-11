using RoR2;
using RoR2.CharacterAI;
using MysticsRisky2Utils;
using UnityEngine;

namespace EliteVariety.CharacterMasters
{
    public class TinkererDrone : BaseCharacterMaster
    {
        public override void OnPluginAwake()
        {
            base.OnPluginAwake();
            prefab = Utils.CreateBlankPrefab(Main.TokenPrefix + "TinkererDroneMaster", true);
        }

        public override void OnLoad()
        {
            base.OnLoad();

            masterName = "TinkererDrone";
            Prepare();

            spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;

            EntityStateMachine aiStateMachine = prefab.AddComponent<EntityStateMachine>();
            aiStateMachine.customName = "AI";
            aiStateMachine.initialStateType = aiStateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));

            BaseAI ai = prefab.AddComponent<BaseAI>();
            ai.fullVision = true;
            ai.neverRetaliateFriendlies = true;
            ai.enemyAttentionDuration = 5f;
            ai.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;
            ai.stateMachine = aiStateMachine;
            ai.scanState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            ai.isHealer = false;
            ai.enemyAttention = 0f;
            ai.aimVectorDampTime = 0.1f;
            ai.aimVectorMaxSpeed = 60f;

            AISkillDriver hardLeashToLeader = prefab.AddComponent<AISkillDriver>();
            hardLeashToLeader.customName = "HardLeashToLeader";
            hardLeashToLeader.skillSlot = SkillSlot.None;
            hardLeashToLeader.minDistance = 30f;
            hardLeashToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            hardLeashToLeader.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            hardLeashToLeader.moveInputScale = 1f;
            hardLeashToLeader.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            hardLeashToLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeashToLeader.driverUpdateTimerOverride = 3f;
            hardLeashToLeader.resetCurrentEnemyOnNextDriverSelection = true;

            AISkillDriver softLeashAttack = prefab.AddComponent<AISkillDriver>();
            softLeashAttack.customName = "SoftLeashAttack";
            softLeashAttack.skillSlot = SkillSlot.Primary;
            softLeashAttack.minDistance = 15f;
            softLeashAttack.maxDistance = 30f;
            softLeashAttack.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeashAttack.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            softLeashAttack.moveInputScale = 1f;
            softLeashAttack.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            softLeashAttack.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            softLeashAttack.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            softLeashAttack.driverUpdateTimerOverride = 1f;
            softLeashAttack.activationRequiresAimTargetLoS = true;
            
            AISkillDriver softLeashToLeader = prefab.AddComponent<AISkillDriver>();
            softLeashToLeader.customName = "SoftLeashToLeader";
            softLeashToLeader.skillSlot = SkillSlot.None;
            softLeashToLeader.minDistance = 15f;
            softLeashToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeashToLeader.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            softLeashToLeader.moveInputScale = 1f;
            softLeashToLeader.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            softLeashToLeader.driverUpdateTimerOverride = 2f;
            softLeashToLeader.resetCurrentEnemyOnNextDriverSelection = true;

            AISkillDriver strafeNearbyEnemies = prefab.AddComponent<AISkillDriver>();
            strafeNearbyEnemies.customName = "StrafeNearbyEnemies";
            strafeNearbyEnemies.skillSlot = SkillSlot.Primary;
            strafeNearbyEnemies.requireSkillReady = true;
            strafeNearbyEnemies.minDistance = 0f;
            strafeNearbyEnemies.maxDistance = 15f;
            strafeNearbyEnemies.selectionRequiresTargetLoS = true;
            strafeNearbyEnemies.activationRequiresTargetLoS = true;
            strafeNearbyEnemies.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            strafeNearbyEnemies.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            strafeNearbyEnemies.moveInputScale = 1f;
            strafeNearbyEnemies.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            strafeNearbyEnemies.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseRandomEnemiesIfLeaderIsDead = prefab.AddComponent<AISkillDriver>();
            chaseRandomEnemiesIfLeaderIsDead.customName = "ChaseRandomEnemiesIfLeaderIsDead";
            chaseRandomEnemiesIfLeaderIsDead.skillSlot = SkillSlot.None;
            chaseRandomEnemiesIfLeaderIsDead.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseRandomEnemiesIfLeaderIsDead.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseRandomEnemiesIfLeaderIsDead.moveInputScale = 1f;
            chaseRandomEnemiesIfLeaderIsDead.aimType = AISkillDriver.AimType.AtCurrentEnemy;

            AISkillDriver attackNearbyEnemiesIfLeaderIsDead = prefab.AddComponent<AISkillDriver>();
            attackNearbyEnemiesIfLeaderIsDead.customName = "AttackNearbyEnemiesIfLeaderIsDead";
            attackNearbyEnemiesIfLeaderIsDead.skillSlot = SkillSlot.Primary;
            attackNearbyEnemiesIfLeaderIsDead.minDistance = 20f;
            attackNearbyEnemiesIfLeaderIsDead.maxDistance = 60f;
            attackNearbyEnemiesIfLeaderIsDead.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            attackNearbyEnemiesIfLeaderIsDead.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            attackNearbyEnemiesIfLeaderIsDead.moveInputScale = 1f;
            attackNearbyEnemiesIfLeaderIsDead.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            attackNearbyEnemiesIfLeaderIsDead.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            attackNearbyEnemiesIfLeaderIsDead.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            attackNearbyEnemiesIfLeaderIsDead.driverUpdateTimerOverride = 1f;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            prefab.GetComponent<CharacterMaster>().bodyPrefab = MysticsRisky2Utils.BaseAssetTypes.BaseCharacterBody.keyToBody["TinkererDrone"].prefab;
        }
    }
}
