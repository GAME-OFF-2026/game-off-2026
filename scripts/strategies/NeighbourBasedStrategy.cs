using System.Linq;
using Godot;

public abstract partial class NeighbourBasedStrategy : MobStrategy
{
    private const float TickSeconds = 0.15f;

    [Export] public float ProtesterDetectionRange = 200;
    [Export] public float FascistDetectionRange = 300;

    private double _timeSinceLastUpdate;

    protected GameManager GameManager { get; private set; }

    public override void Init()
    {
        GameManager = Mob.GetNode<GameManager>("/root/Main/GameManager");
        _timeSinceLastUpdate = Utils.Rng.RandfRange(0, TickSeconds);
    }

    public sealed override void Tick(double delta)
    {
        if (!PreTick(delta))
            return;

        _timeSinceLastUpdate += delta;

        while (_timeSinceLastUpdate > TickSeconds)
        {
            _timeSinceLastUpdate -= TickSeconds;

            var protesters = Mob.MobManager
                .CurrentMobs.Where(
                    mob => mob != Mob &&
                           mob.IsProtesting &&
                           mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= ProtesterDetectionRange * ProtesterDetectionRange
                )
                .ToArray();

            var fascists = Mob.MobManager.CurrentMobs.Where(
                    mob => mob != Mob &&
                           mob.Data.Faction == Faction.Fascist &&
                           mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= FascistDetectionRange * FascistDetectionRange
                )
                .ToArray();

            TickWithNeighbours(TickSeconds, protesters, fascists);
        }
    }

    protected virtual bool PreTick(double delta) => true;

    protected abstract void TickWithNeighbours(double delta, Mob[] protesters, Mob[] fascists);
}