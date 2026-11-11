using System.Linq;
using Godot;

[GlobalClass]
public partial class RoamStrategy : MobStrategy
{
    private const float TickSeconds = 0.1f;

    [Export] public float FascistDetectionRange = 300;
    [Export] public float FascistAvoidanceFactor = 1;
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

            var friends = Mob.MobManager
                .CurrentMobs.Where(
                    mob => mob != Mob &&
                           mob.IsProtesting &&
                           mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= NeighbourRange * NeighbourRange
                )
                .ToArray();

            var fascists = Mob.MobManager.CurrentMobs.Where(
                    mob => mob.Data.Faction == Faction.Fascist &&
                           mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= FascistDetectionRange * FascistDetectionRange
                )
                .ToArray();

            UpdateDirection(friends, fascists);
            UpdateMotivation(friends, fascists);
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

    private void UpdateDirection(Mob[] friends, Mob[] fascists)
    {
        // Add neighbour's direction to self
        if (Mob.IsProtesting)
        {
            var friendInfluence = new Vector2();
            foreach (var friend in friends)
                friendInfluence += ConversionFactor *
                                   friend.LinearVelocity.Normalized() *
                                   friend.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) /
                                   (NeighbourRange * NeighbourRange);

            var fascistInfluence = new Vector2();
            foreach (var fascist in fascists)
                fascistInfluence += FascistAvoidanceFactor * (Mob.GlobalPosition - fascist.GlobalPosition).Normalized();

            _currentDirection += friendInfluence;
            _currentDirection += fascistInfluence;
        }

        // Add noise
        _currentDirection += new Vector2(Utils.Rng.RandfRange(-0.5f, 0.5f), Utils.Rng.RandfRange(-0.5f, 0.5f));

        _currentDirection = _currentDirection.Normalized();
    }

    private void UpdateMotivation(Mob[] friends, Mob[] fascists)
    {
        Mob.Motivation += friends.Length * Mob.MotivationGainRate * (Mob.IsProtesting ? 0.005 : 0.003);

        Mob.Motivation -= Mob.MotivationLossRate * (Mob.IsProtesting ? 0.005 : 0.01);

        foreach (var fascist in fascists)
            Mob.Motivation -= Mob.MotivationLossRate *
                              0.03 *
                              (1 - fascist.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) / (FascistDetectionRange * FascistDetectionRange));
    }

    public override Vector2 GetDestination() => Mob.GlobalPosition + 150 * _currentDirection;
}