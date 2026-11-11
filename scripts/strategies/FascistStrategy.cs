using System.Linq;
using Godot;

[GlobalClass]
public partial class FascistStrategy : MobStrategy
{
    public override Vector2 GetDestination() =>
        Mob.MobManager.CurrentMobs.Where(mob => mob.IsProtesting)
            .MinBy(mob => mob.GlobalPosition.DistanceSquaredTo(Mob.GlobalPosition))
            ?.GlobalPosition ??
        Mob.GlobalPosition;
}