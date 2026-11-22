using Godot;
using System;

[GlobalClass]
public partial class Bar : Node
{
    [Export] public Texture2D[] BarTextures { get; set; } = new Texture2D[4];


	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void SetBarPhase(FutureTimeline timeline)
	{
		GetNode<Sprite2D>("%Bar").Texture = BarTextures[(int)timeline];
	}

}
