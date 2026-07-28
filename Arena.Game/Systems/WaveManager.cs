using Arena.Game.Core;
using System;

namespace Arena.Game.Systems;

public class WaveManager
{
    private int _currentWave = 0;
    private int _zombiesSpawnedInWave = 0;
    private int _zombiesKilledInWave = 0;
    private int _totalZombiesInWave = 0;
    private bool _waveActive = false;

    public int CurrentWave => _currentWave;
    public bool WaveActive => _waveActive;
    public float HealthMultiplier => 1f + (_currentWave - 1) * 0.1f;
    public float SpeedMultiplier => 1f + (_currentWave - 1) * 0.05f;

    public event Action<int> WaveStarted;
    public event Action<int> WaveCompleted;

    public void StartNextWave()
    {
        _currentWave++;
        _zombiesSpawnedInWave = 0;
        _zombiesKilledInWave = 0;
        _totalZombiesInWave = Config.BaseSpawnCount + _currentWave * 2;
        _waveActive = true;

        WaveStarted?.Invoke(_currentWave);
    }

    public void ZombieSpawned()
    {
        _zombiesSpawnedInWave++;
    }

    public void ZombieKilled()
    {
        _zombiesKilledInWave++;
        if (_zombiesKilledInWave >= _totalZombiesInWave && _waveActive)
        {
            _waveActive = false;
            WaveCompleted?.Invoke(_currentWave);
        }
    }

    public bool ShouldSpawnMore()
    {
        return _waveActive && _zombiesSpawnedInWave < _totalZombiesInWave;
    }

    public int GetRemainingZombiesToSpawn()
    {
        return _totalZombiesInWave - _zombiesSpawnedInWave;
    }

    public void Reset()
    {
        _currentWave = 0;
        _waveActive = false;
        _zombiesSpawnedInWave = 0;
        _zombiesKilledInWave = 0;
        _totalZombiesInWave = 0;
    }
}