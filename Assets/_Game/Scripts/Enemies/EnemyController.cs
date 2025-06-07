using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Graphs;
using UnityEngine;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        //Constants
        //-------------------------------------------

        private const int MaxOnFireStacks = 10;
        private const float OnFireStackDuration = 1;
        private const float OnFireChipDamage = 5f;
        
        private const int MaxFrozenStacks = 10;
        private const float FrozenStackDuration = 1;

        
        //-------------------------------------------

        
        public float Health;

        [NonSerialized] public bool InFlamethrower;
        [NonSerialized] public bool InCryoBeam;
        
        public EnemyType Type;

        private int OnFireStacks;
        private float OnFireDurationRemaining;

        private int FrozenStacks;
        private float FrozenSlowModifier;
        private float FrozenDurationRemaining;

        protected virtual void Awake()
        {
            if (GameManager.Instance && !GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Add(this);
        }

        protected void Update()
        {
            if (!InFlamethrower && OnFireStacks > 0)
            {
                if (OnFireDurationRemaining > 0)
                {
                    OnFireDurationRemaining -= Time.deltaTime;
                }
                else
                {
                    TakeDamage(OnFireChipDamage, Color.red, 0.75f);
                    OnFireStacks--;
                    OnFireDurationRemaining = OnFireStackDuration;
                }
            }

            if (FrozenStacks > 0)
            {
                if (FrozenDurationRemaining > 0)
                {
                    FrozenDurationRemaining -= Time.deltaTime;
                }
                else
                {
                    FrozenStacks--;
                    FrozenDurationRemaining = FrozenStackDuration;
                }
            }
        }

        public void TakeDamage(float damage, Color textColor, float textSizeMultiplier = 1)
        {
            Debug.Log(damage.ToString());
            
            var pulseText = Instantiate(GameManager.Instance.PulseTextPrefab, transform.position, Quaternion.identity);
            pulseText.ShowText(damage.ToString(), textColor, textSizeMultiplier);
            
            Health -= damage;

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
        
        public void HitByFlamethrower(float damageTaken)
        {
            TakeDamage(damageTaken, Color.red);
            OnFireStacks = MaxOnFireStacks;
            OnFireDurationRemaining = OnFireStackDuration;
        }
        
        public void HitByCryo(float damageTaken, float frozenModifier)
        {
            TakeDamage(damageTaken, Color.cyan);
            FrozenSlowModifier = frozenModifier;
            FrozenStacks = MaxFrozenStacks;
            FrozenDurationRemaining = FrozenStackDuration;
        }

        private void OnDestroy()
        {
            if (GameManager.HasInstance && GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Remove(this);
        }

        public void BecomeElemental(Element element)
        {
            Debug.Log($"Enemy given {element}");
        }
    }
}
