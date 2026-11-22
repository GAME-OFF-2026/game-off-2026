using System.Linq;
using Godot;

[GlobalClass]
public partial class TearGasManager : Node
{
    private PackedScene _tearGasScene = GD.Load<PackedScene>("res://mobs/tear_gas.tscn");

    private double _tearGasPeriod = 20;
    private double _timeSinceLastTearGas;

    private MobManager _mobManager;

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("%MobManager");
    }

    public override void _Process(double delta)

    {
        _timeSinceLastTearGas += delta;
        if (_timeSinceLastTearGas >= _tearGasPeriod)
        {
            _timeSinceLastTearGas = 0;

            var biggestCluster = _mobManager.ProtesterClusters.MaxBy(c => c.Count);

            var clusterCenter = new Vector2();
            foreach (var mob in biggestCluster)
                clusterCenter += mob.GlobalPosition / biggestCluster.Count;

            var newTearGas = _tearGasScene.Instantiate<PlayerActionItem>();
            newTearGas.GlobalPosition = clusterCenter;
            AddChild(newTearGas);
            newTearGas.IsPlaced = true;
            _mobManager.PlacedItems.Add(newTearGas);
        }
    }
}