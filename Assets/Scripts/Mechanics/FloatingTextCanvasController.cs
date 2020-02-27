using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.Mechanics
{
    public class FloatingTextCanvasController : MonoBehaviour
    {
        public List<Text> floaters = new List<Text>();
       
        public void Update()
        {
            transform.position = transform.parent.position;

            foreach(Text t in floaters)
            {
                if (t == null)
                {
                    floaters.Remove(t);
                }
            }

            if (floaters.Count == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}