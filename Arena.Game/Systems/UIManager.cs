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
    private Rectangle _nextLevelButtonBounds = new Rectangle(350, 400, 120, 50);
    private Rectangle _winnerRestartBounds = new Rectangle(350, 460, 100, 50);
    private Rectangle _gameOverRestartBounds = new Rectangle(350, 400, 100, 50);

    private float _currentPlayerHealthPercent = 1f;
    private float _currentGeneratorHealthPercent = 1f;

    public UIManager(SpriteFont font, Texture2D pixelTexture)
    {
        _font = font;
        _pixelTexture = pixelTexture;
    }

    public void ResetLevel(int level) { }

    public void Update(Player player, Generator generator)
    {
        _currentPlayerHealthPercent = MathHelpers.Lerp(_currentPlayerHealthPercent, player.Health / (float)player.MaxHealth, 0.1f);
        _currentGeneratorHealthPercent = MathHelpers.Lerp(_currentGeneratorHealthPercent, generator.Health / (float)generator.MaxHealth, 0.1f);
    }

    public void DrawHUD(SpriteBatch spriteBatch, Player player, Generator generator,
                        ScoringSystem scoring, int currentLevel, int zombieCount, GameState state)
    {
        try
        {
            // Always draw a yellow square at top-right to confirm this method is called
            spriteBatch.Draw(_pixelTexture, new Rectangle(Config.ScreenWidth - 100, 0, 100, 100), Color.Yellow);

            if (_font == null)
            {
                // If no font, draw a red rectangle and return
                spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, 200, 200), Color.Red);
                return;
            }

            if (state == GameState.Menu)
            {
                DrawMenu(spriteBatch);
                return;
            }

            if (state == GameState.GameOver)
            {
                DrawEndScreen(spriteBatch, "GAME OVER", Color.Red, scoring, currentLevel, player, generator, false);
                return;
            }

            if (state == GameState.Winner)
            {
                DrawEndScreen(spriteBatch, "YOU WIN!", Color.Gold, scoring, currentLevel, player, generator, true);
                return;
            }

            // Playing HUD
            DrawHealthBar(spriteBatch, "Player", _currentPlayerHealthPercent, new Vector2(20, 20), Color.Green);
            DrawHealthBar(spriteBatch, "Generator", _currentGeneratorHealthPercent, new Vector2(20, 50), Color.Blue);

            // Removed Ammo display
            // Score only (no High Score)
            spriteBatch.DrawString(_font, $"Score: {scoring.Score}", new Vector2(20, 80), Color.White);
            spriteBatch.DrawString(_font, $"Level: {currentLevel}", new Vector2(20, 110), Color.White);
            spriteBatch.DrawString(_font, $"Zombies alive: {zombieCount}", new Vector2(20, 135), Color.White);
        }
        catch (Exception ex)
        {
            Console.WriteLine("DrawHUD exception: " + ex.Message);
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, 400, 400), Color.Red);
        }
    }

    private void DrawEndScreen(SpriteBatch spriteBatch, string title, Color titleColor,
                               ScoringSystem scoring, int level, Player player, Generator generator, bool isWinner)
    {
        // Draw a large cyan rectangle to confirm this method is called
        spriteBatch.Draw(_pixelTexture, new Rectangle(0, 100, 800, 400), new Color(0, 255, 255, 100));

        Vector2 titleSize = _font.MeasureString(title);
        spriteBatch.DrawString(_font, title, new Vector2(Config.ScreenWidth / 2 - titleSize.X / 2, 160), titleColor);

        // Display Score, Level, Player Health, Generator Health
        spriteBatch.DrawString(_font, $"Score: {scoring.Score}", new Vector2(Config.ScreenWidth / 2 - 100, 220), Color.White);
        spriteBatch.DrawString(_font, $"Level completed: {level}", new Vector2(Config.ScreenWidth / 2 - 100, 250), Color.White);
        spriteBatch.DrawString(_font, $"Player Health: {player.Health}/{player.MaxHealth}", new Vector2(Config.ScreenWidth / 2 - 100, 280), Color.Green);
        spriteBatch.DrawString(_font, $"Generator Health: {generator.Health}/{generator.MaxHealth}", new Vector2(Config.ScreenWidth / 2 - 100, 310), Color.Blue);

        if (isWinner)
        {
            // Next Level button
            spriteBatch.Draw(_pixelTexture, _nextLevelButtonBounds, Color.Orange);
            spriteBatch.DrawString(_font, "NEXT LEVEL", new Vector2(_nextLevelButtonBounds.X + 10, _nextLevelButtonBounds.Y + 15), Color.Black);

            // Restart button
            spriteBatch.Draw(_pixelTexture, _winnerRestartBounds, Color.Gray);
            spriteBatch.DrawString(_font, "RESTART", new Vector2(_winnerRestartBounds.X + 10, _winnerRestartBounds.Y + 15), Color.White);
        }
        else
        {
            // Restart button for Game Over
            spriteBatch.Draw(_pixelTexture, _gameOverRestartBounds, Color.Blue);
            spriteBatch.DrawString(_font, "RESTART", new Vector2(_gameOverRestartBounds.X + 10, _gameOverRestartBounds.Y + 15), Color.White);
        }
    }

    private void DrawMenu(SpriteBatch spriteBatch)
    {
        string title = "ZOMBIE DEFENSE";
        Vector2 size = _font.MeasureString(title);
        spriteBatch.DrawString(_font, title, new Vector2(Config.ScreenWidth / 2 - size.X / 2, 200), Color.White);

        spriteBatch.Draw(_pixelTexture, _playButtonBounds, Color.Green);
        spriteBatch.DrawString(_font, "PLAY", new Vector2(_playButtonBounds.X + 25, _playButtonBounds.Y + 15), Color.White);
    }

    private void DrawHealthBar(SpriteBatch spriteBatch, string label, float percent, Vector2 position, Color color)
    {
        spriteBatch.DrawString(_font, label, position, Color.White);
        Rectangle bg = new Rectangle((int)position.X + 80, (int)position.Y, 150, 20);
        spriteBatch.Draw(_pixelTexture, bg, Color.DarkGray);
        int width = (int)(150 * MathHelpers.Clamp(percent, 0, 1));
        Rectangle fill = new Rectangle((int)position.X + 80, (int)position.Y, width, 20);
        Color fillColor = percent > 0.5f ? color : Color.Red;
        spriteBatch.Draw(_pixelTexture, fill, fillColor);
        spriteBatch.DrawString(_font, $"{Math.Round(percent * 100)}%", new Vector2(position.X + 240, position.Y), Color.White);
    }

    public bool IsPlayButtonClicked(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _playButtonBounds.Contains(mouse.X, mouse.Y);
    public bool IsNextLevelClicked(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _nextLevelButtonBounds.Contains(mouse.X, mouse.Y);
    public bool IsRestartButtonClicked(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _winnerRestartBounds.Contains(mouse.X, mouse.Y);
    public bool IsGameOverRestartClicked(MouseState mouse) => mouse.LeftButton == ButtonState.Pressed && _gameOverRestartBounds.Contains(mouse.X, mouse.Y);
}