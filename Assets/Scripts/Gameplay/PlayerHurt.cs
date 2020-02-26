using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    public class PlayerHurt : Simulation.Event<PlayerHurt>
    {
        public PlayerController player;
        public int damage;
        
        public bool playerRecoil = true;
        
        public override void Execute()
        {
            player.audioSource.PlayOneShot(player.ouchAudio);

            if (playerRecoil)
            {
                player.jumpState = PlayerController.JumpState.PrepareToRecoil;
            }
            
            var damageLeft = damage;
            while (damageLeft > 0)
            {
                player.health.Decrement();
                damageLeft--;
            }
        }
    }
}