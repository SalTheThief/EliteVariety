using R2API;
using RoR2;
using UnityEngine;

namespace EliteVariety.Elites
{
    public class ImpPlane : BaseElite
    {
        public override void OnLoad()
        {
            base.OnLoad();
            eliteDef.name = "ImpPlane";
            tier = 2;
            modelEffect = PrefabAPI.InstantiateClone(Main.AssetBundle.LoadAsset<GameObject>("Assets/EliteVariety/Elites/ImpPlane/AffixImpPlaneEffect.prefab"), Main.TokenPrefix + "AffixImpPlaneEffect", false);
            JitterBones jitterBones = modelEffect.AddComponent<JitterBones>();
            jitterBones.perlinNoiseFrequency = 20f;
            jitterBones.perlinNoiseStrength = 3f;
            jitterBones.perlinNoiseMinimumCutoff = 0.5f;
            jitterBones.perlinNoiseMaximumCutoff = 1f;
            jitterBones.headBonusStrength = 0f;

            jitterBones = modelEffect.AddComponent<JitterBones>();
            jitterBones.perlinNoiseFrequency = 20f;
            jitterBones.perlinNoiseStrength = 0.2f;
            jitterBones.perlinNoiseMinimumCutoff = 0.1f;
            jitterBones.perlinNoiseMaximumCutoff = 0.9f;
            jitterBones.headBonusStrength = 30f;

            lightColorOverride = new Color32(230, 0, 60, 255);
            particleMaterialOverride = Main.AssetBundle.LoadAsset<Material>("Assets/EliteVariety/Elites/ImpPlane/matErythriteSpikeVoidParticle.mat");

            onModelEffectSpawn = (model, effect) =>
            {
                Util.PlaySound("EliteVariety_Play_elite_erythrite_spawn", effect);
                if (model.mainSkinnedMeshRenderer)
                {
                    JitterBones[] components = effect.GetComponents<JitterBones>();
                    for (int i = 0; i < components.Length; i++)
                    {
                        components[i].skinnedMeshRenderer = model.mainSkinnedMeshRenderer;
                    }
                }
            };

            MysticsRisky2Utils.Overlays.CreateOverlay(Main.AssetBundle.LoadAsset<Material>("Assets/EliteVariety/Elites/ImpPlane/matEliteImpPlaneOverlay.mat"), (model) =>
            {
                return model.body ? model.body.HasBuff(EliteVarietyContent.Buffs.AffixImpPlane) : false;
            });
        }

        public override void AfterContentPackLoaded()
        {
            base.AfterContentPackLoaded();
            eliteDef.eliteEquipmentDef = EliteVarietyContent.Equipment.AffixImpPlane;
        }
    }
}
