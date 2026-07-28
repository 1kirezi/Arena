using NUnit.Framework;
using Arena.Game.Systems;

namespace Arena.Tests;

[TestFixture]
public class WaveManagerTests
{
    [Test]
    public void StartNextWave_IncrementsWaveNumber()
    {
        var waveManager = new WaveManager();
        waveManager.StartNextWave();
        Assert.That(waveManager.CurrentWave, Is.EqualTo(1));
    }

    [Test]
    public void HealthMultiplier_ScalesCorrectly()
    {
        var waveManager = new WaveManager();
        waveManager.StartNextWave(); // Wave 1
        Assert.That(waveManager.HealthMultiplier, Is.EqualTo(1f));
        
        waveManager.StartNextWave(); // Wave 2
        Assert.That(waveManager.HealthMultiplier, Is.EqualTo(1.1f));
    }

    [Test]
    public void SpeedMultiplier_ScalesCorrectly()
    {
        var waveManager = new WaveManager();
        waveManager.StartNextWave(); // Wave 1
        Assert.That(waveManager.SpeedMultiplier, Is.EqualTo(1f));
        
        waveManager.StartNextWave(); // Wave 2
        Assert.That(waveManager.SpeedMultiplier, Is.EqualTo(1.05f));
    }

    [Test]
    public void ZombieKilled_WhenAllKilled_CompletesWave()
    {
        var waveManager = new WaveManager();
        waveManager.StartNextWave();
        int total = waveManager.GetRemainingZombiesToSpawn();
        
        for (int i = 0; i < total; i++)
        {
            waveManager.ZombieSpawned();
            waveManager.ZombieKilled();
        }
        
        Assert.That(waveManager.WaveActive, Is.False);
    }

    [Test]
    public void WaveCompleted_Event_Fires()
    {
        var waveManager = new WaveManager();
        bool eventFired = false;
        waveManager.WaveCompleted += (wave) => eventFired = true;
        
        waveManager.StartNextWave();
        int total = waveManager.GetRemainingZombiesToSpawn();
        for (int i = 0; i < total; i++)
        {
            waveManager.ZombieSpawned();
            waveManager.ZombieKilled();
        }
        
        Assert.That(eventFired, Is.True);
    }
}