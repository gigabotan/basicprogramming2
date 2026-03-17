using System;
using System.Linq;
using ReactiveUI;

namespace func_rocket;

public class ForcesTask
{
    public static RocketForce GetThrustForce(double forceValue)
        => r => new Vector(forceValue * Math.Cos(r.Direction), forceValue * Math.Sin(r.Direction));

    public static RocketForce ConvertGravityToForce(Gravity gravity, Vector spaceSize)
        => r => gravity(spaceSize, r.Location);

    public static RocketForce Sum(params RocketForce[] forces)
        => r => forces.Length > 0
                ? forces.Select(z => z(r)).Aggregate((a, b) => a + b)
                : Vector.Zero;
}
