using System.Collections;
using Projectiles;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Flies up from the bottom of the screen, then stays on screen and shoots the player
    /// </summary>
    public class Jetpack : EnemyBehaviour
    {
        public EnemyProjectile Projectile;

        public override bool AllowedToDoAction()
        {
            var hits = Physics2D.RaycastAll(transform.position, Vector2.left, Controller.GetDistanceToAttackPlayer());

            foreach(var hit in hits)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) 
                {
                    return true;
                } 
            }

            return false;
        }

        public override void DoAction()
        {
            base.DoAction();
            
            //Controller.Animator.SetTrigger("shoot");
            //Controller.SfxShoot.Play();
        
            StartCoroutine(CoCastProjectile());

            IEnumerator CoCastProjectile()
            {
                yield return new WaitForSeconds(.3f);

                if (Controller.IsBeingDestroyed())
                    yield break;

                var laser = Instantiate(Projectile);
                laser.transform.position = Controller.ProjectileSpawnPosition.position;

                var direction = Vector2.left;

                laser.Setup(direction.normalized);
            }
        }
    }
}