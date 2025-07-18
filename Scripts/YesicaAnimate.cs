using Godot;
using System;

public partial class YesicaAnimate : AnimatedSprite2D
{
	public override void _Process(double delta)
	{
		ObtenerAnimacion();
	}
	private void ObtenerAnimacion() 
	{
		Vector2 inputDirection = Input.GetVector("izquierda", "derecha", "arriba", "abajo");
		if (inputDirection.X == 1.0)
			Play("right");
		if (inputDirection.X == -1.0)
			Play("left");
		if (inputDirection.Y == 1.0)
			Play("up");
		if (inputDirection.Y == -1.0)
			Play("down");
	}
}
