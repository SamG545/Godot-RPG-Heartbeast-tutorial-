using Godot;
using System;

public partial class HitBox : Area2D
{
	[Export] public int Damage { get; set; } = 1;
	
	[Export] public int KnockbackAmount { get; set; } = 200;

	[Export] public Vector2 KnockbackDirection;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
