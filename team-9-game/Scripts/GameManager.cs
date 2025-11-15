using Godot;
using System;
using System.Dynamic;

public partial class GameManager : Node
{
    static private GameManager singleton = null;
    // Global Exports

    [Export] public PlayerController playerController;
    [Export] public CameraScript mainCamera;

    public override void _EnterTree()
    {
        singleton = this;
    }

    static public GameManager Get()
    {
        return singleton;
    }

}
