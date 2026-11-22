using System.Collections.Generic;
using Godot;
using Godot.Collections;

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

            var protesters = new Array<Mob>();
            var fascists = new Array<Mob>();
            var neutrals = new Array<Mob>();

            foreach (var mob in Mob.MobManager.CurrentMobs)
            {
                if (mob == Mob || !IsInstanceValid(mob))
                    continue;

                if (mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= ProtesterDetectionRange * ProtesterDetectionRange)
                    (mob.IsProtesting ? protesters : neutrals).Add(mob);

                if (mob.Data.Faction == Faction.Fascist &&
                    mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) <= FascistDetectionRange * FascistDetectionRange)
                    fascists.Add(mob);
            }

            TickWithNeighbours(TickSeconds, protesters, fascists, neutrals);
        }
    }

    protected virtual bool PreTick(double delta) => true;

    protected abstract void TickWithNeighbours(double delta, ICollection<Mob> protesters, ICollection<Mob> fascists, ICollection<Mob> neutrals);
}