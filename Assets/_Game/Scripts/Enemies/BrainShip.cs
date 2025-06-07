using System.Collections;
using Projectiles;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Can appear on screen from an x% chance after any enemy dies, to act as a resurrect. Acts as a simple shooter and joins the horde.
    /// </summary>
    public class BrainShip : EnemyBehaviour
    {
        public EnemyProjectile Projectile;
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