using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arena.Game.Core;
using Arena.Game.Entities;
using Arena.Game.MathUtils;
using System;

namespace Arena.Game.Systems;

public class UIManager
{
    private SpriteFont _font;
    private Texture2D _pixelTexture;
    private Rectangle _playButtonBounds = new Rectangle(350, 300, 100, 50);
    private Rectangle _restartButtonBounds = new Rectangle(350, 350, 100, 50);

    // Health bar animation values (Lerp)
    private float _currentPlayerHealthPercent = 1f;
    private float _currentGeneratorHealthPercent = 1f;

    public UIManager(SpriteFont font, Texture2D pixelTexture)
    {
        _font = font;
        _pixelTexture = pixelTexture;
    }

    public void Update(Player player, Generator generator)
    {
        // Smooth health bar updates using Lerp
        float targetPlayerPercent = player.Health / (float)player.MaxHealth;
        _currentPlayerHealthPercent = MathHelpers.Lerp(_currentPlayerHealthPercent, targetPlayerPercent, 0.1f);

        float targetGeneratorPercent = generator.Health / (float)generator.MaxHealth;
        _currentGeneratorHealthPercent = MathHelpers.Lerp(_currentGeneratorHealthPercent, targetGeneratorPercent, 0.1f);
    }

    public void DrawHUD(SpriteBatch spriteBatch, Player player, Generator generator, 
                        ScoringSystem scoring, WaveManager waveManager, GameState state)
    {
        if (state == GameState.Menu)
        {
            DrawMenu(spriteBatch);
            return;
        }

        if (state == GameState.GameOver)
        {
            DrawGameOver(spriteBatch, scoring, waveManager);
            return;
        }

        // Playing HUD
        // Health bar - Player
        DrawHealthBar(spriteBatch, "Player", _currentPlayerHealthPercent, new Vector2(20, 20), Color.Green);

        // Health bar - Generator
        DrawHealthBar(spriteBatch, "Generator", _currentGeneratorHealthPercent, new Vector2(20, 50), Color.Blue);

        // Ammo
        string ammoText = $"Ammo: {player.Ammo}/{Config.PlayerMaxAmmo}";
        if (player.IsReloading) ammoText += " (Reloading...)";
        spriteBatch.DrawString(_font, ammoText, new Vector2(20, 80), Color.White);

        // Score
        spriteBatch.DrawString(_font, $"Score: {scoring.Score}", new Vector2(20, 110), Color.White);
        spriteBatch.DrawString(_font, $"High Score: {scoring.HighScore}", new Vector2(20, 135), Color.Gold);

        // Wave
        spriteBatch.DrawString(_font, $"Wave: {waveManager.CurrentWave}", new Vector2(20, 160), Color.White);
        spriteBatch.DrawString(_font, $"Zombies remaining: {waveManager.GetRemainingZombiesToSpawn()}", 
            new Vector2(20, 185), Color.White);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, string label, float percent, Vector2 position, Color color)
    {
        spriteBatch.DrawString(_font, label, position, Color.White);

        Rectangle background = new Rectangle((int)position.X + 80, (int)position.Y, 150, 20);
        spriteBatch.Draw(_pixelTexture, background, Color.DarkGray);

        int width = (int)(150 * MathHelpers.Clamp(percent, 0, 1));
        Rectangle fill = new Rectangle((int)position.X + 80, (int)position.Y, width, 20);

        Color fillColor = percent > 0.5f ? color : Color.Red;
        spriteBatch.Draw(_pixelTexture, fill, fillColor);

        spriteBatch.DrawString(_font, $"{Math.Round(percent * 100)}%", 
            new Vector2(position.X + 240, position.Y), Color.White);
    }

    private void DrawMenu(SpriteBatch spriteBatch)
    {
        string title = "ZOMBIE DEFENSE";
        Vector2 titleSize = _font.MeasureString(title);
        spriteBatch.DrawString(_font, title, 
            new Vector2(Config.ScreenWidth / 2 - titleSize.X / 2, 200), Color.White);

        // Play button
        spriteBatch.Draw(_pixelTexture, _playButtonBounds, Color.Green);
        spriteBatch.DrawString(_font, "PLAY", 
            new Vector2(_playButtonBounds.X + 25, _playButtonBounds.Y + 15), Color.White);
    }

    private void DrawGameOver(SpriteBatch spriteBatch, ScoringSystem scoring, WaveManager waveManager)
    {
        string gameOverText = "GAME OVER";
        Vector2 size = _font.MeasureString(gameOverText);
        spriteBatch.DrawString(_font, gameOverText, 
            new Vector2(Config.ScreenWidth / 2 - size.X / 2, 200), Color.Red);

        spriteBatch.DrawString(_font, $"Final Score: {scoring.Score}", 
            new Vector2(Config.ScreenWidth / 2 - 100, 260), Color.White);
        spriteBatch.DrawString(_font, $"Wave Reached: {waveManager.CurrentWave}", 
            new Vector2(Config.ScreenWidth / 2 - 100, 290), Color.White);

        // Restart button
        spriteBatch.Draw(_pixelTexture, _restartButtonBounds, Color.Blue);
        spriteBatch.DrawString(_font, "RESTART", 
            new Vector2(_restartButtonBounds.X + 10, _restartButtonBounds.Y + 15), Color.White);
    }

    public bool IsPlayButtonClicked(MouseState mouse)
    {
        return mouse.LeftButton == ButtonState.Pressed && 
               _playButtonBounds.Contains(mouse.X, mouse.Y);
    }

    public bool IsRestartButtonClicked(MouseState mouse)
    {
        return mouse.LeftButton == ButtonState.Pressed && 
               _restartButtonBounds.Contains(mouse.X, mouse.Y);
    }
}