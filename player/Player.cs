using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float Speed = 100.0f;
	private const float RollSpeed = 150.0f;
	
	private Vector2 _inputVector = Vector2.Zero;
	private Vector2 _lastInputVector = Vector2.Zero;
	
	private AnimationTree _animationTree;
	private AnimationNodeStateMachinePlayback _stateMachine;
	private AnimationNodeStateMachinePlayback _moveState;
	private HitBox _hitbox;

	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_hitbox = GetNode<HitBox>("HitBox");
		
		_stateMachine = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/playback");
		_moveState = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/MoveState/playback");
		
		AddToGroup("player");
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = Vector2.Zero;
		string state = _stateMachine.GetCurrentNode();
		
		switch (state)
		{
			case "MoveState":
				_move(delta);
				break;
			case "AttackState":
				_attack(delta);
				break;
			case "RollState":
				_roll(delta);
				break;
			default:
				_move(delta);
				break;
		}
	}
	
	private void _move(double delta)
	{
		_inputVector = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (Input.IsActionJustPressed("attack"))
		{
			_stateMachine.Travel("AttackState");
		}
		else if (Input.IsActionJustPressed("roll"))
		{
			_stateMachine.Travel("RollState");
		}
		else if (_inputVector != Vector2.Zero)
		{
			_lastInputVector = _inputVector;
			Vector2 directionVector = new Vector2(_inputVector.X, -_inputVector.Y);
			_updateBlendPositions(directionVector);
			_moveState.Travel("RunState");
			Velocity = _inputVector * Speed;
		}
		else
		{
			_moveState.Travel("StandState");
		}
		MoveAndSlide();
	}
	
	private void _attack(double delta)
	{
		if (!_stateMachine.IsPlaying())
		{
			_stateMachine.Travel("MoveState");
		}
	}
	
	private void _roll(double delta)
	{
		Velocity = _lastInputVector * RollSpeed;
		MoveAndSlide();
		
		if (!_stateMachine.IsPlaying())
		{
			_stateMachine.Travel("MoveState");
		}
	}
	
	private void _updateBlendPositions(Vector2 directionVector)
	{
		_animationTree.Set("parameters/StateMachine/MoveState/RunState/blend_position", directionVector);
		_animationTree.Set("parameters/StateMachine/MoveState/StandState/blend_position", directionVector);
		_animationTree.Set("parameters/StateMachine/AttackState/blend_position", directionVector);
		_animationTree.Set("parameters/StateMachine/RollState/blend_position", directionVector);
	}
}
