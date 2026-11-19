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

    private GameManager _gameManager;

    private Vector2? _goal;
    private Vector2 _currentDirection;
    private double _immobileSeconds;

    private double _timeSinceLastUpdate;

    public override void Init()
    {
        _gameManager = Mob.GetParent().GetNode<GameManager>("%GameManager");
        _timeSinceLastUpdate = Utils.Rng.RandfRange(0, TickSeconds);
    }

    public override void Tick(double delta)
    {
        if (Mob.IsProtesting)
            _goal = null;

        if (_goal != null)
        {
            if (Mob.GlobalPosition.DistanceSquaredTo(_goal.Value) < 32 * 32)
                Mob.QueueFree();
            return;
        }

        if (_gameManager.GamePhase == GamePhase.Evening && !Mob.IsProtesting)
        {
            _goal = Mob.MobManager.GetRandomSpawnPoint();
            return;
        }

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
            _currentDirection = new Vector2(Utils.Rng.RandfRange(-1, 1), Utils.Rng.RandfRange(-1, 1)).Normalized();
    }

    private void UpdateDirection(Mob[] friends, Mob[] fascists)
    {
        // Add neighbour's direction to self
        if (Mob.IsProtesting)
        {
            var friendInfluence = new Vector2();
            foreach (var friend in friends)
                friendInfluence += ConversionFactor *
                                   (friend.LinearVelocity.Normalized() + (friend.Data.Strategy as RoamStrategy)?._currentDirection ?? new Vector2()) /
                                   2 *
                                   friend.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) /
                                   (NeighbourRange * NeighbourRange);

            var fascistInfluence = new Vector2();
            foreach (var fascist in fascists)
                fascistInfluence += FascistAvoidanceFactor * (Mob.GlobalPosition - fascist.GlobalPosition).Normalized();

            _currentDirection += friendInfluence;
            _currentDirection += fascistInfluence;
        }

        _currentDirection = _currentDirection.Normalized();

        // Bias towards center
        if (Mob.IsProtesting)
        {
            _currentDirection += -Mob.GlobalPosition.Normalized() / 10;
            foreach (var item in Mob.MobManager.PlacedItems)
            {
                if (!IsInstanceValid(item))
                    continue;

                switch (item.type)
                {
                    case PlayerActionItemType.RallyFlag: _currentDirection += (item.GlobalPosition - Mob.GlobalPosition).Normalized() / 2; break;
                    case PlayerActionItemType.AvoidArea:
                        _currentDirection += Mathf.Clamp(1f - (Mob.GlobalPosition - item.GlobalPosition).LengthSquared() / (500 * 500), 0f, 1f) *
                                             (Mob.GlobalPosition - item.GlobalPosition).Normalized() /
                                             2f;
                        break;
                }
            }
        }

        // Add noise
        _currentDirection += (Mob.IsProtesting ? 0f : 0.5f) * new Vector2(Utils.Rng.RandfRange(-1, 1), Utils.Rng.RandfRange(-1, 1));


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

    public override Vector2 GetDestination() => _goal ?? Mob.GlobalPosition + 150 * _currentDirection;
}