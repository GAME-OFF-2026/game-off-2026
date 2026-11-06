using Godot;

[GlobalClass]
public partial class Mob : RigidBody2D
{
	[Export] private MobData _data;
	private Sprite2D _sprite;
	private NavigationAgent2D _agent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		_sprite = GetNode<Sprite2D>("Sprite2D");
		_sprite.Texture = _data.Sprite;
	}

	public override void _PhysicsProcess(double delta)
	{
		_agent.TargetPosition = _data.Strategy.GetDestination(this);
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_agent.IsNavigationFinished())
			return;

		var nextPosition = _agent.GetNextPathPosition();
		var direction = (nextPosition - GlobalPosition).Normalized();

		ApplyCentralForce(direction * _data.MoveForce);
		if (state.LinearVelocity.LengthSquared() > _data.MaxSpeed * _data.MaxSpeed)
			state.LinearVelocity = state.LinearVelocity.Normalized() * _data.MaxSpeed;
	}
}
