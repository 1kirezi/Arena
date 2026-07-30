using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arena.Game.Core;
using Arena.Game.Entities;
using Arena.Game.Systems;
using Arena.Game.MathUtils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Arena.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixelTexture;
    private Texture2D _playerTexture;
    private Texture2D _generatorTexture;
    private Texture2D _walkerTexture;
    private Texture2D _runnerTexture;
    private Texture2D _bruteTexture;
    private Texture2D _circleTexture;
    private SpriteFont _font;

    private GameStateManager _stateManager;
    private Player _player;
    private Generator _generator;
    private Spawner _spawner;
    private ScoringSystem _scoringSystem;
    private CollisionSystem _collisionSystem;
    private UIManager _uiManager;
    private List<PowerUp> _powerUps;

    private int _currentLevel = 1;
    private const int BaseZombies = 8;
    private int _totalZombiesForLevel;
    private int _zombiesSpawnedSoFar = 0;
    private bool _allZombiesSpawned = false;
    private float _spawnTimer = 0f;
    private const float SpawnInterval = 1.5f;

    private float _attackCooldown = 0f;
    private const float AttackCooldownDuration = 0.5f;
    private float _attackDisplayTimer = 0f;
    private Vector2 _attackPosition;

    private MouseState _previousMouseState;
    private KeyboardState _previousKeyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = Config.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Config.ScreenHeight;
    }

    protected override void Initialize()
    {
        _stateManager = new GameStateManager();
        _scoringSystem = new ScoringSystem();
        _collisionSystem = new CollisionSystem();
        _powerUps = new List<PowerUp>();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixelTexture = CreateTexture(1, 1, Color.White);
        _playerTexture = CreateTexture(32, 32, Color.Green);
        _generatorTexture = CreateTexture(50, 50, Color.Blue);
        _walkerTexture = CreateTexture(30, 30, Color.Brown);
        _runnerTexture = CreateTexture(28, 28, Color.Orange);
        _bruteTexture = CreateTexture(40, 40, Color.DarkRed);
        _circleTexture = CreateCircleTexture(80, Color.Yellow);

        try
        {
            _font = Content.Load<SpriteFont>("Font");
        }
        catch
        {
            _font = null;
            Console.WriteLine("Font not loaded.");
        }

        _player = new Player(new Vector2(400, 300), _playerTexture);
        _generator = new Generator(new Vector2(400, 500), _generatorTexture);
        _spawner = new Spawner(_walkerTexture, _runnerTexture, _bruteTexture);
        _uiManager = new UIManager(_font, _pixelTexture);

        StartLevel(1);
    }

    private Texture2D CreateTexture(int width, int height, Color color)
    {
        var texture = new Texture2D(GraphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++) data[i] = color;
        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateCircleTexture(int diameter, Color color)
    {
        var texture = new Texture2D(GraphicsDevice, diameter, diameter);
        var data = new Color[diameter * diameter];
        float radius = diameter / 2f;
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dx = x - radius;
                float dy = y - radius;
                if (Math.Sqrt(dx * dx + dy * dy) <= radius)
                    data[y * diameter + x] = color;
                else
                    data[y * diameter + x] = Color.Transparent;
            }
        }
        texture.SetData(data);
        return texture;
    }

    private void LogError(string msg) { try { File.AppendAllText("crash.log", $"{DateTime.Now}: {msg}\n"); } catch { } }

    private void StartLevel(int level)
    {
        _currentLevel = level;
        _totalZombiesForLevel = BaseZombies + level * 2;
        _zombiesSpawnedSoFar = 0;
        _allZombiesSpawned = false;
        _spawnTimer = 0f;

        _player = new Player(new Vector2(400, 300), _playerTexture);
        _generator = new Generator(new Vector2(400, 500), _generatorTexture);
        _powerUps.Clear();
        _spawner.Zombies.Clear();
        _attackCooldown = 0f;
        _attackDisplayTimer = 0f;

        _uiManager.ResetLevel(level);
    }

    private void StartNextLevel()
    {
        StartLevel(_currentLevel + 1);
        _stateManager.StartGame();
    }

    private void ResetGame()
    {
        _scoringSystem.Reset();
        StartLevel(1);
        _stateManager.StartGame();
    }

    protected override void Update(GameTime gameTime)
    {
        try
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            switch (_stateManager.CurrentState)
            {
                case GameState.Menu:
                    UpdateMenu(mouse);
                    break;
                case GameState.Playing:
                    UpdatePlaying(gameTime, keyboard, mouse);
                    break;
                case GameState.GameOver:
                case GameState.Winner:
                    UpdateGameOver(mouse);
                    break;
            }

            _previousMouseState = mouse;
            _previousKeyboardState = keyboard;
        }
        catch (Exception ex)
        {
            LogError($"Update crash: {ex}");
            Console.WriteLine("Update exception: " + ex.Message);
        }
        base.Update(gameTime);
    }

    private void UpdateMenu(MouseState mouse)
    {
        if (_uiManager.IsPlayButtonClicked(mouse))
        {
            ResetGame();
        }
    }

    private void UpdatePlaying(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _scoringSystem.Update(gameTime);
        _player.Update(gameTime);
        _generator.Update(gameTime, _player);

        if (_attackCooldown > 0) _attackCooldown -= delta;
        if (_attackDisplayTimer > 0) _attackDisplayTimer -= delta;

        if (keyboard.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space) && _attackCooldown <= 0)
        {
            float attackRadius = 80f;
            _attackPosition = _player.Position;
            _attackDisplayTimer = 0.2f;
            _attackCooldown = AttackCooldownDuration;

            var zombiesCopy = new List<Zombie>(_spawner.Zombies);
            foreach (var zombie in zombiesCopy)
            {
                if (zombie.IsDead) continue;
                if (Vector2.Distance(_player.Position, zombie.Position) <= attackRadius)
                    zombie.TakeDamage(30);
            }
        }

        float healthMult = 1f + (_currentLevel - 1) * 0.2f;
        float speedMult = 1f + (_currentLevel - 1) * 0.1f;

        if (!_allZombiesSpawned && _zombiesSpawnedSoFar < _totalZombiesForLevel)
        {
            _spawnTimer -= delta;
            if (_spawnTimer <= 0)
            {
                _spawner.SpawnSingleZombie(healthMult, speedMult);
                _zombiesSpawnedSoFar++;
                _spawnTimer = SpawnInterval;
                if (_zombiesSpawnedSoFar >= _totalZombiesForLevel)
                    _allZombiesSpawned = true;
            }
        }

        var zombiesCopy2 = new List<Zombie>(_spawner.Zombies);
        foreach (var zombie in zombiesCopy2)
        {
            if (zombie.IsDead) continue;
            float dP = Vector2.Distance(zombie.Position, _player.Position);
            float dG = Vector2.Distance(zombie.Position, _generator.Position);
            zombie.SetTarget(dP < 300f && dP < dG ? _player.Position : _generator.Position);
            zombie.Update(gameTime);
        }

        var powerUpsCopy = new List<PowerUp>(_powerUps);
        foreach (var p in powerUpsCopy) p.Update(gameTime);
        _powerUps.RemoveAll(p => !p.IsActive);

        _collisionSystem.CheckCollisions(_player, _generator, _spawner.Zombies, null, _powerUps);

        // Remove dead zombies and give rewards
        for (int i = _spawner.Zombies.Count - 1; i >= 0; i--)
        {
            var zombie = _spawner.Zombies[i];
            if (zombie.IsDead)
            {
                _scoringSystem.AddPoints(zombie.ScoreValue);
                _player.AddAmmo(2);
                _player.Heal(2);

                if (new Random().NextDouble() < Config.PowerUpDropChance)
                {
                    PowerUpType type = (PowerUpType)new Random().Next(0, 3);
                    var powerUp = new PowerUp(zombie.Position, type, _pixelTexture);
                    _powerUps.Add(powerUp);
                }
                _spawner.Zombies.RemoveAt(i);
            }
        }

        // Win condition
        if (_allZombiesSpawned && _spawner.Zombies.Count == 0)
        {
            Console.WriteLine($"WINNER! All zombies killed. Setting state to Winner.");
            _stateManager.Winner();
        }

        // Lose condition
        if (_player.IsDead || _generator.IsDead)
        {
            Console.WriteLine("GAME OVER! Setting state to GameOver.");
            _stateManager.GameOver();
        }

        _uiManager.Update(_player, _generator);
    }

    private void UpdateGameOver(MouseState mouse)
    {
        if (_stateManager.CurrentState == GameState.Winner)
        {
            if (_uiManager.IsNextLevelClicked(mouse))
            {
                StartNextLevel();
            }
            else if (_uiManager.IsRestartButtonClicked(mouse))
            {
                ResetGame();
            }
        }
        else if (_stateManager.CurrentState == GameState.GameOver)
        {
            if (_uiManager.IsGameOverRestartClicked(mouse))
            {
                ResetGame();
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        try
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            // Draw game objects only if playing
            if (_stateManager.IsPlaying)
            {
                _generator.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);
                foreach (var z in _spawner.Zombies) z.Draw(_spriteBatch);
                foreach (var p in _powerUps) p.Draw(_spriteBatch);

                if (_attackDisplayTimer > 0)
                {
                    _spriteBatch.Draw(_circleTexture, _attackPosition, null, new Color(255, 255, 0, 100), 0f,
                        new Vector2(_circleTexture.Width / 2, _circleTexture.Height / 2), 1f, SpriteEffects.None, 0f);
                }
            }

            // Always draw UI
            _uiManager.DrawHUD(_spriteBatch, _player, _generator, _scoringSystem, _currentLevel, _spawner.Zombies.Count, _stateManager.CurrentState);

            _spriteBatch.End();
        }
        catch (Exception ex)
        {
            LogError($"Draw crash: {ex}");
            Console.WriteLine("Draw exception: " + ex.Message);
        }

        base.Draw(gameTime);
    }
}