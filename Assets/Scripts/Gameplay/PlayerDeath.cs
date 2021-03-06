﻿using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;

            // Ensure the player has died, as this event may be scheduled in scenarios where the player still has HP
            // such as when entering a Death Zone.
            player.health.Die();
            
            model.virtualCamera.m_Follow = null;
            model.virtualCamera.m_LookAt = null;
            player.controlEnabled = false;
            player.collider2d.enabled = false;

            if (player.audioSource && player.ouchAudio)
                player.audioSource.PlayOneShot(player.ouchAudio);
            
            player.animator.SetTrigger("hurt");
            player.animator.SetBool("dead", true);
            Simulation.Schedule<PlayerSpawn>(2);
        }
    }
}