using UnityEngine;
using MysticsRisky2Utils;
using MysticsRisky2Utils.MonoBehaviours;

namespace EliteVariety.Buffs
{
    public class ArmoredHeavyStun : BaseBuff
    {
        public override void OnLoad()
        {
            base.OnLoad();
            buffDef.name = "ArmoredHeavyStun";
            buffDef.canStack = false;
            buffDef.isDebuff = true;
            buffDef.buffColor = new Color32(255, 209, 209, 255);
            AddRootMovementModifier();

            GameObject debuffedVFX = Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/Armored/ArmoredDebuffVFX.prefab");
            CustomTempVFXManagement.MysticsRisky2UtilsTempVFX tempVFX = debuffedVFX.AddComponent<CustomTempVFXManagement.MysticsRisky2UtilsTempVFX>();
            MysticsRisky2UtilsParticleSystemToggle particleSystemToggle = debuffedVFX.transform.Find("Origin/Particle System").gameObject.AddComponent<MysticsRisky2UtilsParticleSystemToggle>();
            particleSystemToggle.particleSystemsToToggle = new ParticleSystem[]
            {
                particleSystemToggle.gameObject.GetComponent<ParticleSystem>()
            };
            tempVFX.enterBehaviours = new MonoBehaviour[]
            {
                particleSystemToggle
            };
            CustomTempVFXManagement.allVFX.Add(new CustomTempVFXManagement.VFXInfo
            {
                prefab = debuffedVFX,
                condition = (x) => x.HasBuff(buffDef),
                radius = CustomTempVFXManagement.DefaultRadiusCall
            });
        }
    }
}