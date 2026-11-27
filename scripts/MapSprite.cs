using Godot;

[GlobalClass]
public partial class MapSprite : Sprite2D
{
	[Export] private Texture2D _dayMap;
	[Export] private Texture2D _eveningMap;

	private GameManager _gameManager;
	[Export] private Texture2D _morningMap;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("%GameManager");
	}

	public override void _Process(double delta)
	{
		Texture = _gameManager.GamePhase switch
		{
			GamePhase.Setup => _morningMap,
			GamePhase.Protest => _dayMap,
			GamePhase.Evening => _eveningMap,
			_ => Texture
		};
	}
}
