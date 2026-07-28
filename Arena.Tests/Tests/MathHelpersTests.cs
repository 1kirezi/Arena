using NUnit.Framework;
using Arena.Game.MathUtils;
using Microsoft.Xna.Framework;

namespace Arena.Tests;

[TestFixture]
public class MathHelpersTests
{
    [Test]
    public void Distance_CalculatesCorrectly()
    {
        var a = new Vector2(0, 0);
        var b = new Vector2(3, 4);
        Assert.That(MathHelpers.Distance(a, b), Is.EqualTo(5f));
    }

    [Test]
    public void DotProduct_OfOrthogonalVectors_IsZero()
    {
        var a = new Vector2(1, 0);
        var b = new Vector2(0, 1);
        Assert.That(MathHelpers.Dot(a, b), Is.EqualTo(0f));
    }

    [Test]
    public void DotProduct_OfParallelVectors_IsOne()
    {
        var a = new Vector2(1, 0);
        var b = new Vector2(1, 0);
        Assert.That(MathHelpers.Dot(a, b), Is.EqualTo(1f));
    }

    [Test]
    public void CrossProduct_ReturnsCorrectScalar()
    {
        var a = new Vector2(1, 0);
        var b = new Vector2(0, 1);
        Assert.That(MathHelpers.Cross(a, b), Is.EqualTo(1f));
    }

    [Test]
    public void CrossProduct_ReverseOrder_ReturnsNegative()
    {
        var a = new Vector2(0, 1);
        var b = new Vector2(1, 0);
        Assert.That(MathHelpers.Cross(a, b), Is.EqualTo(-1f));
    }

    [Test]
    public void Lerp_InterpolatesCorrectly()
    {
        Assert.That(MathHelpers.Lerp(0, 10, 0.5f), Is.EqualTo(5f));
    }

    [Test]
    public void Lerp_WithZeroT_ReturnsStart()
    {
        Assert.That(MathHelpers.Lerp(5, 10, 0f), Is.EqualTo(5f));
    }

    [Test]
    public void Lerp_WithOneT_ReturnsEnd()
    {
        Assert.That(MathHelpers.Lerp(5, 10, 1f), Is.EqualTo(10f));
    }

    [Test]
    public void Clamp_ReturnsMinWhenBelowMin()
    {
        Assert.That(MathHelpers.Clamp(-5, 0, 10), Is.EqualTo(0f));
    }

    [Test]
    public void Clamp_ReturnsMaxWhenAboveMax()
    {
        Assert.That(MathHelpers.Clamp(15, 0, 10), Is.EqualTo(10f));
    }
}