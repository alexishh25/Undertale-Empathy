using Godot;
using System;
using System.ComponentModel;

public partial class yesicaanimationtree : AnimationTree
{
	[Export]
	private AnimationTree animation_tree { get; set; }

	private Mecanica player;

	public override void _Ready()
	{
		player = GetOwner() as Mecanica;
	}

	public override void _PhysicsProcess(double delta)
	{
		
	}
}
