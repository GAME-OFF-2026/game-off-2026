using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MobManager : Node2D
{
    private readonly PackedScene _mobScene = GD.Load<PackedScene>("res://mobs/mob.tscn");
    private GameManager _gameManager;

    [Export] private uint _setupPackSize = 3;
    [Export] private uint _setupPackCount = 3;

    [Export] private MobData _protester;
    [Export] private uint _neutralCount = 50;

    [Export] private MobData _fascist;
    [Export] private uint _fascistCount = 3;

    [Export] private uint _neutralSpawnPeriod = 30;
    [Export] private uint _fascistSpawnPeriod = 60;

    [Export] public Rect2I SpawnRect;

    private double _neutralSpawnTimer;
    private double _fascistSpawnTimer;

    private Array<Mob> _currentPack = [];
    private uint _placedPacks;

    private Node2D[] _spawnPoints;
    public readonly Array<PlayerActionItem> PlacedItems = [];

    public Array<Mob> CurrentMobs { get; } = [];

    public float PlacedPackRate
    {
        get
        {
            if (_setupPackCount == 0)
                return 1;
            return _placedPacks / (float)_setupPackCount;
        }
    }

    public override void _Ready()
    {
        _spawnPoints = [.. GetNode("SpawnPoints").GetChildren().Cast<Node2D>()];
        _gameManager = GetNode<GameManager>("%GameManager");
        SpawnNeutralMobs();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_gameManager.GamePhase != GamePhase.Setup)
            return;

        if (@event.IsActionPressed("Click"))
        {
            _currentPack.Clear();
            _placedPacks++;
            if (_placedPacks >= _setupPackCount)
                _gameManager.EndSetupPhase();
        }
    }

    public override void _Process(double delta)
    {
        if (_gameManager.GamePhase == GamePhase.Setup)
        {
            while (_currentPack.Count < _setupPackSize)
            {
                var mob = SpawnMob(_protester, new Vector2());
                mob.ForceProtesting(1);
                _currentPack.Add(mob);
            }

            for (var i = 0; i < _currentPack.Count; i++)
            {
                var mob = _currentPack[i];
                mob.GlobalPosition = GetGlobalMousePosition() + new Vector2(40, 0).Rotated(2 * Mathf.Pi * (i / (float)_currentPack.Count));
            }

            return;
        }

        _neutralSpawnTimer += delta;
        if (_neutralSpawnTimer >= _neutralSpawnPeriod)
        {
            _neutralSpawnTimer = 0;
            SpawnMob(_protester, GetRandomSpawnPoint());
        }

        _fascistSpawnTimer += delta;
        if (_fascistSpawnTimer >= _fascistSpawnPeriod)
        {
            _fascistSpawnTimer = 0;
            SpawnMob(_fascist, GetRandomSpawnPoint());
        }
    }

    private void SpawnNeutralMobs()
    {
        for (var i = 0; i < _neutralCount; i++)
            SpawnMob(
                _protester,
                new Vector2(
                    Utils.Rng.RandiRange(SpawnRect.Position.X, SpawnRect.End.X),
                    Utils.Rng.RandiRange(SpawnRect.Position.Y, SpawnRect.End.Y)
                )
            );

        var fascistsSpawnPoint = GetRandomSpawnPoint();

        for (var i = 0; i < _fascistCount; i++)
            SpawnMob(_fascist, fascistsSpawnPoint);
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

    private Vector2 GetRandomSpawnPoint() => _spawnPoints[Utils.Rng.RandiRange(0, _spawnPoints.Length - 1)].GlobalPosition;
}