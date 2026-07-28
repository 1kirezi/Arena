using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;
using Arena.Game.Interfaces;

namespace Arena.Game.Entities;

public class Projectile : IMovable
{
    private Texture2D _texture;
    private float _lifetime;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Damage { get; }
    public bool IsActive { get; set; } = true;
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - 4,
        (int)Position.Y - 4,
        8, 8
    );

    public Projectile(Vector2 position, Vector2 velocity, int damage, Texture2D texture)
    {
        Position = position;
        Velocity = velocity;
        Damage = damage;
        _texture = texture;
        _lifetime = Config.ProjectileLifetime;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * delta;
        _lifetime -= delta;

        if (_lifetime <= 0 || Position.X < 0 || Position.X > Config.ScreenWidth ||
            Position.Y < 0 || Position.Y > Config.ScreenHeight)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, null, Color.White, 0f,
            new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }
}