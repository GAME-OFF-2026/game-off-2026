using Godot;

[GlobalClass]
public partial class TearGas : PlayerActionItem
{
    private MobManager _mobManager;

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("/root/Main/MobManager");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        foreach (var mob in _mobManager.CurrentMobs)
            if (mob.Data.Faction != Faction.Fascist && mob.GlobalPosition.DistanceSquaredTo(GlobalPosition) < 120 * 120)
                mob.Damage(delta * 20);
    }
}