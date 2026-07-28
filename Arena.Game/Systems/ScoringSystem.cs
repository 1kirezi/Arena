using System;
namespace Arena.Game.Systems;

public class ScoringSystem
{
    private int _score;
    private int _highScore;

    public int Score => _score;
    public int HighScore => _highScore;

    public event Action<int> ScoreChanged;

    public void AddPoints(int points)
    {
        _score += points;
        if (_score > _highScore)
            _highScore = _score;
        ScoreChanged?.Invoke(_score);
    }

    public void AddWaveBonus(int waveNumber)
    {
        AddPoints(waveNumber * 50);
    }

    public void Reset()
    {
        _score = 0;
    }

    public void LoadHighScore(int highScore)
    {
        _highScore = highScore;
    }
}