using Godot;

public enum Faction
{
    Protester,
    Fascist
}

[GlobalClass]
public partial class MobData : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public MobStrategy Strategy;
    [Export] public float MoveForce = 800;
    [Export] public float MaxSpeed = 100;
    [Export] public Vector2 MotivationThresholdMinMax = new(0.05f, 0.95f);
    [Export] public Vector2 MotivationGainRateMinMax = new(0.1f, 1f);
    [Export] public Vector2 MotivationLossRateMinMax = new(0.1f, 1f);
    [Export] public float SecondsToSwitchMotivation = 2;
    [Export] public Faction Faction;
}