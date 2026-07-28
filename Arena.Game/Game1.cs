using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arena.Game.Core;
using Arena.Game.Entities;
using Arena.Game.Systems;
using Arena.Game.MathUtils;
using System.Collections.Generic;

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
    private Texture2D _projectileTexture;
    private Texture2D _powerUpTexture;
    private SpriteFont _font;

    // Game state
    private GameStateManager _stateManager;
    private Player _player;
    private Generator _generator;
    private Spawner _spawner;
    private WaveManager _waveManager;
    private ScoringSystem _scoringSystem;
    private CollisionSystem _collisionSystem;
    private UIManager _uiManager;
    private List<Projectile> _projectiles;
    private List<PowerUp> _powerUps;

    // Input
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
        _waveManager = new WaveManager();
        _scoringSystem = new ScoringSystem();
        _collisionSystem = new CollisionSystem();
        _projectiles = new List<Projectile>();
        _powerUps = new List<PowerUp>();

        // Subscribe to wave events
        _waveManager.WaveStarted += (wave) => { /* Could add wave start effects */ };
        _waveManager.WaveCompleted += (wave) =>
        {
            _scoringSystem.AddWaveBonus(wave);
        };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create placeholder textures (colored rectangles)
        _pixelTexture = CreateTexture(1, 1, Color.White);
        _playerTexture = CreateTexture(32, 32, Color.Green);
        _generatorTexture = CreateTexture(50, 50, Color.Blue);
        _walkerTexture = CreateTexture(30, 30, Color.Brown);
        _runnerTexture = CreateTexture(28, 28, Color.Orange);
        _bruteTexture = CreateTexture(40, 40, Color.DarkRed);
        _projectileTexture = CreateTexture(8, 8, Color.Yellow);
        _powerUpTexture = CreateTexture(24, 24, Color.Cyan);

        // Load font (you'll need to add a sprite font to Content)
        // _font = Content.Load<SpriteFont>("Font");

        // Use default font if not available - you should add a real font
        _font = CreateDefaultFont();

        // Initialize entities
        _player = new Player(new Vector2(400, 300), _playerTexture);
        _generator = new Generator(new Vector2(400, 500), _generatorTexture);
        _spawner = new Spawner(_walkerTexture, _runnerTexture, _bruteTexture);
        _uiManager = new UIManager(_font, _pixelTexture);

        // Start first wave
        _waveManager.StartNextWave();
        _spawner.SetWaveSpawnCount(_waveManager.GetRemainingZombiesToSpawn());
    }

    private Texture2D CreateTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(GraphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
            data[i] = color;
        texture.SetData(data);
        return texture;
    }

    private SpriteFont CreateDefaultFont()
    {
        // This is a placeholder - you should add a proper sprite font
        // For now, we'll use a basic font loading method
        // You need to add a .spritefont file to your Content project
        return null; // Will cause issues - you need to add a real font
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        var mouse = Mouse.GetState();

        // Exit
        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        // Handle game state
        switch (_stateManager.CurrentState)
        {
            case GameState.Menu:
                UpdateMenu(mouse);
                break;
            case GameState.Playing:
                UpdatePlaying(gameTime, keyboard, mouse);
                break;
            case GameState.GameOver:
                UpdateGameOver(mouse);
                break;
        }

        _previousMouseState = mouse;
        _previousKeyboardState = keyboard;

        base.Update(gameTime);
    }

    private void UpdateMenu(MouseState mouse)
    {
        if (_uiManager.IsPlayButtonClicked(mouse))
        {
            _stateManager.StartGame();
            ResetGame();
        }
    }

    private void ResetGame()
    {
        _player = new Player(new Vector2(400, 300), _playerTexture);
        _generator = new Generator(new Vector2(400, 500), _generatorTexture);
        _projectiles.Clear();
        _powerUps.Clear();
        _spawner.Zombies.Clear();
        _scoringSystem.Reset();
        _waveManager.Reset();
        _waveManager.StartNextWave();
        _spawner.SetWaveSpawnCount(_waveManager.GetRemainingZombiesToSpawn());
    }

    private void UpdatePlaying(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update player
        _player.Update(gameTime);

        // Handle shooting
        if (keyboard.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
        {
            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            var projectile = _player.Shoot(mousePosition, _projectileTexture);
            if (projectile != null)
                _projectiles.Add(projectile);
        }

        // Handle reload
        if (keyboard.IsKeyDown(Keys.R) && _previousKeyboardState.IsKeyUp(Keys.R))
        {
            _player.Reload();
        }

        // Update generator (just for health bar animation)
        // Generator doesn't need update logic

        // Update zombies and set targets
        foreach (var zombie in _spawner.Zombies)
        {
            if (zombie.IsDead) continue;

            // Determine target: generator or player
            float distToPlayer = MathHelpers.Distance(zombie.Position, _player.Position);
            float distToGenerator = MathHelpers.Distance(zombie.Position, _generator.Position);

            // Dot product: Check if player is in zombie's field of view
            Vector2 toPlayer = _player.Position - zombie.Position;
            if (toPlayer.Length() > 0)
            {
                toPlayer.Normalize();
                float dot = MathHelpers.Dot(zombie.Forward, toPlayer);
                
                // If player is in FOV and within reasonable range, target player
                if (dot > Config.FieldOfViewThreshold && distToPlayer < 300f)
                {
                    zombie.SetTarget(_player.Position);
                    zombie.Update(gameTime);
                    continue;
                }
            }

            // Otherwise target generator
            zombie.SetTarget(_generator.Position);
            zombie.Update(gameTime);

            // Cross product: turning behaviour (visual effect)
            Vector2 toTarget = _generator.Position - zombie.Position;
            if (toTarget.Length() > 0)
            {
                toTarget.Normalize();
                float cross = MathHelpers.Cross(zombie.Forward, toTarget);
                // cross > 0 means turn right, < 0 means turn left
                // This could be used for rotation animation
            }
        }

        // Update projectiles
        foreach (var projectile in _projectiles.ToArray())
        {
            projectile.Update(gameTime);
            if (!projectile.IsActive)
                _projectiles.Remove(projectile);
        }

        // Update power-ups
        foreach (var powerUp in _powerUps.ToArray())
        {
            powerUp.Update(gameTime);
            if (!powerUp.IsActive)
                _powerUps.Remove(powerUp);
        }

        // Spawn zombies
        if (_waveManager.ShouldSpawnMore())
        {
            _spawner.Update(gameTime, _waveManager.HealthMultiplier, _waveManager.SpeedMultiplier);
        }

        // Check for wave completion
        if (_spawner.Zombies.Count == 0 && !_waveManager.WaveActive)
        {
            _waveManager.StartNextWave();
            _spawner.SetWaveSpawnCount(_waveManager.GetRemainingZombiesToSpawn());
        }

        // Collisions
        _collisionSystem.CheckCollisions(_player, _generator, _spawner.Zombies, _projectiles, _powerUps);

        // Check for zombie deaths and spawn power-ups
        foreach (var zombie in _spawner.Zombies.ToArray())
        {
            if (zombie.IsDead)
            {
                _scoringSystem.AddPoints(zombie.ScoreValue);
                _waveManager.ZombieKilled();

                // Random power-up drop
                if (new System.Random().NextDouble() < Config.PowerUpDropChance)
                {
                    PowerUpType type = (PowerUpType)new System.Random().Next(0, 3);
                    var powerUp = new PowerUp(zombie.Position, type, _powerUpTexture);
                    _powerUps.Add(powerUp);
                }
            }
        }

        // Check game over conditions
        if (_player.IsDead || _generator.IsDead)
        {
            _stateManager.GameOver();
        }

        // Update UI
        _uiManager.Update(_player, _generator);
    }

    private void UpdateGameOver(MouseState mouse)
    {
        if (_uiManager.IsRestartButtonClicked(mouse))
        {
            _stateManager.StartGame();
            ResetGame();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw game objects
        if (_stateManager.IsPlaying)
        {
            // Draw generator
            _generator.Draw(_spriteBatch);

            // Draw player
            _player.Draw(_spriteBatch);

            // Draw zombies
            foreach (var zombie in _spawner.Zombies)
            {
                zombie.Draw(_spriteBatch);
            }

            // Draw projectiles
            foreach (var projectile in _projectiles)
            {
                projectile.Draw(_spriteBatch);
            }

            // Draw power-ups
            foreach (var powerUp in _powerUps)
            {
                powerUp.Draw(_spriteBatch);
            }
        }

        // Draw UI (covers everything)
        _uiManager.DrawHUD(_spriteBatch, _player, _generator, _scoringSystem, _waveManager, _stateManager.CurrentState);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}