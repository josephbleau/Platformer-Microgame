using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Platformer.Gameplay
{
    public class PlayerHurt : Simulation.Event<PlayerHurt>
    {
        public PlayerController player;
        public int damage;
        
        public Vector2 knockbackDirection;
        public bool temporaryInvincibility = true;
        public bool playerRecoil = true;
        
        public override void Execute()
        {
            var damageLeft = damage;
            while (damageLeft > 0)
            {
                player.health.Decrement();
                damageLeft--;
            }

            if (playerRecoil)
            {
                player.jumpState = PlayerController.JumpState.Recoil;
                player.controlEnabled = false;
                player.Bounce(knockbackDirection);
            }
        }
    }
}