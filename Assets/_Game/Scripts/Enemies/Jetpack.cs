using DG.Tweening;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Flies up from the bottom of the screen, then stays on screen and shoots the player
    /// </summary>
    public class Jetpack : EnemyBehaviour
    {
        public Transform TargetPlace;

        public Transform Render;
        
        private void Awake()
        {
            TargetPlace = EnemyGroup.Instance.GetNextSpot();
            
            Render.DOShakePosition(1, 0.5f).SetLoops(-1);
        }

        private void Update()
        {
            Controller.MoveTowards(TargetPlace.position);

            
        }
    }

    public abstract class EnemyBehaviour : MonoBehaviour
    {
        public EnemyController Controller;
        
        
    }
}