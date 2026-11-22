using Godot;

[GlobalClass]
public partial class PlayerActionData : Resource
{
    [Export] public uint Uses = 1;
    [Export] public bool IsAvailableAtSetup = true;
    [Export] public string Name { get; private set; }
    [Export] public Texture2D Sprite { get; private set; }
    [Export] public PackedScene Item { get; private set; }
}