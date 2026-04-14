using Godot;
using System;

public partial class Grass : Node2D
{
	private Area2D _hurtBox;
	private static readonly PackedScene GrassEffect = GD.Load<PackedScene>("res://effects/grass_effect.tscn");
	
	public override void _Ready()
	{
		_hurtBox = GetNode<Area2D>("HurtBox");
	}

	private void OnHurt(Node OtherHitbox)
	{
		Node2D grassEffectInstance = GrassEffect.Instantiate<Node2D>();
		GetTree().CurrentScene.AddChild(grassEffectInstance);
		grassEffectInstance.GlobalPosition = GlobalPosition;
		QueueFree();
	}
}