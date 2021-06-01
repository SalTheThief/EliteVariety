namespace EliteVariety.Elites
{
    public class Pillaging : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Pillaging";
            tier = 1;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixPillaging;
        }
    }
}
