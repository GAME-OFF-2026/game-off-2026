using Godot;

[GlobalClass]
public partial class MobSpriteLayer : Resource
{
    [Export] public Texture2D[] SpriteVariants { get; private set; }
    [Export] public bool ModulateColor { get; private set; }
    [Export] public float Scale { get; private set; } = 1;
    [Export] public Vector2 Offset { get; private set; }
}