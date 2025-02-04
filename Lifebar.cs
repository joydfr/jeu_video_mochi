using Godot;
using System;

public partial class Lifebar : ProgressBar
{
	public override void _Ready()
	{
		Show();
		MinValue = 0;
		MaxValue = 100;
		Value = 100; // Valeur initiale à 100%
	}

	public void UpdateHealth(float value)
	{
		Value = value;
	}
	
}

