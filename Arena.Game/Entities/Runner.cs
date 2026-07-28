using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arena.Game.Core;

namespace Arena.Game.Entities;

public class Runner : Zombie
{
    public Runner(Vector2 position, Texture2D texture, float healthMultiplier = 1f, float speedMultiplier = 1f)
        : base(
            position,
            texture,
            (int)(Config.RunnerBaseHealth * healthMultiplier),
            Config.RunnerBaseSpeed * speedMultiplier,
            Config.RunnerDamage,
            Config.RunnerScore)
    {
    }
}