using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MobManager : Node
{
	private readonly PackedScene _mobScene = GD.Load<PackedScene>("res://mobs/Mob.tscn");
	[Export] public Dictionary<MobData, int> MobTypes;

	public Array<Mob> CurrentMobs { get; } = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var entry in MobTypes)
			for (var i = 0; i < entry.Value; i++)
				SpawnMob(entry.Key);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SpawnMob(MobData mobData)
	{
		var newMob = _mobScene.Instantiate<Mob>();
		newMob.MobManager = this;
		AddChild(newMob);
		newMob.SetData(mobData);
		CurrentMobs.Add(newMob);
	}
}
