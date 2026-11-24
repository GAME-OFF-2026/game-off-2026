using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class RoamStrategy : NeighbourBasedStrategy
{
    [Export] public float FascistAvoidanceFactor = 10;
    [Export] public float ConversionFactor = 1;
    [Export] public double PatienceSeconds = 1;

    private Polygon2D _boundaryPolygon;

    private Vector2? _goal;
    private Vector2 _currentDirection;
    private double _immobileSeconds;

    public override void Init()
    {
        base.Init();
        _boundaryPolygon = Mob.GetNode<Polygon2D>("/root/Main/StreetsBoundaryPolygon");
    }

    protected override bool PreTick(double delta)
    {
        if (Mob.IsProtesting)
            _goal = null;

        if (_goal != null)
        {
            if (Mob.GlobalPosition.DistanceSquaredTo(_goal.Value) < 32 * 32)
                Mob.QueueFree();
            return false;
        }

        if (GameManager.GamePhase == GamePhase.Evening && !Mob.IsProtesting)
        {
            _goal = Mob.MobManager.GetRandomSpawnPoint();
            return false;
        }

        ImmobilityCheck(delta);

        return true;
    }

    protected override void TickWithNeighbours(double delta, ICollection<Mob> protesters, ICollection<Mob> fascists, ICollection<Mob> neutrals)
    {
        UpdateDirection(protesters, fascists);
        UpdateMotivation(protesters, fascists);

        if (Mob.Data.ProtesterType == ProtesterType.Medic)
        {
            Mob.Heal(delta * 20);
            foreach (var protester in protesters)
                protester.Heal(delta * 20);
            foreach (var neutral in neutrals)
                neutral.Heal(delta * 20);
        }
    }

    private void ImmobilityCheck(double delta)
    {
        if (Mob.LinearVelocity.LengthSquared() < 1)
            _immobileSeconds += delta;
        else
            _immobileSeconds = 0;

        if (_immobileSeconds >= PatienceSeconds)
            _currentDirection = -Mob.GlobalPosition.Normalized();
    }

    private void UpdateDirection(ICollection<Mob> protesters, ICollection<Mob> fascists)
    {
        // Add neighbour's direction to self
        if (Mob.IsProtesting)
        {
            if (!Geometry2D.IsPointInPolygon(Mob.GlobalPosition, _boundaryPolygon.Polygon))
            {
                _currentDirection = -Mob.GlobalPosition.Normalized();
                return;
            }

            if (Mob.Data.ProtesterType == ProtesterType.BlackBlock && protesters.Count > 3 && fascists.Count > 0)
            {
                var averageFriendPosition = protesters.Aggregate(new Vector2(), (sum, mob) => sum + mob.GlobalPosition / protesters.Count);
                var averageFascistPosition = fascists.Aggregate(new Vector2(), (sum, mob) => sum + mob.GlobalPosition / fascists.Count);
                _currentDirection = (averageFascistPosition + averageFriendPosition) / 2 - Mob.GlobalPosition;
            }
            else
            {
                var friendInfluence = new Vector2();
                foreach (var protester in protesters)
                    friendInfluence += ConversionFactor *
                                       (protester.LinearVelocity.Normalized() + (protester.Data.Strategy as RoamStrategy)?._currentDirection ??
                                        new Vector2()) /
                                       2 *
                                       protester.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) /
                                       (ProtesterDetectionRange * ProtesterDetectionRange);

                var fascistInfluence = new Vector2();
                foreach (var fascist in fascists)
                    fascistInfluence += FascistAvoidanceFactor * (Mob.GlobalPosition - fascist.GlobalPosition).Normalized();

                _currentDirection += friendInfluence;
                _currentDirection += fascistInfluence;
            }

            _currentDirection = _currentDirection.Normalized();

            // Bias towards center
            _currentDirection += -Mob.GlobalPosition.Normalized() / 10;
            foreach (var item in Mob.MobManager.PlacedItems)
            {
                if (!IsInstanceValid(item))
                    continue;

                switch (item.Type)
                {
                    case PlayerActionItemType.RallyFlag: _currentDirection += (item.GlobalPosition - Mob.GlobalPosition).Normalized() / 2; break;
                    case PlayerActionItemType.AvoidArea:
                        _currentDirection += Mathf.Clamp(1f - (Mob.GlobalPosition - item.GlobalPosition).LengthSquared() / (500 * 500), 0f, 1f) *
                                             (Mob.GlobalPosition - item.GlobalPosition).Normalized();
                        break;
                }
            }
        }

        // Add noise
        var rand = Utils.Rng.RandfRange(0, 2 * Mathf.Pi);
        _currentDirection += (Mob.IsProtesting ? 0f : 0.5f) * new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));

        _currentDirection = _currentDirection.Normalized();
    }

    private void UpdateMotivation(ICollection<Mob> protesters, ICollection<Mob> fascists)
    {
        Mob.Motivation += (protesters.Count + 5 * protesters.Count(mob => mob.Data.ProtesterType == ProtesterType.Speaker)) *
                          Mob.MotivationGainRate *
                          (Mob.IsProtesting ? 0.005 : 0.003);

        Mob.Motivation -= Mob.MotivationLossRate * (Mob.IsProtesting ? 0.005 : 0.01);

        if (Mob.IsProtesting && Mob.Data.ProtesterType != ProtesterType.BlackBlock)
            foreach (var fascist in fascists)
                Mob.Motivation -= Mob.MotivationLossRate *
                                  0.03 *
                                  (1 -
                                   fascist.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) / (FascistDetectionRange * FascistDetectionRange));
    }

    public override Vector2 GetDestination() => _goal ?? Mob.GlobalPosition + 150 * _currentDirection;
}