using System;
using Microsoft.Xna.Framework;
using Arena.Game.Entities;
using Arena.Game.Interfaces;
using System.Collections.Generic;

namespace Arena.Game.Systems;

public class CollisionSystem
{
    public void CheckCollisions(
        Player player,
        Generator generator,
        List<Zombie> zombies,
        List<Projectile> projectiles,
        List<PowerUp> powerUps)
    {
        // Player vs Zombies
        foreach (var zombie in zombies)
        {
            if (zombie.IsDead) continue;
            if (player.Bounds.Intersects(zombie.Bounds))
            {
                player.TakeDamage(zombie.Damage);
                zombie.TakeDamage(zombie.MaxHealth); // Kill zombie on contact
            }
        }

        // Projectiles vs Zombies
        foreach (var projectile in projectiles.ToArray())
        {
            if (!projectile.IsActive) continue;
            foreach (var zombie in zombies)
            {
                if (zombie.IsDead) continue;
                if (projectile.Bounds.Intersects(zombie.Bounds))
                {
                    zombie.TakeDamage(projectile.Damage);
                    projectile.IsActive = false;
                    break;
                }
            }
        }

        // Zombies vs Generator
        foreach (var zombie in zombies)
        {
            if (zombie.IsDead) continue;
            if (zombie.Bounds.Intersects(generator.Bounds))
            {
                generator.TakeDamage(zombie.Damage);
                zombie.TakeDamage(zombie.MaxHealth); // Zombie dies after attacking generator
            }
        }

        // Player vs PowerUps
        foreach (var powerUp in powerUps.ToArray())
        {
            if (!powerUp.IsActive) continue;
            if (player.Bounds.Intersects(powerUp.Bounds))
            {
                powerUp.Apply(player);
                powerUp.IsActive = false;
            }
        }
    }
}