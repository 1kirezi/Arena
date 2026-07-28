using NUnit.Framework;
using Arena.Game.Entities;
using Arena.Game.Core;
using Microsoft.Xna.Framework;

namespace Arena.Tests;

[TestFixture]
public class PlayerTests
{
    [Test]
    public void Player_TakeDamage_ReducesHealth()
    {
        var player = new Player(new Vector2(0, 0), null);
        int initialHealth = player.Health;
        player.TakeDamage(30);
        Assert.That(player.Health, Is.EqualTo(initialHealth - 30));
    }

    [Test]
    public void Player_Health_DoesNotGoBelowZero()
    {
        var player = new Player(new Vector2(0, 0), null);
        player.TakeDamage(200);
        Assert.That(player.Health, Is.EqualTo(0));
    }

    [Test]
    public void Player_Heal_RestoresHealth()
    {
        var player = new Player(new Vector2(0, 0), null);
        player.TakeDamage(50);
        player.Heal(25);
        Assert.That(player.Health, Is.EqualTo(75));
    }

    [Test]
    public void Player_Heal_DoesNotExceedMaxHealth()
    {
        var player = new Player(new Vector2(0, 0), null);
        player.TakeDamage(10);
        player.Heal(50);
        Assert.That(player.Health, Is.EqualTo(Config.PlayerMaxHealth));
    }

    [Test]
    public void Player_CanShoot_ReturnsTrueWhenAmmoAvailable()
    {
        var player = new Player(new Vector2(0, 0), null);
        Assert.That(player.CanShoot(), Is.True);
    }

    [Test]
    public void Player_Shoot_DecreasesAmmo()
    {
        var player = new Player(new Vector2(0, 0), null);
        int initialAmmo = player.Ammo;
        player.Shoot(new Vector2(10, 0), null);
        Assert.That(player.Ammo, Is.EqualTo(initialAmmo - 1));
    }

    [Test]
    public void Player_AddAmmo_RefillsCorrectly()
    {
        var player = new Player(new Vector2(0, 0), null);
        // Shoot once to reduce ammo
        player.Shoot(new Vector2(10, 0), null);
        int ammoAfterShoot = player.Ammo;
        player.AddAmmo(Config.AmmoRefillAmount);
        // Should be full (or max)
        Assert.That(player.Ammo, Is.EqualTo(Config.PlayerMaxAmmo));
    }

    [Test]
    public void Player_Reload_StartsReloading()
    {
        var player = new Player(new Vector2(0, 0), null);
        // Shoot all ammo
        while (player.Ammo > 0)
            player.Shoot(new Vector2(10, 0), null);
        player.Reload();
        Assert.That(player.IsReloading, Is.True);
    }

    [Test]
    public void Player_IsDead_ReturnsTrueWhenHealthZero()
    {
        var player = new Player(new Vector2(0, 0), null);
        player.TakeDamage(100);
        Assert.That(player.IsDead, Is.True);
    }

    [Test]
    public void Player_Bounds_ReturnsCorrectRectangle()
    {
        var player = new Player(new Vector2(100, 200), null);
        var bounds = player.Bounds;
        Assert.That(bounds.X, Is.EqualTo(84));
        Assert.That(bounds.Y, Is.EqualTo(184));
        Assert.That(bounds.Width, Is.EqualTo(32));
        Assert.That(bounds.Height, Is.EqualTo(32));
    }
}