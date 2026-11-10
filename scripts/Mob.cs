using Godot;

[GlobalClass]
public partial class Mob : RigidBody2D
{
	private Sprite2D _sprite;
	private NavigationAgent2D _agent;

	public MobManager MobManager;
	[Export] public MobData Data { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_agent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void SetData(MobData data)
	{
		if (data == null)
		{
			GD.PrintErr("SetData: data is null");
			return;
		}

		Data = data.DuplicateDeep() as MobData;
		if (Data == null)
		{
			GD.PrintErr("SetData: data duplication failed");
			return;
		}

		Data.Strategy.Initialize(this);

		_sprite.Texture = Data.Sprite;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Data == null)
			return;

		Data.Strategy.Tick(delta);
		_agent.TargetPosition = Data.Strategy.GetDestination();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (Data == null)
			return;
		if (_agent.IsNavigationFinished())
			return;

		var nextPosition = _agent.GetNextPathPosition();
		var direction = (nextPosition - GlobalPosition).Normalized();

		ApplyCentralForce(direction * Data.MoveForce);
		if (state.LinearVelocity.LengthSquared() > Data.MaxSpeed * Data.MaxSpeed)
			state.LinearVelocity = state.LinearVelocity.Normalized() * Data.MaxSpeed;
	}
}
