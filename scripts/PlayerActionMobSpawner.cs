using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PlayerActionMobSpawner : PlayerActionItem
{
    [Export] private MobData[] _mobsToSpawn;

    private readonly Array<Mob> _mobs = [];

    public override void _Ready()
    {
        var mobManager = GetNode<MobManager>("/root/Main/MobManager");
        foreach (var mobData in _mobsToSpawn)
        {
            var newMob = mobManager.SpawnMob(mobData, GlobalPosition);
            newMob.Reparent(this);
            _mobs.Add(newMob);
            newMob.ForceProtesting(1);
        }
    }

    public override void _Process(double delta)
    {
        if (IsPlaced)
            return;

        for (var i = 0; i < _mobs.Count; i++)
        {
            var mob = _mobs[i];
            mob.GlobalPosition = GetGlobalMousePosition() + new Vector2(40, 0).Rotated(2 * Mathf.Pi * (i / (float)_mobs.Count));
        }
    }
}