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

    public List<EnemyPrefab> Prefabs;
    
    [Serializable]
    public class EnemyPrefab
    {
        public EnemyType Type;
        public GameObject Prefab;
        public Transform SpawnStart;
        public Transform SpawnEnd;
        
        public Vector3 GetRandomSpawn() => Vector3.Lerp(SpawnStart.position, SpawnEnd.position, Random.value);
    }

    private bool IsNextElemental()
    {
        float chance = DifficultyCurve.Evaluate(DifficultyFactor);
        return Random.value < chance;
    }
        
    public void SpawnEnemy(EnemyType enemy)
    {
        EnemyPrefab prefab = Prefabs.Find(p => p.Type == enemy);
        GameObject enemyObj = Instantiate(prefab.Prefab, prefab.GetRandomSpawn(), Quaternion.identity);

        if (IsNextElemental())
        {
            Element element = EvolutionManager.Instance.GetRandomElementalType();
            enemyObj.GetComponent<EnemyController>().BecomeElemental(element);
        }
    }

    public void AddDifficulty(float more = 0.05f)
    {
        DifficultyFactor = Mathf.Clamp01(DifficultyFactor + more);
    }
}