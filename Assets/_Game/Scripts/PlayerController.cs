using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Projectiles;
using UnityEngine;

    public enum Upgrade
    {
        EngineSpeed1,
        EngineSpeed2,
        EngineSpeed3,
        LaserDamage1,
        LaserDamage2,
        LaserDamage3,
        LaserCooldown1,
        LaserCooldown2,
        LaserCooldown3,
        Flamethrower1,
        Flamethrower2,
        Flamethrower3,
        Cryo1,
        Cryo2,
        Cryo3,
        Lightning1,
        Lightning2,
        Lightning3,
        Shotgun,
        AddShip1,
        AddShip2
    }
    public class PlayerController : MonoBehaviour
    {
        //Constants
        //-------------------------------------------

        //Speed
        private const float BaseShipSpeed = 5f;

        private const float Speed1Modifier = 1.2f;
        private const float Speed2Modifier = 1.4f;
        private const float Speed3Modifier = 1.6f;

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
        private const float BaseFlamethrowerDamageCooldown = .5f;
        private const float BaseFlamethrowerDamage = 10f;
        
        private const float FlamethrowerDamage1Modifier = 1f;
        private const float FlamethrowerDamage2Modifier = 1.5f;
        private const float FlamethrowerDamage3Modifier = 2f;
        
        //Cryo
        private const float BaseCryoDamageCooldown = .2f;
        private const float BaseCryoDamage = 4f;
        
        private const float CryoDamage1Modifier = 1f;
        private const float CryoDamage2Modifier = 1.5f;
        private const float CryoDamage3Modifier = 2f;
        
        private const float CryoFrozen1Modifier = 1f;
        private const float CryoFrozen2Modifier = 1.5f;
        private const float CryoFrozen3Modifier = 2f;
        
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

        public Rigidbody2D RigidBody;
        public Transform ProjectileSpawnPosition;

        public GameObject FlamethrowerCone;

        public List<Upgrade> Upgrades = new List<Upgrade>();

        private int CurrentWeaponIndex;

        private Vector2 InputDirection = Vector2.zero;
        private float CurrentLaserCooldown;
        private float CurrentFlamethrowerDamageCooldown;
        private float CurrentCryoDamageCooldown;
        private float CurrentLightningCooldown;

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
                if (Upgrades.Contains(Upgrade.LaserCooldown3))
                    return LaserCooldown3Modifier;
                if (Upgrades.Contains(Upgrade.LaserCooldown2))
                    return LaserCooldown2Modifier;
                if (Upgrades.Contains(Upgrade.LaserCooldown1))
                    return LaserCooldown1Modifier;

                return 1;
            }
        }

        private float LaserDamageModifier
        {
            get
            {
                if (Upgrades.Contains(Upgrade.LaserDamage3))
                    return LaserDamage3Modifier;
                if (Upgrades.Contains(Upgrade.LaserDamage2))
                    return LaserDamage2Modifier;
                if (Upgrades.Contains(Upgrade.LaserDamage1))
                    return LaserDamage1Modifier;

                return 1;
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

                return 1;
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

                return 1;
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

                return .5f;
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

                return 1;
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

                return 1;
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

                return 1;
            }
        }
        #endregion

        public void Awake()
        {
            
        }

        public void Update()
        {
            if (CurrentLaserCooldown > 0)
                CurrentLaserCooldown -= Time.deltaTime;
            
            if (CurrentCryoDamageCooldown > 0)
                CurrentCryoDamageCooldown -= Time.deltaTime;
            
            if (CurrentLightningCooldown > 0)
                CurrentLightningCooldown -= Time.deltaTime;

            if (Input.GetMouseButton(0)) //TODO Change this to use input system
            {
                Shoot();
            }
            else
            {
                StopShooting();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                NextWeapon();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                PreviousWeapon();
            }

            InputDirection = Vector2.zero;
            InputDirection.x += Input.GetAxisRaw("Horizontal");
            InputDirection.y += Input.GetAxisRaw("Vertical");
            InputDirection = InputDirection.normalized;
        }
        public void FixedUpdate()
        {
            RigidBody.linearVelocity = BaseShipSpeed * SpeedModifier * InputDirection;
        }
        
        public void Shoot()
        {
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

        public void StopShooting()
        {   
            FlamethrowerCone.SetActive(false);
        }

        public void ShootLaser()
        {
            if (CurrentLaserCooldown > 0)
                return;

            var laser = Instantiate(LaserProjectile);
            laser.transform.position = ProjectileSpawnPosition.position;

            var direction = Vector2.right;

            laser.Setup(direction.normalized, LaserDamageModifier);

            CurrentLaserCooldown = BaseLaserCooldown * LaserCooldownModifier;
        }

        public void ShootShotgun()
        {
            if (CurrentLaserCooldown > 0)
                return;

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

        public void ShootFlamethrower()
        {
            FlamethrowerCone.SetActive(true);
        }

        public void ShootCryoGun()
        {
            if (CurrentCryoDamageCooldown > 0)
                return;
            
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

        public void ShootLightningGun()
        {
            if (CurrentLightningCooldown > 0)
                return;

            var lightning = Instantiate(LightningProjectile);
            lightning.transform.position = ProjectileSpawnPosition.position;

            var direction = Vector2.right;

            lightning.Setup(direction.normalized, LightningDamageModifier, LightningChainModifier);

            CurrentLightningCooldown = LightningCooldownModifier;
        }
        
        private void NextWeapon()
        {
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
                            NextWeapon();
                        }

                        break;

                    case 2: //Cryo
                        if (!Upgrades.Contains(Upgrade.Cryo1))
                        {
                            NextWeapon();
                        }

                        break;

                    case 3: //Lightning
                        if (!Upgrades.Contains(Upgrade.Lightning1))
                        {
                            NextWeapon();
                        }

                        break;
                }
            }
            
            Debug.Log($"Weapon Index: {CurrentWeaponIndex}");
        }
        
        private void PreviousWeapon()
        {
            if (CurrentWeaponIndex == 0)
            {
                CurrentWeaponIndex = 3;
            }
            else
            {
                CurrentWeaponIndex--;
            }
            
            switch (CurrentWeaponIndex)
            {
                case 1: //Flamethrower
                    if (!Upgrades.Contains(Upgrade.Flamethrower1))
                    {
                        PreviousWeapon();
                    }
                    break;
                
                case 2: //Cryo
                    if (!Upgrades.Contains(Upgrade.Cryo1))
                    {
                        PreviousWeapon();
                    }
                    break;
                
                case 3: //Lightning
                    if (!Upgrades.Contains(Upgrade.Lightning1))
                    {
                        PreviousWeapon();
                    }
                    break;
            }
            
            Debug.Log($"Weapon Index: {CurrentWeaponIndex}");
        }
    }