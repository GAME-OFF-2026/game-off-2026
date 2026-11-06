using Godot;

[GlobalClass]
public partial class MobData : Resource
{
	[Export] public Texture2D Sprite;
	[Export] public MobStrategy Strategy;
	[Export] public float MoveForce = 800;
	[Export] public float MaxSpeed = 100;
}
