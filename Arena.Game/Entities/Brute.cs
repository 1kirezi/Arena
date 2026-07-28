using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;

namespace Arena.Game.Entities;

public class Brute : Zombie
{
    public Brute(Vector2 position, Texture2D texture, float healthMultiplier = 1f, float speedMultiplier = 1f)
        : base(
            position,
            texture,
            (int)(Config.BruteBaseHealth * healthMultiplier),
            Config.BruteBaseSpeed * speedMultiplier,
            Config.BruteDamage,
            Config.BruteScore)
    {
    }
}