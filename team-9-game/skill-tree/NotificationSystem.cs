using Godot;
using System;

public partial class NotificationSystem : Node
{
	private CanvasLayer _canvasLayer;
	private Control _container;
	private PackedScene _notifItemScene;

	private const int MARGIN_LEFT = 25;
	private const int MARGIN_BOTTOM = 60;
	private const int SLOT_HEIGHT = 70; // Approx height per notification
	private int _activeCount = 0;

	public override void _Ready()
	{
		_notifItemScene = GD.Load<PackedScene>("res://skill-tree/notification_item.tscn");

		_canvasLayer = new CanvasLayer();
		_canvasLayer.Layer = 100;
		AddChild(_canvasLayer);

		// Simple container
		_container = new Control();
		_container.MouseFilter = Control.MouseFilterEnum.Ignore;
		_canvasLayer.AddChild(_container);
		
		GD.Print("📣 NotificationSystem ready!");
	}

	public void Notify(string text, Texture2D icon)
	{
		if (_notifItemScene == null)
		{
			GD.PrintErr("notification_item.tscn not loaded!");
			return;
		}

		GD.Print($"✨ Notify: {text}");

		var viewport = GetViewport();
		Vector2 screenSize = viewport.GetVisibleRect().Size;

		NotificationItem item = _notifItemScene.Instantiate<NotificationItem>();
		item.MouseFilter = Control.MouseFilterEnum.Ignore;
		_container.AddChild(item);

		// Force a layout update so we can read the size
		item.ResetSize();

		// Position from the bottom-left, stacking upward
		float x = MARGIN_LEFT;
		float y = screenSize.Y - MARGIN_BOTTOM - SLOT_HEIGHT - (_activeCount * (SLOT_HEIGHT + 10));
		item.Position = new Vector2(x, y);

		_activeCount++;
		item.Setup(text, icon);

		// Decrement counter when this item is freed
		item.TreeExited += () => _activeCount = Mathf.Max(0, _activeCount - 1);
	}
}
