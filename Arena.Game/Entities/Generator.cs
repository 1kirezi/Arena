using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;
using Arena.Game.Interfaces;
using System;

namespace Arena.Game.Entities;

public class Generator : IDamageable
{
    private Texture2D _texture;
    private float _ammoProductionTimer = 0f;
    private const float AmmoProductionInterval = 5.0f;
    private const int AmmoPerProduction = 3;

    public Vector2 Position { get; set; }
    public int Health { get; private set; }
    public int MaxHealth { get; } = Config.GeneratorMaxHealth;
    public bool IsDead => Health <= 0;
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - 25,
        (int)Position.Y - 25,
        50, 50
    );

    public Generator(Vector2 position, Texture2D texture)
    {
        Position = position;
        _texture = texture;
        Health = MaxHealth;
    }

    public void Update(GameTime gameTime, Player player)
    {
        _ammoProductionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_ammoProductionTimer >= AmmoProductionInterval)
        {
            _ammoProductionTimer = 0;
            player.AddAmmo(AmmoPerProduction);
        }
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, null, Color.White, 0f,
            new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }

    public void OnCollision(ICollidable other) { }
}