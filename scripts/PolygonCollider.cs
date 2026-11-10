using Godot;

[GlobalClass]
public partial class PolygonCollider : Polygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var collider = new StaticBody2D();
		var shape = new CollisionPolygon2D();
		shape.Polygon = Polygon;
		collider.AddChild(shape);
		AddChild(collider);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
