using System;
using System.Collections;
using System.Collections.Generic;
using CamLib;
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
    

    public IEnumerator SpawnWaves(Wave[] waves)
    {
        Debug.Log("Spawn waves");
        foreach (Wave wave in waves)
        {
            yield return SpawnWave(wave);
            
            //have a breather between each wave. just a short one
            Debug.Log("wave interval");
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator SpawnWave(Wave wave)
    {
        var wait = new WaitUntil(() =>
        {
            //Debug.Log($"wait to spawn enemies: {GameManager.Instance.Enemies.Count} / {wave.MaxOnScreen}");
            return GameManager.Instance.Enemies.Count < wave.MaxOnScreen;
        });
        foreach (EnemyType enemy in wave.Enemies)
        {
            EnemySpawner.Instance.SpawnEnemy(enemy);
            yield return wait;
        }
    }
}