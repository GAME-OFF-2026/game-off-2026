using System.Linq;
using Godot;

[GlobalClass]
public partial class RoamStrategy : MobStrategy
{
    private const float TickSeconds = 0.1f;

    [Export] public float NeighbourRange = 150;
    [Export] public float ConversionFactor = 0.05f;
    [Export] public double PatienceSeconds = 3f;

    private Vector2 _currentDirection;
    private double _immobileSeconds;

    private double _timeSinceLastUpdate;

    public override void Init()
    {
        _timeSinceLastUpdate = Utils.Rng.RandfRange(0, TickSeconds);
    }

    public override void Tick(double delta)
    {
        ImmobilityCheck(delta);

        _timeSinceLastUpdate += delta;
        if (_timeSinceLastUpdate > TickSeconds)
        {
            _timeSinceLastUpdate -= TickSeconds;

            var protestingNeighbours = Mob.MobManager
                .CurrentMobs.Where(
                    mob => mob != Mob &&
                           mob.IsProtesting &&
                           mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= NeighbourRange * NeighbourRange
                )
                .ToArray();

            UpdateDirection(protestingNeighbours);
            UpdateMotivation(protestingNeighbours);
        }
    }

    private void ImmobilityCheck(double delta)
    {
        if (Mob.LinearVelocity.LengthSquared() < 1)
            _immobileSeconds += delta;
        else
            _immobileSeconds = 0;

        if (_immobileSeconds >= PatienceSeconds)
            _currentDirection = new Vector2(Utils.Rng.RandfRange(-1, 1), Utils.Rng.RandfRange(-1, 1));
    }

    private void UpdateDirection(Mob[] neighbours)
    {
        // Add neighbour's direction to self
        if (Mob.IsProtesting)
            foreach (var mob in neighbours)
                _currentDirection += ConversionFactor *
                                     mob.LinearVelocity.Normalized() *
                                     (mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) / NeighbourRange * NeighbourRange);

        // Add noise
        _currentDirection += new Vector2(Utils.Rng.RandfRange(-0.5f, 0.5f), Utils.Rng.RandfRange(-0.5f, 0.5f));

        _currentDirection = _currentDirection.Normalized();
    }

    private void UpdateMotivation(Mob[] neighbours)
    {
        foreach (var mob in neighbours)
            if (mob.IsProtesting)
                Mob.Motivation += Mob.MotivationGainRate * (Mob.IsProtesting ? 0.005 : 0.003);

        Mob.Motivation -= Mob.MotivationLossRate * (Mob.IsProtesting ? 0.005 : 0.01);
    }

    public override Vector2 GetDestination() => Mob.GlobalPosition + 150 * _currentDirection;
}