using System;
using System.Collections;
using System.Collections.Generic;
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
        Incendiary,
        Cryo,
        Lightning,
        Shotgun,
        AddShip1,
        AddShip2
    }
    public class PlayerController : MonoBehaviour
    {
        //Constants
        //-------------------------------------------

        public const float BaseShipSpeed = 5;

        public const float Speed1Modifier = 1.2f;
        public const float Speed2Modifier = 1.4f;
        public const float Speed3Modifier = 1.6f;

        public const float BaseLaserCooldown = .33f;

        public const float LaserCooldown1Modifier = .75f;
        public const float LaserCooldown2Modifier = .5f;
        public const float LaserCooldown3Modifier = .25f;

        public const float LaserDamage1Modifier = 1.25f;
        public const float LaserDamage2Modifier = 1.5f;
        public const float LaserDamage3Modifier = 2f;

        public const float ShotgunSpread = 5f;
        public const int ShotgunHowMany = 3;

        //-------------------------------------------

        //Prefabs
        //-------------------------------------------
        public ProjectileLaser LaserProjectile;
        //-------------------------------------------
        [Space]

        public Rigidbody2D RigidBody;
        public Transform ProjectileSpawnPosition;

        public List<Upgrade> Upgrades;

        private Vector2 InputDirection = Vector2.zero;
        private float CurrentLaserCooldown;

        #region Modifiers
        private float SpeedModifier
        {
            get 
            {
                if (Upgrades.Contains(Upgrade.EngineSpeed3))
                    return Speed3Modifier;
                else if (Upgrades.Contains(Upgrade.EngineSpeed2))
                    return Speed2Modifier;
                else if (Upgrades.Contains(Upgrade.EngineSpeed1))
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
                else if (Upgrades.Contains(Upgrade.LaserCooldown2))
                    return LaserCooldown2Modifier;
                else if (Upgrades.Contains(Upgrade.LaserCooldown1))
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
                else if (Upgrades.Contains(Upgrade.LaserDamage2))
                    return LaserDamage2Modifier;
                else if (Upgrades.Contains(Upgrade.LaserDamage1))
                    return LaserDamage1Modifier;

                return 1;
            }
        }
        #endregion

        public void Awake()
        {
            Upgrades = new List<Upgrade>();
        }

        public void Update()
        {
            if (CurrentLaserCooldown > 0)
                CurrentLaserCooldown -= Time.deltaTime;

            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            {
                if (Upgrades.Contains(Upgrade.Shotgun))
                    ShootShotgun();
                else
                    ShootLaser();
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
    }