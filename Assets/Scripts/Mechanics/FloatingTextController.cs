using UnityEngine;
using UnityEngine.UI;

namespace Platformer.Gameplay
{
    public class FloatingTextController : MonoBehaviour
    {
        public Text parentText;
        
        private float upwardSpeed = 1.0f;
        private float wobbleSpeed = 2.0f;
        private float center;
        private float wobbleTarget;
        
        public void Awake()
        {
            upwardSpeed = 1f + Random.Range(0f, .5f);
            wobbleSpeed = .5f;
            center = transform.position.x;
            wobbleTarget = center;
        }

        private void newWobbleTarget(bool left)
        {
            if (left)
            {
                wobbleTarget = center + Random.Range(-0.2f, -.4f);
            }
            else
            {
                wobbleTarget = center + Random.Range(0.2f, .4f);
            }
        }
        
        public void Update()
        {
            float delta = Time.deltaTime;

            var color = parentText.color;
            color.a -= 0.5f * delta;
            parentText.color = color;

            if (color.a < 0)
            {
                parentText.enabled = false;
                Destroy(parentText.gameObject);
                return;
            }
            
            float upMovement = delta * upwardSpeed;
            float wobbleMovement = delta * wobbleSpeed;
            
            Vector3 newPosition = transform.position;
            newPosition.y += upMovement;

            if (transform.position.x > wobbleTarget)
            {
                if (transform.position.x - wobbleMovement <= wobbleTarget) newWobbleTarget(false);
                newPosition.x -= wobbleMovement;
            }
            else
            {
                if (transform.position.x + wobbleMovement >= wobbleTarget) newWobbleTarget(true);
                newPosition.x += wobbleMovement;
            }
            
            transform.SetPositionAndRotation(newPosition, Quaternion.identity);
        }
    }
}