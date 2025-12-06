using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public partial class ForceManager : Node
{
    class Force
    {
        Vector2 direction;
        float strength;
        float duration;
        float timeLeft;
        Func<float, float> interpolationFunction; // (noramlizedTimeLeft) => normalizedStrength;

        public Force(Vector2 direction, float strength, float duration, Func<float, float> interpolationFunction)
        {
            this.direction = direction.Normalized();
            this.strength = strength;
            this.duration = duration;
            this.timeLeft = duration;
            this.interpolationFunction = interpolationFunction;
        }

        public Vector2 GetDisplacement(float dt)
        {
            Vector2 newVelocity = direction * strength * dt * interpolationFunction(timeLeft / duration);
            timeLeft -= dt;
            return newVelocity;
        }

        public Boolean shouldRemove()
        {
            return timeLeft <= 0;
        }
    }

    CharacterBody2D parent;
    List<Force> appliededForces = new List<Force>();

    /// <summary>
    /// Applies a force to a PhysicsBody2D over a duration with a changing strength.
    /// </summary>
    /// <param name="direction">The direction vector of the force.</param>
    /// <param name="strength">The peak strength of the force at the start.</param>
    /// <param name="interpolationFunction">The type of interpolation function to use for the strength over time (from 1.0 to 0.0). (strength, %ofTimeLeft) => strength </param>
    /// <param name="duration">The duration in seconds over which the force is applied.</param>
    public void ApplyForce(
        Vector2 direction,
        float strength,
        float duration,
        Func<float, float> interpolationFunction)
    {
        appliededForces.Add(new Force(direction, strength, duration, interpolationFunction));
    }

    public override void _Ready()
    {
        parent = GetParent<CharacterBody2D>();
        if (parent == null)
        {
            GD.PrintErr("Missing parent 'CharacterBody2D' for 'Gravity' node");
            SetProcess(false); // Stop script if there is no parent
        }
    }


    public override void _Process(double delta)
    {
        float dt = (float)delta;

        for (int i = appliededForces.Count - 1; i >= 0; i--)
        {

            Force force = appliededForces[i];
            Vector2 displacement = force.GetDisplacement(dt);
            parent.Position += displacement;
            if (force.shouldRemove())
            {
                appliededForces.RemoveAt(i);
            }
        }

    }

}
