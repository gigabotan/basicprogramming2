using System;

namespace func_rocket;

public class ControlTask
{
    private static readonly double minDist = 20;
    private static readonly double directionCoef = 3;

    public static Turn ControlRocket(Rocket rocket, Vector target)
    {
        var toTarget = target - rocket.Location;
        if (toTarget.Length < minDist)
            return Turn.None;
        var speed = rocket.Velocity.Length;
        var velAngle = NormalizeAngle(rocket.Velocity.Angle - toTarget.Angle);
        var rocketAngle = NormalizeAngle(rocket.Direction - toTarget.Angle);
        var weightedAngle = (velAngle * speed + rocketAngle * directionCoef) / (speed + directionCoef);
        return Math.Abs(weightedAngle) < 0.01 ? Turn.None : weightedAngle > 0 ? Turn.Left : Turn.Right;
    }

    private static double NormalizeAngle(double angle)
    {
        while (angle <= -Math.PI) angle += 2 * Math.PI;
        while (angle > Math.PI) angle -= 2 * Math.PI;
        return angle;
    }
}
