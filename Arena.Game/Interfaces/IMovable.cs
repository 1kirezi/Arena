using Microsoft.Xna.Framework;

namespace Arena.Game.Interfaces;

public interface IMovable
{
    Vector2 Position { get; set; }
    Vector2 Velocity { get; set; }
    void Update(GameTime gameTime);
}