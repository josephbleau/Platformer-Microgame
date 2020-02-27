using System.Numerics;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
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

        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;
            
            if (willHurtEnemy)
            {
                var enemyHealth = enemy.GetComponent<Health>();

                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    Schedule<EnemyHurt>().enemy = enemy;

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
                var ev = Schedule<PlayerHurt>();
                ev.player = player;
                ev.damage = 1;

                player.Recoil(player.Bounds.center.x <= enemy.Bounds.center.x);
            }
        }
    }
}