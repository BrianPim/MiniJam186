using System.Collections.Generic;
using CamLib;
using UnityEngine;

namespace Enemies
{
    public class Formation : MonoBehaviour
    {
        public List<Transform> Spots;

        public RandomStock<Transform> SpotStock;
            
        public void Awake()
        {
            SpotStock = new RandomStock<Transform>(Spots);
        }
            
            
        public void OnDrawGizmos()
        {
            if (Spots == null || Spots.Count < 2) return;

            // Set the color for the gizmo lines
            Gizmos.color = Color.yellow;

            // Draw lines connecting all spots
            for (int i = 0; i < Spots.Count - 1; i++)
            {
                Gizmos.DrawLine(Spots[i].position, Spots[i + 1].position);
            }

        }
    }
}