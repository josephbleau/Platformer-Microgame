using System.Numerics;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using static Platformer.Core.Simulation;
using Vector2 = UnityEngine.Vector2;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = GetModel<PlatformerModel>();

        public override bool Precondition()
        {
            return player.tangible;
        }

        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;

            if (willHurtEnemy)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        player.Bounce(7);
                    }
                    else
                    {
                        player.Bounce(2);
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                    player.Bounce(2);
                }
            }
            else
            {
                enemy._collider.enabled = false;

                var xKnockback = (player.Bounds.center.x >= enemy.Bounds.max.x) ? 40 : 40;
                var yKnockback = 5;

                var ev = Schedule<PlayerHurt>();
                ev.player = player;
                ev.damage = 0;
                ev.knockbackDirection = new Vector2(xKnockback, yKnockback);
            }
        }
    }
}