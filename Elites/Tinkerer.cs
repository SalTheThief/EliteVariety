namespace EliteVariety.Elites
{
    public class Tinkerer : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "Tinkerer";
            tier = 1;
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixTinkerer;
        }
    }
}
