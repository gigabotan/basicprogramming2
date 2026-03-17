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

    private static Gravity MakeWhiteHole(Vector target) => (size, v) =>
    {
        var targetVector = v - defaultTarget;
        var d = targetVector.Length;
        var magnitude = 140 * d / (d * d + 1);
        return new Vector(magnitude, 0).Rotate(targetVector.Angle);
    };

    private static Gravity MakeBlackHole(Vector anomaly) => (size, v) =>
    {
        var anomalyVector = anomaly - v;
        var d = anomalyVector.Length;
        var magnitude = 300 * d / (d * d + 1);
        return new Vector(magnitude, 0).Rotate(anomalyVector.Angle);
    };

    private static Gravity MakeMixedGravity(Vector target, Vector anomaly) => (size, v) =>
    {
        return (MakeWhiteHole(target)(size, v) + MakeBlackHole(anomaly)(size, v)) / 2;
    };

    private static Level DefaultLevelWithGravity(String name, Gravity gravity) => new Level(name,
                                                                                            defaultRocket,
                                                                                            defaultTarget,
                                                                                            gravity,
                                                                                            standardPhysics);

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
            MakeWhiteHole(defaultTarget));
        yield return DefaultLevelWithGravity("BlackHole",
            MakeBlackHole(defaultAnomaly));
        yield return DefaultLevelWithGravity("BlackAndWhite",
            MakeMixedGravity(defaultTarget, defaultAnomaly));
    }
}
