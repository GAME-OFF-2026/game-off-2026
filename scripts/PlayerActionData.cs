using Godot;

[GlobalClass]
public partial class PlayerActionData : Resource
{
    [Export] public uint Uses = 1;
    [Export] public string Name { get; private set; }
    [Export] public Texture2D Sprite { get; private set; }
    [Export] public PackedScene Item { get; private set; }
    [Export] public GamePhase Phase { get; private set; }
}