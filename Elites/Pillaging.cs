namespace EliteVariety.Elites
{
    public class Pillaging : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Pillaging";
            tier = 1;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixPillaging;
            };
        }
    }
}
