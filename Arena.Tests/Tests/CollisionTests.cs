using NUnit.Framework;
using Arena.Game.Entities;
using Microsoft.Xna.Framework;

namespace Arena.Tests;

[TestFixture]
public class CollisionTests
{
    [Test]
    public void PlayerBounds_Intersects_ZombieBounds()
    {
        var player = new Player(new Vector2(0, 0), null);
        var zombie = new Walker(new Vector2(10, 0), null);
        Assert.That(player.Bounds.Intersects(zombie.Bounds), Is.True);
    }

    [Test]
    public void PlayerBounds_DoesNotIntersect_DistantZombie()
    {
        var player = new Player(new Vector2(0, 0), null);
        var zombie = new Walker(new Vector2(100, 0), null);
        Assert.That(player.Bounds.Intersects(zombie.Bounds), Is.False);
    }

    [Test]
    public void PowerUpBounds_Intersects_PlayerBounds()
    {
        var player = new Player(new Vector2(0, 0), null);
        var powerUp = new PowerUp(new Vector2(10, 0), PowerUpType.Medkit, null);
        Assert.That(player.Bounds.Intersects(powerUp.Bounds), Is.True);
    }
}