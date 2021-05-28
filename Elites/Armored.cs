namespace EliteVariety.Elites
{
    public class Armored : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Armored";
            tier = 1;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixArmored;
            };
        }
    }
}
