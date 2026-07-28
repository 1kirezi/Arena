using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Arena.Game.Core;
using Arena.Game.Interfaces;
using Arena.Game.MathUtils;

namespace Arena.Game.Entities;

public class Player : IDamageable, IMovable
{
    private Texture2D _texture;
    private float _reloadTimer;
    private float _shootCooldownTimer;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Health { get; private set; }
    public int MaxHealth { get; } = Config.PlayerMaxHealth;
    public int Ammo { get; private set; } = Config.PlayerMaxAmmo;
    public float Speed { get; set; } = Config.PlayerSpeed;
    public bool IsReloading { get; private set; }
    public bool IsDead => Health <= 0;
    public Rectangle Bounds => new Rectangle(
        (int)Position.X - 16,
        (int)Position.Y - 16,
        32, 32
    );

    public Player(Vector2 startPosition, Texture2D texture)
    {
        Position = startPosition;
        _texture = texture;
        Health = MaxHealth;
        Ammo = Config.PlayerMaxAmmo;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Handle reload timer
        if (IsReloading)
        {
            _reloadTimer -= delta;
            if (_reloadTimer <= 0)
            {
                IsReloading = false;
                Ammo = Config.PlayerMaxAmmo;
            }
        }

        // Handle shoot cooldown
        if (_shootCooldownTimer > 0)
            _shootCooldownTimer -= delta;

        // Movement
        var keyboard = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            direction.Y = -1;
        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            direction.Y = 1;
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            direction.X = -1;
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            direction.X = 1;

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            Velocity = direction * Speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        Position += Velocity * delta;

        // Clamp player within screen bounds
        Position = new Vector2(
            MathHelper.Clamp(Position.X, 20, Config.ScreenWidth - 20),
            MathHelper.Clamp(Position.Y, 20, Config.ScreenHeight - 20)
        );
    }

    public bool CanShoot()
    {
        return !IsReloading && Ammo > 0 && _shootCooldownTimer <= 0;
    }

    public Projectile Shoot(Vector2 targetPosition, Texture2D projectileTexture)
    {
        if (!CanShoot()) return null;

        Ammo--;
        _shootCooldownTimer = Config.ShootCooldown;

        Vector2 direction = targetPosition - Position;
        direction.Normalize();

        return new Projectile(
            Position + direction * 20,
            direction * Config.ProjectileSpeed,
            Config.ProjectileDamage,
            projectileTexture
        );
    }

    public void Reload()
    {
        if (!IsReloading && Ammo < Config.PlayerMaxAmmo)
        {
            IsReloading = true;
            _reloadTimer = Config.ReloadTime;
        }
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    public void AddAmmo(int amount)
    {
        Ammo = Math.Min(Config.PlayerMaxAmmo, Ammo + amount);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, null, Color.White, 0f,
            new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }

    public void OnCollision(ICollidable other) { }
}