using System;
using System.Collections;
using CamLib;
using UnityEngine;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        //Constants
        //-------------------------------------------

        private const int BaseHealth = 50;
        private const float BaseMovementSpeed = 10000f;
        private const float BaseActionCooldownDuration = 2f;
        private const float BaseAttackDistance = 50f;
        
        private const int MaxOnFireStacks = 10;
        private const float OnFireStackDuration = 1;
        private const float OnFireChipDamage = 5f;
        
        private const int MaxFrozenStacks = 10;
        private const float FrozenStackDuration = 1;
        
        //-------------------------------------------

        [SerializeField] private float Health = BaseHealth;
        [SerializeField] private float MoveSpeed = BaseMovementSpeed;
        [Space]
        [SerializeField] private float ActionCooldownDuration = BaseActionCooldownDuration;
        [SerializeField] private float DistanceToAttackPlayer = BaseAttackDistance;
        [SerializeField] private bool RetreatIfTooClose;
        [Space]
        public EnemyType Type;
        public EnemyBehaviour EnemyBehaviour;
        [Space]
        public Animator Animator;
        public string AnimAction = "attack";
        public string AnimHit = "hit";
        public string AnimDeath = "death";
        public float AnimDeathTiming = 0.5f;
        
        public Rigidbody2D Rigidbody;
        public Transform ProjectileSpawnPosition;
        
        [NonSerialized] public float MoveSpeedModifier = 1f;
        [NonSerialized] public bool InFlamethrower;
        [NonSerialized] public bool InCryoBeam;

        [NonSerialized] public bool BlockActions = true;
        [NonSerialized] public bool BlockMovement;

        private Element Element;
        
        private Vector3 OverrideDestination;
        
        private int OnFireStacks;
        private float OnFireDurationRemaining;

        private int FrozenStacks;
        private float FrozenSlowModifier = 1f;
        private float FrozenDurationRemaining;
        
        private float CooldownRemaining;

        private bool SpawnComplete;
        private bool Destroying;

        private void Awake()
        {
            if (GameManager.Instance && !GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Add(this);
            
            //In case it isn't assigned in the Inspector.
            EnemyBehaviour = GetComponent<EnemyBehaviour>();
        }

        private void Update()
        {
            if (CooldownRemaining > 0)
            {
                CooldownRemaining -= Time.deltaTime;
            }
            else if (!BlockActions && EnemyBehaviour && EnemyBehaviour.AllowedToDoAction())
            {
                CooldownRemaining = ActionCooldownDuration;
                
                if (!Destroying && Vector2.Distance(GameManager.Instance.Player.transform.position, transform.position) <= DistanceToAttackPlayer)
                {
                    CooldownRemaining = ActionCooldownDuration;
                    DoAction();
                }
            }

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
                    FrozenSlowModifier = 1f;
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
                HandleMovement();
            }
        }
        
        private void HandleMovement()
        {
            if (Vector2.Distance(OverrideDestination, transform.position) > .1f)
            {
                var direction = (OverrideDestination - transform.position).normalized;
                
                var force = direction * (MoveSpeedModifier * FrozenSlowModifier * MoveSpeed * Time.deltaTime);
                Rigidbody.AddForce(force);
            }
            else
            {
                transform.position = OverrideDestination;
                OverrideDestination = Vector3.zero;

                if (!SpawnComplete)
                {
                    EnemySpawningComplete();
                }
            }
        }
        
        public void EnemySpawningComplete()
        {
            SpawnComplete = true;
            BlockActions = false;
            
            if (EnemyBehaviour) EnemyBehaviour.OnSpawnComplete();
        }

        public void SetOverrideDestination(Vector3 overrideDestination)
        {
            OverrideDestination = overrideDestination;
        }

        public void DoAction()
        {
            SetTrigger(AnimAction);
            
            EnemyBehaviour.DoAction();
        }

        public AudioSource SfxHurt;
        public AudioSource SfxDeath;
        public AudioSource SfxShoot;
        
        public void TakeDamage(float damage, Element element, Color textColor, float textSizeMultiplier = 1)
        {
            var pulseText = Instantiate(GameManager.Instance.PulseTextPrefab, transform.position + Vector3.up, Quaternion.identity);
            pulseText.ShowText(damage.ToString(), textColor, textSizeMultiplier);

            GameManager.Instance.AddPoints((int)damage);
            
            Health -= damage;

            if (Health <= 0)
            {
                DefeatEnemy(element);
                SfxDeath.Play();
                
                MainCamera.Instance.ShakeCamera(0.05f, 0.3f);
            }
            else
            {
                SetTrigger(AnimHit);
                
                SfxHurt.Play();

                if (EnemyBehaviour)
                {
                    EnemyBehaviour.OnTakeDamage();
                }
            }
        }
        
        public void HitByFlamethrower(float damageTaken)
        {
            TakeDamage(damageTaken, Element.Fire, GameManager.Instance.GetElementColor(Element.Fire));
            OnFireStacks = MaxOnFireStacks;
            OnFireDurationRemaining = OnFireStackDuration;
        }
        
        public void HitByCryo(float damageTaken, float frozenModifier)
        {
            TakeDamage(damageTaken, Element.Ice, GameManager.Instance.GetElementColor(Element.Ice));
            FrozenSlowModifier = frozenModifier;
            FrozenStacks = MaxFrozenStacks;
            FrozenDurationRemaining = FrozenStackDuration;
        }

        public void BecomeElemental(Element element)
        {
            Element = element;
        }

        public Element GetElement()
        {
            return Element;
        }

        public float GetDistanceToAttackPlayer()
        {
            return DistanceToAttackPlayer;
        }

        public void DefeatEnemy(Element element)
        {
            Destroying = true;


            
            
            //Animator.SetTrigger("die");
            //SfxSuckDefeat.Play();
            
            StartCoroutine(CoDefeated());

            IEnumerator CoDefeated()
            {
                SetTrigger(AnimDeath);
                
                yield return new WaitForSeconds(AnimDeathTiming);

                Destroy(gameObject);
                
                EvolutionManager.Instance.Increment(element);
                EnemySpawner.Instance.AddDifficulty();
				EnemyDirector.Instance.EnemyKilled();

                if (EnemySpawner.Instance.ShouldSpawnBrain())
                {
                    EnemySpawner.Instance.SpawnEnemy(EnemyType.BrainShip);
                }

                int pointOnKill = 100;
                
                GameManager.Instance.AddPoints(pointOnKill);
                
                var pulseText = Instantiate(GameManager.Instance.PulseTextPrefab, transform.position + Vector3.up, Quaternion.identity);
                pulseText.ShowText(pointOnKill.ToString(), Color.white, 1.5f);
                
            }
        }

        public bool IsBeingDestroyed()
        {
            return Destroying;
        }
        
        private void OnDestroy()
        {
            if (GameManager.HasInstance && GameManager.Instance.Enemies.Contains(this))
                GameManager.Instance.Enemies.Remove(this);
        }

        public void SetTrigger(string anim)
        {
            if (Animator && !anim.IsNullOrEmpty())
            {
                Animator.SetTrigger(anim);
            }
        }
    }
}