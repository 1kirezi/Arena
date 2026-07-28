namespace Arena.Game.Interfaces;

public interface IDamageable
{
    int Health { get; }
    int MaxHealth { get; }
    void TakeDamage(int damage);
    bool IsDead { get; }
}