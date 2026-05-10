using UnityEngine;

namespace Game.Combat
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private DamageType _type;
        private GameObject _source;
        private LayerMask _hitMask;
        private int _sourceTeamId;
        private bool _initialized;

        public void Init(float damage, DamageType type, GameObject source, float speed, float lifetime, Vector2 dir, LayerMask hitMask)
        {
            _damage = damage;
            _type = type;
            _source = source;
            _hitMask = hitMask;
            _initialized = true;

            if (source != null)
            {
                CombatantBehaviour srcCombatant = source.GetComponent<CombatantBehaviour>();
                _sourceTeamId = srcCombatant != null ? srcCombatant.TeamId : 0;
            }

            Vector2 normalized = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = normalized * speed;

            transform.right = normalized;

            if (lifetime > 0f) Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_initialized || other == null) return;
            if (((1 << other.gameObject.layer) & _hitMask.value) == 0) return;

            CombatantBehaviour target = other.GetComponentInParent<CombatantBehaviour>();
            if (target == null || !target.IsAlive) return;
            if (target.TeamId == _sourceTeamId) return;

            target.ApplyDamage(_damage, _type, _source);
            Destroy(gameObject);
        }
    }
}
