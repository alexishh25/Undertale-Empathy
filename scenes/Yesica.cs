using Godot;
using System;

public partial class Yesica : CharacterBody2D
{
	private Camera2D camera;
	private AnimatedSprite2D animatedsprite2d;
	private AnimationPlayer animationplayer;

	private int idle_key;
	private bool idle_flip;
	[Export] public int Speed { get; set; } = 100;

	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");
		idle_key = 0;
		idle_flip = false;
		GD.Print($"Posicion de Yesica: {Position.X}");
		GD.Print($"Posicion de global?: {GlobalPosition.X}");
		animatedsprite2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationplayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("izquierda", "derecha", "arriba", "abajo");
		Velocity = inputDirection * Speed;
		if (inputDirection.X != 0)
			GD.Print($"Movimiento eje x : {inputDirection.X}");
		if (inputDirection.Y != 0)
			GD.Print($"Movimiento eje y : {inputDirection.Y}");
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();

		camera.GlobalPosition = new Vector2(GlobalPosition.X, 0f);

		string animation = GetNewAnimation();
		if (animation != animationplayer.CurrentAnimation)
		{
			animationplayer.GetAnimation("idle").TrackSetKeyValue(1, 0, idle_key);
			animationplayer.GetAnimation("walk_horizontal").TrackSetKeyValue(2, 0, idle_flip); //Se modifica la key de walk_horizontal
			animationplayer.Play(animation);
		}

		//Correr
		if (Input.IsActionJustPressed("correr")) {
			Speed = 180;
			animationplayer.SpeedScale = 1.5f;
		}
		else if (Input.IsActionJustReleased("correr"))
		{
			animationplayer.SpeedScale = 1.0f;
			Speed = 100;
		}

		//Menu

	}


	private string GetNewAnimation()
	{
		if (Velocity.Y > 0)
		{
			GD.Print($"{Velocity.Y}");
			idle_key = 0;
			return "walk_down";
		}
		else if (Velocity.Y < 0)
		{
			GD.Print($"{Velocity.Y}");
			idle_key = 2;
			return "walk_up";
		}
		else if (Velocity.X != 0)
		{
			idle_key = 1;
			idle_flip = Velocity.X > 0;
			return "walk_horizontal";
		}

		return "idle";
	}
}
