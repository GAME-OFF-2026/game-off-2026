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

    protected override void TickWithNeighbours(double delta, Mob[] protesters, Mob[] fascists)
    {
        if (fascists.Length == 0)
        {
            _goal = Mob.MobManager.CurrentMobs.Where(mob => IsInstanceValid(mob) && mob != Mob && mob.Data.Faction == Faction.Fascist)
                .MinBy(mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition))
                .GlobalPosition;
            return;
        }

        var nearestProtester = Mob.MobManager.CurrentMobs.Where(mob => mob.IsProtesting)
            .MinBy(mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition));

        if (nearestProtester != null)
            _goal = nearestProtester.GlobalPosition;

        foreach (var protester in protesters)
            if (protester.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) < 40 * 40)
                protester.Damage(delta * Mob.Data.Dps);

        foreach (var placedItem in Mob.MobManager.PlacedItems)
            if (placedItem is Barricade barricade &&
                barricade.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition) < barricade.DamageRadius * barricade.DamageRadius)
                barricade.Health -= delta * Mob.Data.Dps;
    }
}