using Godot;

[GlobalClass]
public partial class PlayerActionData : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public Texture2D Sprite { get; private set; }
    [Export] public PackedScene Item { get; private set; }
}