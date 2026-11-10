using System.Linq;
using Godot;

[GlobalClass]
public partial class RoamStrategy : MobStrategy
{
    private static readonly RandomNumberGenerator Rng = new();

    [Export] public float NeighbourRange = 150;
    [Export] public float ConversionFactor = 0.05f;
    [Export] public double PatienceSeconds = 3f;

    private Vector2 _currentDirection;
    private double _immobileSeconds;

    public override void Tick(double delta)
    {
        if (_currentDirection.LengthSquared() < 1)
            _immobileSeconds += delta;
        else
            _immobileSeconds = 0;

        if (_immobileSeconds >= PatienceSeconds)
            _currentDirection = new Vector2(Rng.RandfRange(-1, 1), Rng.RandfRange(-1, 1));

        if (Rng.Randi() % 10 == 0)
        {
            // Add neighour's direction to self
            if (NeighbourRange > 0 && ConversionFactor > 0)
            {
                var neighbours = Mob.MobManager
                    .CurrentMobs.Where(
                        mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= NeighbourRange * NeighbourRange &&
                               mob.Data.Faction == Mob.Data.Faction
                    )
                    .ToArray();

                foreach (var mob in neighbours)
                    _currentDirection += ConversionFactor *
                                         mob.LinearVelocity *
                                         (mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) / NeighbourRange * NeighbourRange);
            }

            // Add noise
            _currentDirection += new Vector2(Rng.RandfRange(-0.5f, 0.5f), Rng.RandfRange(-0.5f, 0.5f));
        }


        _currentDirection = _currentDirection.Normalized();
    }

    public override Vector2 GetDestination() => Mob.GlobalPosition + 150 * _currentDirection;
}