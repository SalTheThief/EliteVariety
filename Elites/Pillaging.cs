using RoR2;
using System.Collections.Generic;

namespace EliteVariety.Elites
{
    public class Pillaging : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Pillaging";
            tier = 1;

            scenesToSpawnPillagingMoreOften.Add("goldshores");

            On.RoR2.CombatDirector.Init += CombatDirector_Init;
        }

        private void CombatDirector_Init(On.RoR2.CombatDirector.orig_Init orig)
        {
            orig();

            CombatDirector.EliteTierDef tier1EliteTierDef = CombatDirector.eliteTiers[1];
            CombatDirector.EliteTierDef tier1HonorEliteTierDef = CombatDirector.eliteTiers[2];

            CombatDirector.EliteTierDef eliteTierDef = new CombatDirector.EliteTierDef();
            eliteTierDef.costMultiplier = tier1EliteTierDef.costMultiplier;
            eliteTierDef.damageBoostCoefficient = tier1EliteTierDef.damageBoostCoefficient;
            eliteTierDef.healthBoostCoefficient = tier1EliteTierDef.healthBoostCoefficient;
            eliteTierDef.eliteTypes = new EliteDef[]
            {
                eliteDef
            };
            eliteTierDef.isAvailable = (SpawnCard.EliteRules rules) => tier1EliteTierDef.isAvailable(rules) && CanSpawnPillagingMoreOften() && PillagingMoreOftenChance();
            HG.ArrayUtils.ArrayAppend(ref CombatDirector.eliteTiers, eliteTierDef);

            eliteTierDef = new CombatDirector.EliteTierDef();
            eliteTierDef.costMultiplier = tier1HonorEliteTierDef.costMultiplier;
            eliteTierDef.damageBoostCoefficient = tier1HonorEliteTierDef.damageBoostCoefficient;
            eliteTierDef.healthBoostCoefficient = tier1HonorEliteTierDef.healthBoostCoefficient;
            eliteTierDef.eliteTypes = new EliteDef[]
            {
                eliteDef
            };
            eliteTierDef.isAvailable = (SpawnCard.EliteRules rules) => tier1HonorEliteTierDef.isAvailable(rules) && CanSpawnPillagingMoreOften() && PillagingMoreOftenChance();
            HG.ArrayUtils.ArrayAppend(ref CombatDirector.eliteTiers, eliteTierDef);
        }

        public static List<string> scenesToSpawnPillagingMoreOften = new List<string>();
        public static bool CanSpawnPillagingMoreOften()
        {
            if (SceneInfo.instance && SceneInfo.instance.sceneDef)
            {
                if (scenesToSpawnPillagingMoreOften.Contains(SceneInfo.instance.sceneDef.baseSceneName)) return true;
            }
            return false;
        }

        public static bool PillagingMoreOftenChance()
        {
            return Util.CheckRoll(50f);
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixPillaging;
        }
    }
}
