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

        private const int BaseHealth = 50;
        private const float BaseMovementSpeed = 5f;
        
        private const int BaseAttackDistance = 50;
        
        private const int MaxOnFireStacks = 10;
        private const float OnFireStackDuration = 1;
        private const float OnFireChipDamage = 5f;
        
        private const int MaxFrozenStacks = 10;
        private const float FrozenStackDuration = 1;
        
        //-------------------------------------------

        [SerializeField] private float Health = BaseHealth;
        [SerializeField] private float MoveSpeed = BaseMovementSpeed;
        [SerializeField] private float DistanceToAttackPlayer = BaseAttackDistance;
        
        [SerializeField] private bool RetreatIfTooClose;
        
        public EnemyType Type;
        public EnemyBehaviour EnemyBehaviour;
        
        public Animator Animator;
        public Rigidbody2D Rigidbody;
        
        [NonSerialized] public bool InFlamethrower;
        [NonSerialized] public bool InCryoBeam;

        private bool BlockActions = true;
        private bool BlockMovement = true;
        
        private Vector3 OverrideDestination;
        private float MoveSpeedModifier = 1f;
        
        private float CooldownRemaining;
        
        private int OnFireStacks;
        private float OnFireDurationRemaining;

        private int FrozenStacks;
        private float FrozenSlowModifier;
        private float FrozenDurationRemaining;

        private void Awake()
        {
            if (GameManager.Instance && !GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Add(this);
            
            //In case it isn't assigned in the Inspector.
            EnemyBehaviour = GetComponent<EnemyBehaviour>();
        }

        private void Update()
        {
            if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

            if (!InFlamethrower && OnFireStacks > 0)
            {
                if (OnFireDurationRemaining > 0)
                {
                    OnFireDurationRemaining -= Time.deltaTime;
                }
                else
                {
                    TakeDamage(OnFireChipDamage, Element.Fire, Color.red, 0.75f);
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
        
        private void FixedUpdate()
        {
            Rigidbody.linearVelocity = Vector2.zero;
            
            if (!BlockMovement && OverrideDestination != Vector3.zero)
            {
                if (Vector2.Distance(OverrideDestination, transform.position) > .1f)
                {
                    var direction = (OverrideDestination - transform.position).normalized;
                    Rigidbody.AddForce(direction * (MoveSpeedModifier * MoveSpeed * Time.deltaTime));
                }
                else
                {
                    transform.position = OverrideDestination;
                    OverrideDestination = Vector3.zero;
                }
            }
        }

        public void EnemySpawningComplete()
        {
            BlockActions = false;
            BlockMovement = false;
        }

        public void SetOverrideDestination(Vector3 overrideDestination)
        {
            OverrideDestination = overrideDestination;
        }

        public void DoAction()
        {
            EnemyBehaviour.DoAction();
        }
        
        
        public void TakeDamage(float damage, Element element, Color textColor, float textSizeMultiplier = 1)
        {
            var pulseText = Instantiate(GameManager.Instance.PulseTextPrefab, transform.position, Quaternion.identity);
            pulseText.ShowText(damage.ToString(), textColor, textSizeMultiplier);
            
            Health -= damage;

            if (Health <= 0)
            {
                Destroy(gameObject);
                
                EvolutionManager.Instance.Increment(element);
                EnemySpawner.Instance.AddDifficulty();
                EnemyDirector.Instance.EnemyKilled();
            }
        }
        
        public void HitByFlamethrower(float damageTaken)
        {
            TakeDamage(damageTaken, Element.Fire, Color.red);
            OnFireStacks = MaxOnFireStacks;
            OnFireDurationRemaining = OnFireStackDuration;
        }
        
        public void HitByCryo(float damageTaken, float frozenModifier)
        {
            TakeDamage(damageTaken, Element.Ice, Color.cyan);
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

        public void MoveTowards(Vector3 destination)
        {
            float currentSpeed = MoveSpeed;
            
            if (FrozenStacks > 0)
            {
                currentSpeed *= (1f - FrozenSlowModifier);
            }
            
            transform.position = Vector3.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
        }
    }
}