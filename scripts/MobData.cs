using Godot;

public enum Faction
{
    Protester,
    Fascist
}

[GlobalClass]
public partial class MobData : Resource
{
    [Export] public MobSpriteLayer[] Layers { get; private set; }
    [Export] public MobStrategy Strategy { get; private set; }
    [Export] public double MaxHealth { get; private set; } = 100;
    [Export] public double Dps { get; private set; } = 20;
    [Export] public float MoveForce { get; private set; } = 800;
    [Export] public float MaxSpeed { get; private set; } = 100;
    [Export] public Vector2 MotivationThresholdMinMax { get; private set; } = new(0.05f, 0.95f);
    [Export] public Vector2 MotivationGainRateMinMax { get; private set; } = new(0.1f, 1f);
    [Export] public Vector2 MotivationLossRateMinMax { get; private set; } = new(0.1f, 1f);
    [Export] public float SecondsToSwitchMotivation { get; private set; } = 2;
    [Export] public Faction Faction { get; private set; }
}