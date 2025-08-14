using Godot;
using System;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using static Godot.TextServer;
public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 400;
	private Vector2 UltimaTecla = Vector2.Down;


	public AnimatedSprite2D AnimatorSprite;
	public CollisionShape2D colision;

	[Export] public bool podermoverse = true;
	
	private string AnimationString = "";
	private Timer timer;
	private Vector2 LastRegisteredDirection = Vector2.Down;
	public override void _Ready()
	{
        AnimatorSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		colision = GetNode<CollisionShape2D>("CollisionShape2D");
		timer = GetNode<Timer>("Timer");
	}
	public override void _Process(double delta)
	{
		if (podermoverse) 
			MovementControl();
		AnimationChecker();
	}
	void MovementControl()
	{
		Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		Velocity = input.Normalized() * Speed;
		if (Input.IsActionPressed("Correr"))
		{
            Speed = 150;
            AnimatorSprite.SpeedScale = 1.75f;
        }
		else if (Input.IsActionJustReleased("Correr"))
		{
            Speed = 100;
            AnimatorSprite.SpeedScale = 1.0f;
        }
		MoveAndSlide();
	}
	void AnimationChecker()
	{
		Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		var rectShape = colision.Shape as RectangleShape2D;

		if (input != Vector2.Zero)
		{
			LastRegisteredDirection = input.Normalized();
		}
		if(input == Vector2.Zero)
		{
			if (LastRegisteredDirection.Y < 0)
			{
                colision.Position = new Vector2(-1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                UltimaTecla = Vector2.Up;
				AnimatorSprite.FlipH = false;
				AnimationController("IdleUp");
			}
			if (LastRegisteredDirection.Y > 0)
			{
                colision.Position = new Vector2(-1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                UltimaTecla = Vector2.Down;
				AnimatorSprite.FlipH = false;
				AnimationController("IdleDown");
			}
			if (LastRegisteredDirection.X < 0)
			{
                colision.Position = new Vector2(-1.5f, 16.5f);
				rectShape.Size = new Vector2(15.0f, 4.25f);
                UltimaTecla = Vector2.Left;
				AnimatorSprite.FlipH = false;
				AnimationController("IdleLeft");
			}
			if (LastRegisteredDirection.X > 0)
			{
                colision.Position = new Vector2(1.5f, 16.5f);
				rectShape.Size = new Vector2(15.0f, 4.25f);
                UltimaTecla = Vector2.Right;
				AnimatorSprite.FlipH = true;
				AnimationController("IdleLeft");
			}
		}
		
		// Derecha
		if(input.X > 0 && input.Y == 0)
		{
            colision.Position = new Vector2(1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            UltimaTecla = Vector2.Right;
			AnimatorSprite.FlipH = true;
			AnimationController("WalkLeft");
		}

		// Izquierda
		if (input.X < 0 && input.Y == 0)
		{
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            UltimaTecla = Vector2.Left;
			AnimatorSprite.FlipH = false;
			AnimationController("WalkLeft");
		}
		// Arriba
		if (input.X == 0 && input.Y < 0)
		{
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            UltimaTecla = Vector2.Up;
			AnimatorSprite.FlipH = false;
			AnimationController("WalkUp");
		}

		// Abajo
		if (input.X == 0 && input.Y > 0)
		{
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            UltimaTecla = Vector2.Down;
            AnimatorSprite.FlipH = false;
			AnimationController("WalkDown");
		}

		if (input != Vector2.Zero) {
			if (UltimaTecla == Vector2.Right)
			{
				AnimatorSprite.FlipH = true;
				AnimationController("WalkLeft");
			}
			else if (UltimaTecla == Vector2.Left)
			{
				AnimatorSprite.FlipH = false;
				AnimationController("WalkLeft");
			}
			else if (UltimaTecla == Vector2.Up)
			{
				AnimatorSprite.FlipH = false;
				AnimationController("WalkUp");
			}
			else if (UltimaTecla == Vector2.Down)
			{
				AnimatorSprite.FlipH = false;
				AnimationController("WalkDown");
			}
		}
	}
	// si
	async void AnimationController(string animation, float time = 0f)
	{
		if(time > 0)
		{
			timer.WaitTime = time;
			timer.Start();
			await ToSignal(timer, Timer.SignalName.Timeout);
		}
		else
		{
			validateAnim();
		}
		void validateAnim()
		{
			if(AnimationString != animation)
			{
				AnimationString = animation;
				AnimatorSprite.Play(AnimationString);
			}
		}
	}
}
