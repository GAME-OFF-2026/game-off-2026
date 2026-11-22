using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MobManager : Node2D
{
    private readonly PackedScene _mobScene = GD.Load<PackedScene>("res://mobs/mob.tscn");
    private GameManager _gameManager;

    [Export] private MobData _protester;
    [Export] private uint _neutralCount = 50;

    [Export] private MobData _fascist;
    [Export] private uint _fascistCount = 3;

    [Export] private uint _neutralSpawnPeriod = 30;
    [Export] private uint _fascistSpawnPeriod = 60;
    [Export] private uint _fascistPackSize = 3;

    [Export] public Rect2I SpawnRect;

    private double _neutralSpawnTimer;
    private double _fascistSpawnTimer;

    private Node2D[] _spawnPoints;
    public readonly Array<PlayerActionItem> PlacedItems = [];

    private Array<Mob> _currentMobs = [];
    private Array<Array<Mob>> _protesterClusters = [];

    private double _timeSinceLastClustering;
    private double _clusteringDistance = 100;

    public IEnumerable<Mob> CurrentMobs => _currentMobs;
    public IEnumerable<Array<Mob>> ProtesterClusters => _protesterClusters;

    public Vector2 GetRandomSpawnPoint() => _spawnPoints[Utils.Rng.RandiRange(0, _spawnPoints.Length - 1)].GlobalPosition;

    public override void _Ready()
    {
        _spawnPoints = [.. GetNode("SpawnPoints").GetChildren().Cast<Node2D>()];
        _gameManager = GetNode<GameManager>("%GameManager");
        SpawnNeutralMobs();
    }

    public override void _Process(double delta)
    {
        _currentMobs = [.. _currentMobs.Where(IsInstanceValid)];

        if (_gameManager.GamePhase == GamePhase.Setup)
            return;

        if (_gameManager.GamePhase == GamePhase.Protest)
        {
            _neutralSpawnTimer += delta;
            if (_neutralSpawnTimer >= _neutralSpawnPeriod)
            {
                _neutralSpawnTimer = 0;
                SpawnMob(_protester, GetRandomSpawnPoint());
            }
        }

        _fascistSpawnTimer += delta;
        if (_fascistSpawnTimer >= _fascistSpawnPeriod)
        {
            _fascistSpawnTimer = 0;
            var spawnPoint = GetRandomSpawnPoint();
            for (var i = 0; i < _fascistPackSize; i++)
                SpawnMob(_fascist, spawnPoint);
        }

        // Apply scaling motivation penalty in phase 3
        if (_gameManager.GamePhase == GamePhase.Evening)
        {
            var penalty = delta * _gameManager.EveningPhaseCompletion();
            foreach (var mob in _currentMobs)
                mob.Motivation -= mob.MotivationLossRate * penalty;
        }

        _timeSinceLastClustering += delta;
        if (_timeSinceLastClustering >= 1)
        {
            _timeSinceLastClustering = 0;
            UpdateClusters();
        }
    }

    private void UpdateClusters()
    {
        var protesters = _currentMobs.Where(m => m.IsProtesting).ToArray();
        var assignedProtesters = new HashSet<Mob>();

        _protesterClusters.Clear();

        foreach (var iterMob in protesters)
        {
            if (assignedProtesters.Contains(iterMob))
                continue;

            _protesterClusters.Add([iterMob]);
            var clusterIndex = _protesterClusters.Count - 1;
            assignedProtesters.Add(iterMob);

            var queue = new Queue<Mob>();
            queue.Enqueue(iterMob);

            while (queue.Count > 0)
            {
                var currentMob = queue.Dequeue();
                foreach (var testedMob in protesters)
                {
                    if (testedMob == currentMob || assignedProtesters.Contains(testedMob))
                        continue;

                    if (currentMob.GlobalPosition.DistanceSquaredTo(testedMob.GlobalPosition) < _clusteringDistance * _clusteringDistance)
                    {
                        _protesterClusters[clusterIndex].Add(testedMob);
                        assignedProtesters.Add(testedMob);
                        queue.Enqueue(testedMob);
                    }
                }
            }
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

    public Mob SpawnMob(MobData mobData, Vector2 spawnPosition, bool addToMobs = true)
    {
        var newMob = _mobScene.Instantiate<Mob>();
        newMob.MobManager = this;
        newMob.GlobalPosition = spawnPosition;
        AddChild(newMob);
        newMob.SetData(mobData);
        if (addToMobs)
            _currentMobs.Add(newMob);
        return newMob;
    }

    public void AddMob(Mob mob)
    {
        if (_currentMobs.Contains(mob))
            return;
        _currentMobs.Add(mob);
    }
}