using System.Linq;
using Godot;

[GlobalClass]
public partial class FascistStrategy : NeighbourBasedStrategy
{
    private Vector2 _goal;

    public override Vector2 GetDestination() => _goal;

    public override void Init()
    {
        _goal = Mob.GlobalPosition;
    }

    protected override void TickWithNeighbours(Mob[] protesters, Mob[] fascists)
    {
        if (fascists.Length == 0)
        {
            _goal = Mob.MobManager.CurrentMobs.Where(mob => mob != Mob && mob.Data.Faction == Faction.Fascist)
                .MinBy(mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition))
                .GlobalPosition;
            return;
        }

        _goal = Mob.MobManager.CurrentMobs.Where(mob => mob.IsProtesting)
            .MinBy(mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition))
            .GlobalPosition;
    }
}