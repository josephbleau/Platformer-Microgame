using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using Canvas = Unity.UIWidgets.ui.Canvas;

namespace Platformer.Gameplay
{
    public class EnemyHurt : Simulation.Event<EnemyHurt>
    {
        static GameObject ftcContainer = GameObject.Find("FloatingTextContainer");
        
        public EnemyController enemy;
        public float damage;
        
        public override void Execute()
        {
            var ftcPrefab = enemy.ftcPrefab;
            
            var ftcCanvas = Object.Instantiate(
                ftcPrefab, 
                enemy.transform.position, 
                Quaternion.identity
            );
            
            var x = enemy.transform.position.x;
            var y = enemy.transform.position.y;
            var z = enemy.transform.position.z;
            
            ftcCanvas.transform.position = new Vector3(x,y,z);
        }
    }
}