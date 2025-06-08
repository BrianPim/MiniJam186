using System;
using System.Collections;
using System.Collections.Generic;
using CamLib;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Enemies;
using JetBrains.Annotations;
using Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum Upgrade
    {
        EngineSpeed1,
        EngineSpeed2,
        EngineSpeed3,
        Laser1,
        Laser2,
        Laser3,
        Shotgun,
        Flamethrower1,
        Flamethrower2,
        Flamethrower3,
        Cryo1,
        Cryo2,
        Cryo3,
        Lightning1,
        Lightning2,
        Lightning3,
        AddShip1,
        AddShip2
    }
    public class PlayerController : MonoBehaviour
    {
        //Constants
        //-------------------------------------------

        private const int BaseHealth = 3;
        private const float BaseIFramesDuration = 0.5f;
        
        //Speed
        private const float BaseShipSpeed = 5f;

        private const float Speed1Modifier = 1.2f;
        private const float Speed2Modifier = 1.4f;
        private const float Speed3Modifier = 1.6f;
        
        private const float BaseDodgeModifier = 5f;
        private const float BaseDodgeDuration = .1f;

        //Lasers
        private const float BaseLaserCooldown = .33f;

        private const float LaserCooldown1Modifier = .75f;
        private const float LaserCooldown2Modifier = .5f;
        private const float LaserCooldown3Modifier = .25f;

        private const float LaserDamage1Modifier = 1.25f;
        private const float LaserDamage2Modifier = 1.5f;
        private const float LaserDamage3Modifier = 2f;

        private const float ShotgunSpread = 5f;
        private const int ShotgunHowMany = 3;
        
        //Flamethrower
        private const float BaseFlamethrowerDamageCooldown = .2f;
        private const float BaseFlamethrowerDamage = 4f;
        
        private const float FlamethrowerDamage1Modifier = 1f;
        private const float FlamethrowerDamage2Modifier = 1.5f;
        private const float FlamethrowerDamage3Modifier = 2f;
        
        private const float FlamethrowerConeDistance1Modifier = 5f;
        private const float FlamethrowerConeDistance2Modifier = 7.5f;
        private const float FlamethrowerConeDistance3Modifier = 10f;
        
        private const float FlamethrowerConeAngle1Modifier = 30f;
        private const float FlamethrowerConeAngle2Modifier = 37.5f;
        private const float FlamethrowerConeAngle3Modifier = 45f;
        
        //Cryo
        private const float BaseCryoDamageCooldown = .2f;
        private const float BaseCryoDamage = 4f;
        
        private const float CryoDamage1Modifier = 1f;
        private const float CryoDamage2Modifier = 1.5f;
        private const float CryoDamage3Modifier = 2f;
        
        private const float CryoFrozen1Modifier = .75f;
        private const float CryoFrozen2Modifier = .5f;
        private const float CryoFrozen3Modifier = .25f;
        
        private const float CryoCircleCastRadius1Modifier = .5f;
        private const float CryoCircleCastRadius2Modifier = 1f;
        private const float CryoCircleCastRadius3Modifier = 1.5f;
        
        private const float CryoCircleCastRange = 100f;
        
        //Lightning
        private const float LightningCooldown1Modifier = .75f;
        private const float LightningCooldown2Modifier = .5f;
        private const float LightningCooldown3Modifier = .25f;

        private const float LightningDamage1Modifier = 1f;
        private const float LightningDamage2Modifier = 1.5f;
        private const float LightningDamage3Modifier = 2f;
        
        private const int LightningChain1Modifier = 1;
        private const int LightningChain2Modifier = 2;
        private const int LightningChain3Modifier = 3;

        //-------------------------------------------

        //Prefabs
        //-------------------------------------------
        public ProjectileLaser LaserProjectile;
        public ProjectileLightning LightningProjectile;
        //-------------------------------------------

        [Space] 
        
        public InputAction PlayerMovement;
        public InputAction PlayerShoot;
        public InputAction PlayerDodge;
        public InputAction PlayerSwitchWeapons;
        
        [Space]
        
        public Rigidbody2D RigidBody;
        public Transform ProjectileSpawnPosition;
        public Animator Animator;
        public MeshRenderer MeshRenderer;

        public Light2D FlamethrowerCone;

        [Space] 
        
        public AudioSource SfxHurt;
        public AudioSource SfxDeath;
        public AudioSource SfxShoot;
        public ParticleSystem ParticlesShoot;
        public GameObject ReviveHeart;
        
        [SerializeField] private int Health = BaseHealth;
        [SerializeField] private float IFramesDuration = BaseIFramesDuration;
        
        [Space]
        
        [SerializeField] private float DodgeModifier = BaseDodgeModifier;
        [SerializeField] private float DodgeDuration = BaseDodgeDuration;
        
        public List<Upgrade> Upgrades = new List<Upgrade>();

        private int CurrentWeaponIndex;

        private Vector2 InputDirection = Vector2.zero;
        private float CurrentLaserCooldown;
        private float CurrentFlamethrowerDamageCooldown;
        private float CurrentCryoDamageCooldown;
        private float CurrentLightningCooldown;
        private float CurrentDodgeCooldown;
        private bool IsShooting;
        private bool Invincible;
        private bool AllowSwitchWeapons = true;

        #region Modifiers
        private float SpeedModifier
        {
            get 
            {
                if (Upgrades.Contains(Upgrade.EngineSpeed3))
                    return Speed3Modifier;
                if (Upgrades.Contains(Upgrade.EngineSpeed2))
                    return Speed2Modifier;
                if (Upgrades.Contains(Upgrade.EngineSpeed1))
                    return Speed1Modifier;

                return 1;
            }
        }

        private float LaserCooldownModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Laser3))
                    return LaserCooldown3Modifier;
                if (Upgrades.Contains(Upgrade.Laser2))
                    return LaserCooldown2Modifier;
                if (Upgrades.Contains(Upgrade.Laser1))
                    return LaserCooldown1Modifier;

                return 1;
            }
        }

        private float LaserDamageModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Laser3))
                    return LaserDamage3Modifier;
                if (Upgrades.Contains(Upgrade.Laser2))
                    return LaserDamage2Modifier;
                if (Upgrades.Contains(Upgrade.Laser1))
                    return LaserDamage1Modifier;

                return 1;
            }
        }
        
        private float FlamethrowerDamageModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Flamethrower3))
                    return FlamethrowerDamage3Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower2))
                    return FlamethrowerDamage2Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower1))
                    return FlamethrowerDamage1Modifier;

                return 0;
            }
        }
        
        private float FlamethrowerConeDistanceModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Flamethrower3))
                    return FlamethrowerConeDistance3Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower2))
                    return FlamethrowerConeDistance2Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower1))
                    return FlamethrowerConeDistance1Modifier;

                return 0;
            }
        }
        
        private float FlamethrowerConeAngleModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Flamethrower3))
                    return FlamethrowerConeAngle3Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower2))
                    return FlamethrowerConeAngle2Modifier;
                if (Upgrades.Contains(Upgrade.Flamethrower1))
                    return FlamethrowerConeAngle1Modifier;

                return 0;
            }
        }
        
        private float CryoDamageModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Cryo3))
                    return CryoDamage3Modifier;
                if (Upgrades.Contains(Upgrade.Cryo2))
                    return CryoDamage2Modifier;
                if (Upgrades.Contains(Upgrade.Cryo1))
                    return CryoDamage1Modifier;

                return 0;
            }
        }
        
        private float CryoFrozenModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Cryo3))
                    return CryoFrozen3Modifier;
                if (Upgrades.Contains(Upgrade.Cryo2))
                    return CryoFrozen2Modifier;
                if (Upgrades.Contains(Upgrade.Cryo1))
                    return CryoFrozen1Modifier;

                return 0;
            }
        }
        
        private float CryoCircleCastRadiusModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Cryo3))
                    return CryoCircleCastRadius3Modifier;
                if (Upgrades.Contains(Upgrade.Cryo2))
                    return CryoCircleCastRadius2Modifier;
                if (Upgrades.Contains(Upgrade.Cryo1))
                    return CryoCircleCastRadius1Modifier;

                return 0;
            }
        }
        
        private float LightningCooldownModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Lightning3))
                    return LightningCooldown3Modifier;
                if (Upgrades.Contains(Upgrade.Lightning2))
                    return LightningCooldown2Modifier;
                if (Upgrades.Contains(Upgrade.Lightning1))
                    return LightningCooldown1Modifier;

                return 100;
            }
        }

        private float LightningDamageModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Lightning3))
                    return LightningDamage3Modifier;
                if (Upgrades.Contains(Upgrade.Lightning2))
                    return LightningDamage2Modifier;
                if (Upgrades.Contains(Upgrade.Lightning1))
                    return LightningDamage1Modifier;

                return 0;
            }
        }
        
        private int LightningChainModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.Lightning3))
                    return LightningChain3Modifier;
                if (Upgrades.Contains(Upgrade.Lightning2))
                    return LightningChain2Modifier;
                if (Upgrades.Contains(Upgrade.Lightning1))
                    return LightningChain1Modifier;

                return 0;
            }
        }
        #endregion

        public bool IsDead => Health <= 0;

        private void Awake()
        {
            Animator.SetInteger("life", Health);
            
            FlamethrowerCone.gameObject.SetActive(false);

            PlayerDodge.started += Dodge;
            
            PlayerShoot.started += StartShooting;
            PlayerShoot.canceled += StopShooting;
            PlayerSwitchWeapons.started += NextWeapon;
        }

        private void Update()
        {
            if (IsDead) return;
            
            if (CurrentLaserCooldown > 0)
                CurrentLaserCooldown -= Time.deltaTime;
            
            if (CurrentFlamethrowerDamageCooldown > 0)
                CurrentFlamethrowerDamageCooldown -= Time.deltaTime;
            
            if (CurrentCryoDamageCooldown > 0)
                CurrentCryoDamageCooldown -= Time.deltaTime;
            
            if (CurrentLightningCooldown > 0)
                CurrentLightningCooldown -= Time.deltaTime;
            
            if (CurrentDodgeCooldown > 0)
                CurrentDodgeCooldown -= Time.deltaTime;

            InputDirection = PlayerMovement.ReadValue<Vector2>();

            if (IsShooting) Shoot();
        }

        private void OnEnable()
        {
            PlayerMovement.Enable();
            PlayerShoot.Enable();
            PlayerDodge.Enable();
            PlayerSwitchWeapons.Enable();
        }
        
        private void OnDisable()
        {
            PlayerMovement.Disable();
            PlayerShoot.Disable();
            PlayerDodge.Disable();
            PlayerSwitchWeapons.Disable();
        }

        private void FixedUpdate()
        {
            if (IsDead)
            {
                RigidBody.linearVelocity = Vector2.zero;
                return;
            }
            
            RigidBody.linearVelocity = BaseShipSpeed * SpeedModifier * InputDirection;

            if (CurrentDodgeCooldown > 0)
                RigidBody.linearVelocity *= DodgeModifier;
            
            Animator.SetBool("Moving", RigidBody.linearVelocity.magnitude > 0.01f);
        }

        private void Dodge(InputAction.CallbackContext CallbackContext)
        {
            CurrentDodgeCooldown = DodgeDuration;
            Animator.SetTrigger("dodge");
        }

        public int GetHealth()
        {
            return Health;
        }

        public void SetHealth(int health)
        {
            Health = health;
        }

        public void TakeDamage(int damage)
        {
            IEnumerator IFrames()
            {
                Invincible = true;
                
                float elapsed = 0;

                while (elapsed < IFramesDuration)
                {
                    MeshRenderer.enabled = !MeshRenderer.enabled;

                    elapsed += .1f;
                    yield return new WaitForSeconds(.1f);
                }

                MeshRenderer.enabled = true;
                Invincible = false;
            }
            
            if (Invincible) return;

            StartCoroutine(IFrames());
            
            Health -= damage;

            Animator.SetInteger("life", Health);
            
            MainCamera.Instance.ShakeCamera(0.1f, 0.5f);
            
            if (Health <= 0)
            {
                //do death stuff
                Animator.SetTrigger("die");
                SfxDeath.Play();
                StopShooting(default);
                GameManager.Instance.AddDeath();

                DoRevive();
            }
            else
            {
                SfxHurt.Play();
            }
        }

        public int GetCurrentWeaponIndex()
        {
            return CurrentWeaponIndex;
        }

        private void StartShooting(InputAction.CallbackContext CallbackContext)
        {
            if (IsDead) return;
            IsShooting = true;
        }
        
        private void Shoot()
        {
            Animator.SetBool("shooting", false);
            
            switch (CurrentWeaponIndex)
            {
                case 1: //Flamethrower
                    ShootFlamethrower();
                    break;
                case 2: //Cryo
                    ShootCryoGun();
                    break;
                case 3: //Lightning
                    ShootLightningGun();
                    break;
                default:
                    if (Upgrades.Contains(Upgrade.Shotgun))
                        ShootShotgun();
                    else
                        ShootLaser();
                    break;
            }
        }

        private void StopShooting(InputAction.CallbackContext CallbackContext)
        {
            IsShooting = false;
            Animator.SetBool("shooting", false);
            FlamethrowerCone.gameObject.SetActive(false);
            //disable cryo laser
        }

        private void ShootLaser()
        {
            if (CurrentLaserCooldown > 0)
                return;

            Animator.SetTrigger("shoot");
            SfxShoot.Play();
            ParticlesShoot.Play();
            
            var laser = Instantiate(LaserProjectile);
            laser.transform.position = ProjectileSpawnPosition.position;

            var direction = Vector2.right;

            laser.Setup(direction.normalized, LaserDamageModifier);

            CurrentLaserCooldown = BaseLaserCooldown * LaserCooldownModifier;
        }

        private void ShootShotgun()
        {
            if (CurrentLaserCooldown > 0)
                return;

            Animator.SetTrigger("shoot");
            
            for(int i = -Mathf.FloorToInt(ShotgunHowMany/2f); i < Mathf.CeilToInt(ShotgunHowMany / 2f); i++)
            {
                var laser = Instantiate(LaserProjectile);
                laser.transform.position = ProjectileSpawnPosition.position;

                var laserAngle = ShotgunSpread * i;

                var angleAxis = Quaternion.AngleAxis(laserAngle, Vector3.forward);
                var direction = angleAxis * Vector2.right;

                laser.transform.Rotate(Vector3.forward, ShotgunSpread * i);
                laser.Setup(direction.normalized, LaserDamageModifier);

                //Debug.Log(laserAngle.ToString());
            }

            CurrentLaserCooldown = BaseLaserCooldown * LaserCooldownModifier;
        }

        private void ShootFlamethrower()
        {
            Animator.SetBool("shooting", true);
            
            FlamethrowerCone.pointLightOuterRadius = FlamethrowerConeDistanceModifier;
            FlamethrowerCone.pointLightInnerAngle = FlamethrowerConeAngleModifier;
            FlamethrowerCone.pointLightOuterAngle = FlamethrowerConeAngleModifier;
            
            FlamethrowerCone.gameObject.SetActive(true);

            if (CurrentFlamethrowerDamageCooldown > 0)
                return;

            foreach (var enemy in GameManager.Instance.Enemies)
            {
                if (FlamethrowerCone.IsInLight(enemy.transform.position))
                {
                    enemy.HitByFlamethrower(BaseFlamethrowerDamage * FlamethrowerDamageModifier);
                }
            }
            
            CurrentFlamethrowerDamageCooldown = BaseFlamethrowerDamageCooldown;
        }

        private void ShootCryoGun()
        {
            if (CurrentCryoDamageCooldown > 0)
                return;
            
            Animator.SetTrigger("shoot");
            
            RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, CryoCircleCastRadiusModifier, Vector3.right, CryoCircleCastRange, LayerMask.GetMask("Enemy"));

            foreach (var hit in results)
            {
                if (!hit.collider) continue;

                hit.transform.gameObject.TryGetComponent<EnemyController>(out var enemy);

                if (enemy)
                {
                    enemy.HitByCryo(BaseCryoDamage * CryoDamageModifier, CryoFrozenModifier);
                }
            }

            CurrentCryoDamageCooldown = BaseCryoDamageCooldown;
        }

        private void ShootLightningGun()
        {
            if (CurrentLightningCooldown > 0)
                return;

            Animator.SetTrigger("shoot");
            
            var lightning = Instantiate(LightningProjectile);
            lightning.transform.position = ProjectileSpawnPosition.position;

            var direction = Vector2.right;

            lightning.Setup(direction.normalized, LightningDamageModifier, LightningChainModifier);

            CurrentLightningCooldown = LightningCooldownModifier;
        }

        public void SetAllowSwitchWeapons(bool allow)
        {
            AllowSwitchWeapons = allow;
        }
        
        private void NextWeapon(InputAction.CallbackContext CallbackContext)
        {
            if (IsDead) return;
            if (!AllowSwitchWeapons) return;
            
            var oldIndex = CurrentWeaponIndex;
            
            if (CurrentWeaponIndex == 3)
            {
                CurrentWeaponIndex = 0;
            }
            else
            {
                CurrentWeaponIndex++;

                //This can go inside this condition because we don't need to consider the switch if CurrentWeaponIndex is 0.
                switch (CurrentWeaponIndex)
                {
                    case 1: //Flamethrower
                        if (!Upgrades.Contains(Upgrade.Flamethrower1))
                        {
                            NextWeapon(CallbackContext);
                        }

                        break;

                    case 2: //Cryo
                        if (!Upgrades.Contains(Upgrade.Cryo1))
                        {
                            NextWeapon(CallbackContext);
                        }

                        break;

                    case 3: //Lightning
                        if (!Upgrades.Contains(Upgrade.Lightning1))
                        {
                            NextWeapon(CallbackContext);
                        }

                        break;
                }
            }
            
            HudManager.Instance.UpdateWeaponWheel(oldIndex);
        }

        public void AddUpgrade(Upgrade upgrade)
        {
            Upgrades.Add(upgrade);
        }

        public Volume DeadVolume;
        void DoRevive()
        {
            ReviveHeart.SetActive(true);
            //ReviveHeart.SetActive(true);
            ReviveHeart.transform.localPosition = Vector2.up * 10;
            ReviveHeart.transform.DOLocalMoveY(0, 2).OnComplete(Revive).SetDelay(1);
            
            MusicPlayer.Instance.SetDead(true);
            DeadVolume.DoFade(1, 0.5f);
        }

        private void Revive()
        {
            Health = 3;
            ReviveHeart.SetActive(false);
            Animator.SetTrigger("revive");
            Animator.SetInteger("life", Health);
            MusicPlayer.Instance.SetDead(false);
            DeadVolume.DoFade(0, 1.0f);
        }
    }

    public static class DoTweenExtensions
    {
        [PublicAPI]
        public static TweenerCore<float, float, FloatOptions> DoFade(this Volume target, float endValue, float duration, bool snapping = false)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.weight, x => target.weight = x, endValue, duration);
            t.SetOptions(snapping).SetTarget(target);
            return t;
        }
    }
    