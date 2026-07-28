using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;

namespace Arena.Game.Entities;

public class Walker : Zombie
{
    public Walker(Vector2 position, Texture2D texture, float healthMultiplier = 1f, float speedMultiplier = 1f)
        : base(
            position,
            texture,
            (int)(Config.WalkerBaseHealth * healthMultiplier),
            Config.WalkerBaseSpeed * speedMultiplier,
            Config.WalkerDamage,
            Config.WalkerScore)
    {
    }
}