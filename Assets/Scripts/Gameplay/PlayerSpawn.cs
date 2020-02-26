using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            
            player.collider2d.enabled = true;
            player.controlEnabled = false;
            player.health.Live();
            
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);

            player.collider2d.enabled = true;
            player.Teleport(model.spawnPoint.transform.position);
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);

            var playerTransform = player.transform;
            model.virtualCamera.m_Follow = playerTransform;
            model.virtualCamera.m_LookAt = playerTransform;
            
            Simulation.Schedule<EnablePlayerInput>(2f);
        }
    }
}