using Godot;

[GlobalClass]
public partial class FollowProtester: MobStrategy
{
	public override Vector2 GetDestination(Mob mob) => Vector2.Zero;
}
