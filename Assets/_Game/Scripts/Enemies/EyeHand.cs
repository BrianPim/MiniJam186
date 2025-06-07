using System.Collections;
using Projectiles;
using UnityEngine;

namespace Enemies
{
    
    public class EyeHand : EnemyBehaviour
    {
        public EnemyProjectile Projectile;
        public int ProjectileBurstHowMany = 3;
        public float TimeBetweenProjectiles = .2f;
        public override void DoAction()
        {
            base.DoAction();
            
            //Controller.Animator.SetTrigger("shoot");
            //Controller.SfxShoot.Play();
            StartCoroutine(CoCastProjectileBurst());

            IEnumerator CoCastProjectileBurst()
            {
                yield return new WaitForSeconds(.3f);

                for(int i = 0; i < ProjectileBurstHowMany; i++)
                {
                    if (Controller.IsBeingDestroyed())
                        yield break;

                    var laser = Instantiate(Projectile);
                    laser.transform.position = Controller.ProjectileSpawnPosition.position;

                    var direction = Vector2.left;

                    laser.Setup(direction.normalized);

                    yield return new WaitForSeconds(TimeBetweenProjectiles);
                }
            }
        }
    }
}