using System;
using System.Collections.Generic;

namespace func_rocket;

public class LevelsTask
{
    private static readonly Physics standardPhysics = new();
    private static readonly Vector defaultTarget = new(600, 200);
    private static readonly Vector defaultStart = new(200, 500);
    private static readonly Rocket defaultRocket = new Rocket(defaultStart, Vector.Zero, -0.5 * Math.PI);
    private static readonly Vector defaultAnomaly = defaultStart + (defaultTarget - defaultStart) / 2;

    public static IEnumerable<Level> CreateLevels()
    {
        yield return DefaultLevelWithGravity("Zero", (size, v) => Vector.Zero);
        yield return DefaultLevelWithGravity("Heavy", (size, v) => new Vector(0, 0.9));
        yield return new Level("Up",
            defaultRocket,
            new Vector(700, 500),
            (size, v) => new Vector(0, -300 / (size.Y - v.Y + 300.0)),
            standardPhysics);
        yield return DefaultLevelWithGravity("WhiteHole",
            MakeGravityHole(defaultTarget, 140, -1));
        yield return DefaultLevelWithGravity("BlackHole",
            MakeGravityHole(defaultAnomaly, 300, 1));
        yield return DefaultLevelWithGravity("BlackAndWhite",
            MakeMixedGravity(defaultTarget, defaultAnomaly));
    }

    private static Gravity MakeGravityHole(Vector center, double coefficient, double sign) => (size, v) =>
    {
        var directionVector = sign * (center - v);
        var distance = directionVector.Length;
        var magnitude = coefficient * distance / (distance * distance + 1);
        return new Vector(magnitude, 0).Rotate(directionVector.Angle);
    };

    private static Gravity MakeMixedGravity(Vector target, Vector anomaly) => (size, v) =>
    {
        return (MakeGravityHole(target, 140, -1)(size, v) + MakeGravityHole(anomaly, 300, 1)(size, v)) / 2;
    };

    private static Level DefaultLevelWithGravity(String name, Gravity gravity) => new Level(name,
        defaultRocket,
        defaultTarget,
        gravity,
        standardPhysics);
}
