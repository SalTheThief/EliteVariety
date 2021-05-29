namespace EliteVariety.Elites
{
    public class Buffing : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Buffing";
            tier = 1;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixBuffing;
            };
        }
    }
}
