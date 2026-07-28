using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;
using Arena.Game.Entities;
using System;
using System.Collections.Generic;

namespace Arena.Game.Systems;

public class Spawner
{
    private Random _random = new Random();
    private Texture2D _walkerTexture;
    private Texture2D _runnerTexture;
    private Texture2D _bruteTexture;
    private float _spawnTimer;
    private int _zombiesToSpawn;

    public List<Zombie> Zombies { get; } = new List<Zombie>();
    public int MaxZombies { get; set; } = Config.MaxZombiesOnScreen;

    public Spawner(Texture2D walkerTexture, Texture2D runnerTexture, Texture2D bruteTexture)
    {
        _walkerTexture = walkerTexture;
        _runnerTexture = runnerTexture;
        _bruteTexture = bruteTexture;
    }

    public void SetWaveSpawnCount(int count)
    {
        _zombiesToSpawn = count;
        _spawnTimer = 0;
    }

    public void Update(GameTime gameTime, float healthMultiplier, float speedMultiplier)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Remove dead zombies
        Zombies.RemoveAll(z => z.IsDead);

        if (_zombiesToSpawn <= 0 || Zombies.Count >= MaxZombies)
            return;

        _spawnTimer -= delta;
        if (_spawnTimer <= 0)
        {
            SpawnZombie(healthMultiplier, speedMultiplier);
            _zombiesToSpawn--;
            _spawnTimer = Config.SpawnInterval;
        }
    }

    private void SpawnZombie(float healthMultiplier, float speedMultiplier)
    {
        Vector2 position = GetRandomEdgePosition();
        int type = _random.Next(0, 10); // Weighted random

        Zombie zombie;
        if (type < 5) // 50% Walker
        {
            zombie = new Walker(position, _walkerTexture, healthMultiplier, speedMultiplier);
        }
        else if (type < 8) // 30% Runner
        {
            zombie = new Runner(position, _runnerTexture, healthMultiplier, speedMultiplier);
        }
        else // 20% Brute
        {
            zombie = new Brute(position, _bruteTexture, healthMultiplier, speedMultiplier);
        }

        Zombies.Add(zombie);
    }

    private Vector2 GetRandomEdgePosition()
    {
        int edge = _random.Next(0, 4);
        float x, y;

        switch (edge)
        {
            case 0: // Top
                x = _random.Next(0, Config.ScreenWidth);
                y = -30;
                break;
            case 1: // Bottom
                x = _random.Next(0, Config.ScreenWidth);
                y = Config.ScreenHeight + 30;
                break;
            case 2: // Left
                x = -30;
                y = _random.Next(0, Config.ScreenHeight);
                break;
            default: // Right
                x = Config.ScreenWidth + 30;
                y = _random.Next(0, Config.ScreenHeight);
                break;
        }

        return new Vector2(x, y);
    }
}