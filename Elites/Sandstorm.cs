namespace EliteVariety.Elites
{
    public class Sandstorm : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Sandstorm";
            tier = 1;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixSandstorm;
        }
    }
}
