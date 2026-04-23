using NUnit.Framework;
using UnityEngine;

public class CharacterStatsTests
{
    [Test]
    public void UseStamina_DecreasesCurrentStamina_WhenEnoughResource()
    {
        GameObject go = new GameObject("stats");
        CharacterStats stats = go.AddComponent<CharacterStats>();

        float before = stats.currentStamina;
        bool used = stats.UseStamina(10f);

        Assert.IsTrue(used);
        Assert.Less(stats.currentStamina, before);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void TakeDamage_PhysicalDamageIsReducedByArmor()
    {
        GameObject go = new GameObject("stats");
        CharacterStats stats = go.AddComponent<CharacterStats>();

        float before = stats.currentHealth;
        stats.TakeDamage(10f, DamageType.Physical);

        // With default agility, armor is high enough to fully absorb 10 damage.
        Assert.AreEqual(before, stats.currentHealth, 0.0001f);

        Object.DestroyImmediate(go);
    }
}
