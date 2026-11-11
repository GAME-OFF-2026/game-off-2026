using Godot;

[GlobalClass]
public partial class Mob : RigidBody2D
{
    private Sprite2D _sprite;
    private NavigationAgent2D _agent;

    public MobManager MobManager;

    public double Motivation;
    private double _motivationThreshold;
    private double _motivationSwitchTimer;

    public double MotivationGainRate { get; private set; }
    public double MotivationLossRate { get; private set; }

    public bool IsProtesting { get; private set; }

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

        _motivationThreshold = Utils.Rng.RandfRange(Data.MotivationThresholdMinMax.X, Data.MotivationThresholdMinMax.Y);
        MotivationGainRate = Utils.Rng.RandfRange(Data.MotivationGainRateMinMax.X, Data.MotivationGainRateMinMax.Y);
        MotivationLossRate = Utils.Rng.RandfRange(Data.MotivationLossRateMinMax.X, Data.MotivationLossRateMinMax.Y);

        IsProtesting = Motivation > _motivationThreshold;

        _sprite.Texture = Data.Sprite;
    }

    public override void _Process(double delta)
    {
        _sprite.Modulate = Color.FromHsv(
            0,
            (float)(IsProtesting ? Motivation : Motivation / 2f),
            1
        );
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Data == null)
            return;

        Data.Strategy.Tick(delta);
        Motivation = Mathf.Clamp(Motivation, 0, 1);

        if (IsProtesting ? Motivation < _motivationThreshold : Motivation > _motivationThreshold)
        {
            _motivationSwitchTimer += delta;
            if (_motivationSwitchTimer >= Data.SecondsToSwitchMotivation)
                IsProtesting = !IsProtesting;
        }
        else
        {
            _motivationSwitchTimer = 0;
        }

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

    public void ForceProtesting(double motivation)
    {
        IsProtesting = true;
        Motivation = motivation;
        _motivationSwitchTimer = 0;
    }
}