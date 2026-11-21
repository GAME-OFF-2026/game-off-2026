using System.Linq;
using Godot;

[GlobalClass]
public partial class RoamStrategy : NeighbourBasedStrategy
{
    [Export] public float FascistAvoidanceFactor = 1;
    [Export] public float ConversionFactor = 0.05f;
    [Export] public double PatienceSeconds = 3f;

    private Vector2? _goal;
    private Vector2 _currentDirection;
    private double _immobileSeconds;

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

    protected override void TickWithNeighbours(double delta, Mob[] protesters, Mob[] fascists)
    {
        UpdateDirection(protesters, fascists);
        UpdateMotivation(protesters, fascists);

        if (Mob.Data.ProtesterType == ProtesterType.Medic)
            foreach (var protester in protesters)
                protester.Heal(delta * 20);
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
                                   (ProtesterDetectionRange * ProtesterDetectionRange);

            var fascistInfluence = new Vector2();
            if (Mob.Data.ProtesterType != ProtesterType.BlackBlock)
                foreach (var fascist in fascists)
                    fascistInfluence += FascistAvoidanceFactor * (Mob.GlobalPosition - fascist.GlobalPosition).Normalized();

            _currentDirection += friendInfluence;
            _currentDirection += fascistInfluence;
        }

        _currentDirection = _currentDirection.Normalized();

        if (Mob.IsProtesting)
        {
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
        Mob.Motivation += (friends.Length + 10 * friends.Count(mob => mob.Data.ProtesterType == ProtesterType.Speaker)) *
                          Mob.MotivationGainRate *
                          (Mob.IsProtesting ? 0.005 : 0.003);

        Mob.Motivation -= Mob.MotivationLossRate * (Mob.IsProtesting ? 0.005 : 0.01);

        if (Mob.Data.ProtesterType != ProtesterType.BlackBlock)
            foreach (var fascist in fascists)
                Mob.Motivation -= Mob.MotivationLossRate *
                                  0.03 *
                                  (1 -
                                   fascist.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) / (FascistDetectionRange * FascistDetectionRange));
    }

    public override Vector2 GetDestination() => _goal ?? Mob.GlobalPosition + 150 * _currentDirection;
}