using System;
using System.Collections.Generic;
using System.Linq;
using CamLib;
using UnityEngine;

namespace Enemies
{
    //This is a list of transforms that the enemy will try to congregate to. This group will try to move towards the player
    public class EnemyGroup : Singleton<EnemyGroup>
    {
        public List<Formation> Formations;

        public RandomStock<Formation> FormationStock;

        private Formation CurrentFormation;

        protected override void Awake()
        {
            base.Awake();
            
            FormationStock = new RandomStock<Formation>(Formations);
            SetNewFormation();
        }

        private void OnDrawGizmos()
        {
            foreach (Formation formation in Formations)
            {
                if (formation.gameObject.activeInHierarchy)
                {
                    formation.OnDrawGizmos();
                }
            }
        }
        
        public void SetNewFormation()
        {
            CurrentFormation = FormationStock.GetRandom();
        }

        public Transform GetNextSpot()
        {
            return CurrentFormation.SpotStock.GetRandom();
        }
    }
}