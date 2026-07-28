using Microsoft.Xna.Framework;

namespace Arena.Game.Interfaces;

public interface ICollidable
{
    Rectangle Bounds { get; }
    void OnCollision(ICollidable other);
}