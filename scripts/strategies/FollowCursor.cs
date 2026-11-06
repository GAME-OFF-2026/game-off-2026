using Godot;

[GlobalClass]
public partial class FollowCursor : MobStrategy
{
	public override Vector2 GetDestination(Mob mob) => mob.GetGlobalMousePosition();
}
