using NUnit.Framework;
using UnityEngine;

public class EffectHandlerTests
{
    [Test]
    public void ApplyEffect_AddsActiveEffect_AndModifiesTargetStat()
    {
        GameObject go = new GameObject("effect-target");
        CharacterStats stats = go.AddComponent<CharacterStats>();
        EffectHandler handler = go.AddComponent<EffectHandler>();

        EffectData effect = ScriptableObject.CreateInstance<EffectData>();
        effect.type = EffectType.BuffStats;
        effect.duration = 5f;
        effect.isStackable = false;
        effect.modifiers.Add(new StatModifier
        {
            stat = StatType.Strength,
            modType = ModificationType.Flat,
            value = 3f
        });

        float baseStrength = stats.FinalStr;
        handler.ApplyEffect(effect);

        Assert.AreEqual(1, handler.ActiveEffects.Count);
        Assert.Greater(stats.FinalStr, baseStrength);

        Object.DestroyImmediate(effect);
        Object.DestroyImmediate(go);
    }
}
