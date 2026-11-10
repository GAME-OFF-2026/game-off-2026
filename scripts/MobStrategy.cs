using Godot;

[GlobalClass]
public abstract partial class MobStrategy : Resource
{
	protected Mob Mob { get; private set; }

	public void Initialize(Mob mob)
	{
		Mob = mob;
	}

	public virtual void Tick(double delta)
	{
	}

	public abstract Vector2 GetDestination();
}
