using Godot;

[GlobalClass]
public partial class FollowPoint : MobStrategy
{
	public override Vector2 GetDestination(Mob mob) => Vector2.Zero;
}
