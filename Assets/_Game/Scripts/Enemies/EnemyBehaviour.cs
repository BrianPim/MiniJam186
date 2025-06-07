using DG.Tweening;
using UnityEngine;

namespace Enemies
{
    public abstract class EnemyBehaviour : MonoBehaviour
    {
        public Transform Render;
        public EnemyController Controller;

        [SerializeField] 
        private bool JoinFormation;
        
        private Transform TargetPlace;
        
        public virtual void Awake()
        {
            if (!Controller) 
                Controller = GetComponent<EnemyController>();

            if (JoinFormation) 
                TargetPlace = EnemyGroup.Instance.GetNextSpot();
            else 
                Controller.EnemySpawningComplete();

            //if (!Render)
            //{
            //    Render = transform.GetChild(0);
            //}

            
            //Render.DOShakePosition(1, 0.5f).SetLoops(-1);
        }

        public virtual void Update()
        {
            if (JoinFormation) 
                Controller.SetOverrideDestination(TargetPlace.position);
        }

        public virtual void OnSpawnComplete()
        {
            
        }

        public virtual bool AllowedToDoAction()
        {
            return true;
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