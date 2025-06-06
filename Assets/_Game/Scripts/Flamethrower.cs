using System;
using Enemies;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();

        if (enemy)
        {
            enemy.InFlamethrower = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();

        if (enemy)
        {
            enemy.InFlamethrower = false;
        }
    }
}
