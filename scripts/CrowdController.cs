using Godot;

[GlobalClass]
public partial class CrowdController : RigidBody2D
{
	[Export] public float MoveForce = 800;
	[Export] public float MaxSpeed = 100;

	private NavigationAgent2D _agent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_agent.PathDesiredDistance = 32;
		_agent.TargetDesiredDistance = 32;
	}

	public override void _PhysicsProcess(double delta)
	{
		_agent.TargetPosition = GetGlobalMousePosition();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_agent.IsNavigationFinished())
			return;

		var nextPosition = _agent.GetNextPathPosition();
		var direction = (nextPosition - GlobalPosition).Normalized();
		ApplyCentralForce(direction * MoveForce);
		if (state.LinearVelocity.LengthSquared() > MaxSpeed * MaxSpeed)
			state.LinearVelocity = state.LinearVelocity.Normalized() * MaxSpeed;
	}
}
