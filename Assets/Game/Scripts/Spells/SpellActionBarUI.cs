using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Spells
{
    public class SpellActionBarUI : MonoBehaviour
    {
        [System.Serializable]
        private struct SlotView
        {
            public Image Icon;
            public Image CooldownMask;
            public TMP_Text KeyLabel;
            public TMP_Text CooldownLabel;
        }

        [SerializeField] private SpellCasterRuntime spellCaster;
        [SerializeField] private List<SlotView> slots = new();

        private void Awake()
        {
            if (spellCaster == null)
            {
                spellCaster = FindAnyObjectByType<SpellCasterRuntime>();
            }
        }

        private void Update()
        {
            if (spellCaster == null)
            {
                return;
            }

            var defs = spellCaster.Definitions;
            for (int i = 0; i < slots.Count; i++)
            {
                bool valid = i < defs.Count && defs[i] != null;
                if (!valid)
                {
                    continue;
                }

                SpellDefinition def = defs[i];
                SlotView slot = slots[i];
                float cooldownRemaining = spellCaster.GetCooldownRemaining(def.id);
                float duration = Mathf.Max(0.01f, def.cooldown);
                float ratio = cooldownRemaining / duration;

                if (slot.Icon != null) slot.Icon.sprite = def.icon;
                if (slot.CooldownMask != null) slot.CooldownMask.fillAmount = ratio;
                if (slot.KeyLabel != null) slot.KeyLabel.text = (i + 1).ToString();
                if (slot.CooldownLabel != null) slot.CooldownLabel.text = cooldownRemaining > 0.01f ? cooldownRemaining.ToString("0.0") : string.Empty;
            }
        }
    }
}
