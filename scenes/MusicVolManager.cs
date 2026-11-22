using Godot;

public partial class MusicVolManager : HSlider
{
	[Export] public string BusName = "Musique";

	private int _busIndex;
	public override void _Ready()
	{
		FocusMode = FocusModeEnum.None;

		_busIndex = AudioServer.GetBusIndex(BusName);

		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
		ValueChanged += OnValueChanged;
	}

	private void OnValueChanged(double v)
	{
		AudioServer.SetBusVolumeDb(_busIndex, Mathf.LinearToDb((float)v));
		AudioServer.SetBusMute(_busIndex, v <= 0.001);
	}
}
