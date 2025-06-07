using System;
using UnityEngine;

[CreateAssetMenu]
public class LevelInfo : ScriptableObject
{
    public string LevelName;
    public Background BackgroundToShow;
    public Wave[] Waves;
}