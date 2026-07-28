namespace Arena.Game.Core;

public static class Config
{
    // Screen dimensions
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;

    // Player settings
    public const int PlayerMaxHealth = 100;
    public const float PlayerSpeed = 200f;
    public const int PlayerMaxAmmo = 12;
    public const float ReloadTime = 1.5f;
    public const float ShootCooldown = 0.3f;
    public const int ProjectileDamage = 20;
    public const float ProjectileSpeed = 400f;
    public const float ProjectileLifetime = 2.0f;

    // Generator settings
    public const int GeneratorMaxHealth = 100;

    // Zombie base stats (will be modified by wave scaling)
    public const int WalkerBaseHealth = 40;
    public const float WalkerBaseSpeed = 80f;
    public const int WalkerDamage = 5;
    public const int WalkerScore = 10;

    public const int RunnerBaseHealth = 20;
    public const float RunnerBaseSpeed = 180f;
    public const int RunnerDamage = 3;
    public const int RunnerScore = 15;

    public const int BruteBaseHealth = 120;
    public const float BruteBaseSpeed = 40f;
    public const int BruteDamage = 15;
    public const int BruteScore = 25;

    // Spawning
    public const int MaxZombiesOnScreen = 30;
    public const float SpawnInterval = 2.0f;
    public const int BaseSpawnCount = 3;

    // Power-ups
    public const float PowerUpDropChance = 0.2f;
    public const float PowerUpPickupRange = 40f;
    public const int MedkitHealAmount = 25;
    public const int AmmoRefillAmount = 12;
    public const float TurretDuration = 10f;
    public const float TurretFireRate = 0.5f;
    public const int TurretDamage = 10;
    public const float TurretRange = 200f;

    // Zombie FOV (dot product threshold)
    public const float FieldOfViewThreshold = 0.7f; // ~45 degrees
}