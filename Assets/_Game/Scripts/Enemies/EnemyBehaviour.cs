using DG.Tweening;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBehaviour : MonoBehaviour
    {
        public Transform Render;
        public EnemyController Controller;

        [SerializeField] private bool JoinFormationOnSpawn;
        
        private Transform TargetPlace;
        
        public virtual void Awake()
        {
            if (JoinFormationOnSpawn)
            {
                TargetPlace = EnemyGroup.Instance.GetNextSpot();
                Controller.SetOverrideDestination(TargetPlace.position);
            }

            //if (!Render)
            //{
            //    Render = transform.GetChild(0);
            //}

            Controller = GetComponent<EnemyController>();
            
            //Render.DOShakePosition(1, 0.5f).SetLoops(-1);
        }

        public virtual void Update()
        {
            
        }

        public virtual void DoAction()
        {
            
        }

        public virtual void OnDestroy()
        {
            //Render.DOKill();
        }
        
    }
}