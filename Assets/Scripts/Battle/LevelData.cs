using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public List<LaneWaveData> lanes = new List<LaneWaveData>();
}

[System.Serializable]
public class LaneWaveData
{
    public float laneZ; // Z position for this lane
    public List<WaveData> waves = new List<WaveData>();
}

[System.Serializable]
public class WaveData
{
    public float startDelay; // Delay before wave starts
    public EnemyData enemyType; // Type of enemy to spawn
    public int enemyCount; // Number of enemies in wave
    public float delayBetweenEnemies; // Time between enemy spawns
} 