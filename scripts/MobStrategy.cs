using Godot;

[GlobalClass]
public abstract partial class MobStrategy : Resource
{
	public abstract Vector2 GetDestination(Mob mob);
}
