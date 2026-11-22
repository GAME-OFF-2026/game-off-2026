using System.Linq;
using Godot;

[GlobalClass]
public partial class Barricade : PlayerActionItem
{
    [Export] private double _maxHealth = 500;
    public double Health;

    private Sprite2D[] _sprites;
    private double _healthFlashCounter;

    public double DamageRadius => 150;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Health = _maxHealth;
        _sprites = [.. GetChildren().OfType<Sprite2D>()];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GetNode<CollisionShape2D>("%CollisionShape2D").SetDisabled(!IsPlaced);

        var healthFraction = Health / _maxHealth;
        var flashIntensity = 1 - healthFraction * healthFraction;
        _healthFlashCounter += 10 * flashIntensity * delta;

        foreach (var sprite in _sprites)
            sprite.Modulate = Color.FromHsv(
                0,
                (float)(flashIntensity * (1 + Mathf.Cos(_healthFlashCounter)) / 2),
                1
            );


        if (Health <= 0)
            QueueFree();
    }
}