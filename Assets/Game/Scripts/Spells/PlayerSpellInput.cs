using UnityEngine;

namespace Game.Spells
{
    public class PlayerSpellInput : MonoBehaviour
    {
        [SerializeField] private SpellCasterRuntime spellCaster;
        [SerializeField] private Camera worldCamera;

        private void Awake()
        {
            if (spellCaster == null)
            {
                spellCaster = GetComponent<SpellCasterRuntime>();
            }

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (spellCaster == null)
            {
                return;
            }

            for (int i = 0; i < spellCaster.Definitions.Count && i < 9; i++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    continue;
                }

                SpellDefinition def = spellCaster.Definitions[i];
                Vector3 point = transform.position;
                if (worldCamera != null)
                {
                    Vector3 mouse = Input.mousePosition;
                    mouse.z = Mathf.Abs(worldCamera.transform.position.z);
                    point = worldCamera.ScreenToWorldPoint(mouse);
                }

                Vector2 direction = ((Vector2)point - (Vector2)transform.position).normalized;
                spellCaster.TryCast(new SpellCastIntent
                {
                    Spell = def,
                    ExplicitTarget = null,
                    TargetPoint = point,
                    Direction = direction
                });
                break;
            }
        }
    }
}
