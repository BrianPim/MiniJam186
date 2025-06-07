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
        foreach (Wave wave in waves)
        {
            yield return SpawnWave(wave);
            
            //have a breather between each wave. just a short one
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator SpawnWave(Wave wave)
    {
        WaitUntil canSpawnAnotherEnemy = new WaitUntil(() => GameManager.Instance.Enemies.Count < wave.MaxOnScreen);

        foreach (EnemyType enemy in wave.Enemies)
        {
            EnemySpawner.Instance.SpawnEnemy(enemy);
            yield return canSpawnAnotherEnemy;
        }
    }
}