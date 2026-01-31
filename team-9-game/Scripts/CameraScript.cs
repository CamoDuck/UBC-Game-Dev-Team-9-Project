using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public partial class CameraScript : Camera2D
{
    [Export] float followSpeed = 5;

    PlayerController playerController;
    public override void _Ready()
    {
        GameManager gm = GameManager.Get();
        playerController = gm.playerController;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        Vector2 playerPosition = playerController.Position;
        Position = Position.Lerp(playerPosition, dt * followSpeed);
    }


}
