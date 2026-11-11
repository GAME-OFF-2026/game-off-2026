using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MobManager : Node
{
    private readonly PackedScene _mobScene = GD.Load<PackedScene>("res://mobs/Mob.tscn");
    [Export] public Dictionary<MobData, int> MobTypes;
    [Export] public Rect2I SpawnRect;

    public Array<Mob> CurrentMobs { get; } = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (var entry in MobTypes)
        {
            for (var i = 0; i < entry.Value; i++)
                SpawnMob(
                    entry.Key,
                    new Vector2(
                        Utils.Rng.RandiRange(SpawnRect.Position.X, SpawnRect.End.X),
                        Utils.Rng.RandiRange(SpawnRect.Position.Y, SpawnRect.End.Y)
                    )
                );

            for (var i = 0; i < 10; i++)
            {
                var mob = SpawnMob(entry.Key, new Vector2());
                mob.ForceProtesting(0.5);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private Mob SpawnMob(MobData mobData, Vector2 spawnPosition)
    {
        var newMob = _mobScene.Instantiate<Mob>();
        newMob.MobManager = this;
        newMob.GlobalPosition = spawnPosition;
        AddChild(newMob);
        newMob.SetData(mobData);
        CurrentMobs.Add(newMob);
        return newMob;
    }
}