using Godot;

public enum Faction
{
    Protester,
    Fascist
}

[GlobalClass]
public partial class MobData : Resource
{
    [Export] public Texture2D Sprite { get; private set; }
    [Export] public MobStrategy Strategy { get; private set; }
    [Export] public float MoveForce { get; private set; } = 800;
    [Export] public float MaxSpeed { get; private set; } = 100;
    [Export] public Vector2 MotivationThresholdMinMax { get; private set; } = new(0.05f, 0.95f);
    [Export] public Vector2 MotivationGainRateMinMax { get; private set; } = new(0.1f, 1f);
    [Export] public Vector2 MotivationLossRateMinMax { get; private set; } = new(0.1f, 1f);
    [Export] public float SecondsToSwitchMotivation { get; private set; } = 2;
    [Export] public Faction Faction { get; private set; }
}