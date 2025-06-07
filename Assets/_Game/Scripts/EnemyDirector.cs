using System;
using System.Collections;
using System.Collections.Generic;
using CamLib;
using Enemies;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// The game contains X levels with a set piece.
/// A level contains X waves.
/// A level is spaced apart with a breather which shows your points, annds and drops points based on things that happened in the level. and you pick an upgrade
/// 
/// A wave contains a bunch of enemies.
/// A wave will attempt to spawn everything, but is limited because of X maximum 
/// </summary>
public class EnemyDirector : Singleton<EnemyDirector>
{
    public Wave[] CurrentWaves;
    public Wave CurrentWave;
    
    public float CurrentWaveIndex;
    public int CurrentWaveEnemiesKilled;

    public IEnumerator SpawnWaves(Wave[] waves)
    {
        CurrentWaves = waves;
        Debug.Log("Spawn waves");
        for (int i = 0; i < waves.Length; i++)
        {
            Wave wave = waves[i];
            CurrentWave = wave;
            CurrentWaveIndex = i;
            CurrentWaveEnemiesKilled = 0;
            
            EnemyGroup.Instance.SetNewFormation();

            GameManager.Instance.UpdateTotalProgress();
            
            yield return SpawnWave(wave);

            //have a breather between each wave. just a short one
            Debug.Log("wave interval");
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator SpawnWave(Wave wave)
    {
        var interval = new WaitForSeconds(0.4f);;
        var wait = new WaitUntil(() =>
        {
            //Debug.Log($"wait to spawn enemies: {GameManager.Instance.Enemies.Count} / {wave.MaxOnScreen}");
            return GameManager.Instance.Enemies.Count < wave.MaxOnScreen;
        });
        
        var waitForAllDead = new WaitUntil(() =>
        {
            //Debug.Log($"wait to spawn enemies: {GameManager.Instance.Enemies.Count} / {wave.MaxOnScreen}");
            return GameManager.Instance.Enemies.Count <= 0;
        });
        
        foreach (EnemyType enemy in wave.Enemies)
        {
            EnemySpawner.Instance.SpawnEnemy(enemy);
            yield return interval;
            yield return wait;
        }

        yield return waitForAllDead;
    }

    public void EnemyKilled()
    {
        CurrentWaveEnemiesKilled++;
        GameManager.Instance.UpdateTotalProgress();
    }
}