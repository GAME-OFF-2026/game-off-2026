using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MobManager : Node
{
    private readonly PackedScene _mobScene = GD.Load<PackedScene>("res://mobs/Mob.tscn");

    [Export] private MobData _protester;
    [Export] private uint _neutralCount = 50;
    [Export] private uint _protestingCount = 10;

    [Export] private MobData _fascist;
    [Export] private uint _fascistCount = 3;

    [Export] private uint _neutralSpawnPeriod = 30;
    [Export] private uint _fascistSpawnPeriod = 60;

    [Export] public Rect2I SpawnRect;

    private double _neutralSpawnTimer;
    private double _fascistSpawnTimer;

    public Array<Mob> CurrentMobs { get; } = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (var i = 0; i < _neutralCount; i++)
            SpawnMob(
                _protester,
                new Vector2(
                    Utils.Rng.RandiRange(SpawnRect.Position.X, SpawnRect.End.X),
                    Utils.Rng.RandiRange(SpawnRect.Position.Y, SpawnRect.End.Y)
                )
            );

        for (var i = 0; i < _protestingCount; i++)
            SpawnMob(_protester, new Vector2(600, 200)).ForceProtesting(1);

        for (var i = 0; i < _fascistCount; i++)
            SpawnMob(_fascist, new Vector2(-500, 0));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _neutralSpawnTimer += delta;
        if (_neutralSpawnTimer >= _neutralSpawnPeriod)
        {
            _neutralSpawnTimer = 0;
            SpawnMob(
                _protester,
                new Vector2(
                    Utils.Rng.RandiRange(SpawnRect.Position.X, SpawnRect.End.X),
                    Utils.Rng.RandiRange(SpawnRect.Position.Y, SpawnRect.End.Y)
                )
            );
        }

        _fascistSpawnTimer += delta;
        if (_fascistSpawnTimer >= _fascistSpawnPeriod)
        {
            _fascistSpawnTimer = 0;
            SpawnMob(
                _fascist,
                new Vector2(-500, 0)
            );
        }
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