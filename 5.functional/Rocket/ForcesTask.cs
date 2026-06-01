using System;
using System.Linq;
using ReactiveUI;

namespace func_rocket;

public class ForcesTask
{
    public static RocketForce GetThrustForce(double thrust) => rocket
        => new Vector(thrust * Math.Cos(rocket.Direction), thrust * Math.Sin(rocket.Direction));

    public static RocketForce ConvertGravityToForce(Gravity gravity, Vector fieldSize) => rocket
        => gravity(fieldSize, rocket.Location);

    public static RocketForce Sum(params RocketForce[] forces) => rocket
        => forces.Length > 0
                ? forces.Select(force => force(rocket)).Aggregate((acc, next) => acc + next)
                : Vector.Zero;
}
