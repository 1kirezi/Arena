using System;
using Microsoft.Xna.Framework;

namespace Arena.Game.Systems;

public class ScoringSystem
{
    private int _score;
    private int _highScore;
    private float _timeScoreAccumulator = 0f;
    private const float TimeScoreInterval = 1.0f;
    private const int TimeScorePerInterval = 5;

    public int Score => _score;
    public int HighScore => _highScore;

    public event Action<int> ScoreChanged;

    public void Update(GameTime gameTime)
    {
        _timeScoreAccumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeScoreAccumulator >= TimeScoreInterval)
        {
            _timeScoreAccumulator = 0;
            AddPoints(TimeScorePerInterval);
        }
    }

    public void AddPoints(int points)
    {
        _score += points;
        if (_score > _highScore) _highScore = _score;
        ScoreChanged?.Invoke(_score);
    }

    public void AddWaveBonus(int waveNumber)
    {
        AddPoints(waveNumber * 50);
    }

    public void Reset()
    {
        _score = 0;
        _timeScoreAccumulator = 0;
    }

    public void LoadHighScore(int highScore)
    {
        _highScore = highScore;
    }
}