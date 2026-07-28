using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Interfaces;
using Arena.Game.MathUtils;

namespace Arena.Game.Entities;

public abstract class Zombie : IDamageable, IMovable
{
    protected Texture2D _texture;
    protected Vector2 _targetPosition;
    protected float _turnTimer;
    protected const float TurnCooldown = 0.5f;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Forward { get; protected set; } = new Vector2(1, 0);
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public float Speed { get; protected set; }
    public int Damage { get; protected set; }
    public int ScoreValue { get; protected set; }
    public bool IsDead => Health <= 0;
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - 15,
        (int)Position.Y - 15,
        30, 30
    );

    protected Zombie(Vector2 position, Texture2D texture, int health, float speed, int damage, int score)
    {
        Position = position;
        _texture = texture;
        Health = health;
        MaxHealth = health;
        Speed = speed;
        Damage = damage;
        ScoreValue = score;
        _targetPosition = position;
    }

    public virtual void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Move toward target
        Vector2 direction = _targetPosition - Position;
        if (direction.Length() > 5f)
        {
            direction.Normalize();
            Velocity = direction * Speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        // Update forward direction (for FOV calculations)
        if (Velocity.Length() > 0.1f)
        {
            Forward = Vector2.Normalize(Velocity);
        }

        Position += Velocity * delta;
    }

    public virtual void SetTarget(Vector2 target)
    {
        _targetPosition = target;
    }

    public virtual void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        float rotation = MathF.Atan2(Forward.Y, Forward.X);
        spriteBatch.Draw(_texture, Position, null, Color.White, rotation,
            new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }

    public void OnCollision(ICollidable other) { }
}