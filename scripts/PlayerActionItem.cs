using Godot;

public enum PlayerActionItemType
{
    Other,
    RallyFlag,
    AvoidArea
}

[GlobalClass]
public partial class PlayerActionItem : Node2D
{
    public bool IsPlaced;

    [Export] public PlayerActionItemType Type { get; private set; }
    [Export] public double Duration { get; private set; }

    public override void _Process(double delta)
    {
        if (IsPlaced)
            Duration -= delta;

        if (Duration <= 0)
            QueueFree();
    }
}