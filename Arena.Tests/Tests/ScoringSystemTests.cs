using NUnit.Framework;
using Arena.Game.Systems;

namespace Arena.Tests;

[TestFixture]
public class ScoringSystemTests
{
    [Test]
    public void AddPoints_IncreasesScore()
    {
        var scoring = new ScoringSystem();
        scoring.AddPoints(10);
        Assert.That(scoring.Score, Is.EqualTo(10));
    }

    [Test]
    public void AddWaveBonus_AddsCorrectPoints()
    {
        var scoring = new ScoringSystem();
        scoring.AddWaveBonus(3);
        Assert.That(scoring.Score, Is.EqualTo(150)); // 3 * 50
    }

    [Test]
    public void HighScore_UpdatesCorrectly()
    {
        var scoring = new ScoringSystem();
        scoring.AddPoints(100);
        scoring.AddPoints(50);  // Score becomes 150, high score should be 150 (max ever)
        Assert.That(scoring.HighScore, Is.EqualTo(150));
        // Or test a scenario where score does not exceed previous high:
        // scoring.Reset(); scoring.AddPoints(100); scoring.AddPoints(30); then high score = 100.
        // But we'll keep the simple case.
    }

    [Test]
    public void Reset_SetsScoreToZero()
    {
        var scoring = new ScoringSystem();
        scoring.AddPoints(100);
        scoring.Reset();
        Assert.That(scoring.Score, Is.EqualTo(0));
    }

    [Test]
    public void ScoreChanged_Event_FiresOnAddPoints()
    {
        var scoring = new ScoringSystem();
        bool eventFired = false;
        scoring.ScoreChanged += (score) => eventFired = true;
        scoring.AddPoints(10);
        Assert.That(eventFired, Is.True);
    }
}