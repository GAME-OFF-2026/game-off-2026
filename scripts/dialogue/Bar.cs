using Godot;

[GlobalClass]
public partial class Bar : Node
{
    [Export] public Texture2D[] BarTextures { get; set; } = new Texture2D[4];

    public void SetBarPhase(FutureTimeline timeline)
    {
        GetNode<Sprite2D>("%Bar").Texture = BarTextures[(int)timeline];
    }
}