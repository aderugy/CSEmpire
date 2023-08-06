using Game.Player;
using UnityEngine;

namespace Game.Bots
{
    public class Bot : MonoBehaviour, IDamageable
    {
        public int health = 100;
        
        public void TakeDamage(int attackerViewID, int damage)
        {
            health -= damage;

            if (health <= 0)
                Die();
        }

        private void Die()
        {
            GameModes.Game.Instance.OnPlayerKilled(null, null);
            Destroy(gameObject);
        }
    }
}