using System;
using System.Collections.Generic;
using CamLib;
using Enemies;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Singleton<EnemySpawner>
{
    public float DifficultyFactor; 
    public AnimationCurve DifficultyCurve;
    public AnimationCurve BrainSpawnCurve;

    public Transform EnemyDespawnPoint;
    public List<EnemyPrefab> Prefabs;
    
    [Serializable]
    public class EnemyPrefab
    {
        public EnemyType Type;
        
        [SerializeReference] 
        public EnemyController Prefab;
        
        public Transform SpawnStart;
        public Transform SpawnEnd;

        public Vector3 SpawnPoint => Vector3.Lerp(SpawnStart.position, SpawnEnd.position, Random.value);
    }

    private bool IsNextElemental()
    {
        float chance = DifficultyCurve.Evaluate(DifficultyFactor);
        return Random.value < chance;
    }
    
    public bool ShouldSpawnBrain()
    {
        float chance = BrainSpawnCurve.Evaluate(DifficultyFactor);
        return Random.value < chance;
    }
        
    public void SpawnEnemy(EnemyType enemyType)
    {
        EnemyPrefab prefab = Prefabs.Find(p => p.Type == enemyType);
        
        EnemyController enemy = Instantiate(prefab.Prefab, prefab.SpawnPoint, Quaternion.identity);

        if (IsNextElemental())
        {
            Element element = EvolutionManager.Instance.GetRandomElementalType();
            enemy.BecomeElemental(element);
        }
    }

    public void AddDifficulty(float more = 0.05f)
    {
        DifficultyFactor = Mathf.Clamp01(DifficultyFactor + more);
        HudBarDifficulty.Instance.UpdateBar(DifficultyFactor);
    }
}