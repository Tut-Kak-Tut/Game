using Game.Spells;
using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(menuName = "RPG System/Weapons/Magic", fileName = "MagicWeaponData")]
    public class MagicWeaponData : WeaponData
    {
        [Header("Magic Specific")]
        public SpellDefinition spell;

        public override bool PerformAttack(PlayerWeaponAttack invoker, Vector2 facing, GameObject sourceGO)
        {
            if (sourceGO == null || spell == null) return false;

            SpellCasterRuntime caster = sourceGO.GetComponent<SpellCasterRuntime>();
            if (caster == null) return false;

            SpellCastIntent intent = new SpellCastIntent
            {
                Spell = spell,
                Direction = facing,
                TargetPoint = invoker.AimPoint
            };

            return caster.TryCast(intent);
        }
    }
}
