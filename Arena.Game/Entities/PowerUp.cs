using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;
using Arena.Game.Interfaces;

namespace Arena.Game.Entities;

public enum PowerUpType
{
    Medkit,
    AmmoBox,
    Turret
}

public class PowerUp : IMovable
{
    private Texture2D _texture;
    private float _lifeTimer = 10f; // Auto-despawn after 10 seconds

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public PowerUpType Type { get; }
    public bool IsActive { get; set; } = true;
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - 12,
        (int)Position.Y - 12,
        24, 24
    );

    public PowerUp(Vector2 position, PowerUpType type, Texture2D texture)
    {
        Position = position;
        Type = type;
        _texture = texture;
        Velocity = new Vector2(0, -30f); // Float upward slightly
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * delta;
        _lifeTimer -= delta;

        // Bobbing effect
        Velocity = new Vector2(Velocity.X, -30f + MathF.Sin(_lifeTimer * 3) * 20f);
        if (_lifeTimer <= 0)
            IsActive = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = Type switch
        {
            PowerUpType.Medkit => Color.Green,
            PowerUpType.AmmoBox => Color.Yellow,
            PowerUpType.Turret => Color.Orange,
            _ => Color.White
        };
        spriteBatch.Draw(_texture, Position, null, color, 0f,
            new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }

    public void Apply(Player player)
    {
        switch (Type)
        {
            case PowerUpType.Medkit:
                player.Heal(Config.MedkitHealAmount);
                break;
            case PowerUpType.AmmoBox:
                player.AddAmmo(Config.AmmoRefillAmount);
                break;
            case PowerUpType.Turret:
                // Turret will be created by the game world
                break;
        }
        IsActive = false;
    }
}