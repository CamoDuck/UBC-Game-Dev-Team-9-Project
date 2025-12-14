using System.Collections.Generic;
using Godot;
using Team9Game.Scripts.Dialog;

public partial class DialogOptionUi : Control
{
	[Export]
	public Json DialogueData { get; set; }
	
	[Export]
	public string StartFrom { get; set; } = "";
	
	[Signal]
	public delegate void DialogFinishedEventHandler();

	private DialogContext context;
	
	private DialogRunner runner;

	private bool _allowAdvanceByKey;

	private bool _ignoreFirstNextRelease;

	public Stack<(string, int)> ChoicesStack => runner.ChoicesStack;

	#region UIInitialize
	[Export]
	public VBoxContainer ChoicesContainer { get; set; }
	
	[Export]
	public Label TextLabel { get; set; }
	
	[Export]
	public Label SpeakerLabel { get; set; }
	
	[Export]
	public TextureRect BackgroundImage { get; set; }
	
	[Export]
	public HBoxContainer SpeakerStack { get; set; }
	#endregion
	
	public override void _Ready()
	{
		//Here, initialize everything
		context = new DialogContext(DialogueData.Data.AsGodotDictionary());
		runner = new DialogRunner(context);
		if (string.IsNullOrEmpty(StartFrom)) 
			runner.Start();
		else runner.StartFrom(StartFrom);
		Update();
	}
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("dialog_next") && _ignoreFirstNextRelease)
		{
			if (runner.NoChoices)//
				ChoiceSelected(0);
			else
			{
				UpdateChoice();
				_ignoreFirstNextRelease = false;
			}
				
		}
		_ignoreFirstNextRelease = true;
	}

	private void Update()
	{
		BackgroundImage.Texture = runner.CurrentImage;
		TextLabel.Text = runner.Text;
		var speakers = runner.Speakers;
		SpeakerLabel.Text = string.Join('&', speakers);
		UpdateSpeakers();
		ClearChoices();
	}

	#region Choice Selection
	private void ClearChoices()
		=> ClearStack(ChoicesContainer);
	private void ClearStack(BoxContainer container)
	{
		var choices = container.GetChildren();
		foreach (var choice in choices)
			choice.QueueFree();
	}

	private void UpdateChoice()
	{
		var choices = runner.Choices;
		ClearChoices();
		InitializeChoice(choices);
	}

	private DialogChoiceUI NewChoiceUI(string choice, int index)
	{
		var scene = GD.Load<PackedScene>("res://Scenes/Dialog/DialogChoiceUI.tscn");
		var ret = scene.Instantiate<DialogChoiceUI>();
		ret.ChoiceIndex = index;
		ret.ChoiceText = choice;
		return ret;
	}

	private void OnChoiceSelected(DialogChoiceUI btn)
		=> ChoiceSelected(btn.ChoiceIndex);

	private void ChoiceSelected(int index)
	{
		if (runner.Next(index))
		{
			EmitSignalDialogFinished();
			return;
		}
		_ignoreFirstNextRelease = false;
		Update();
	}

	private void InitializeChoice(string[] choices)
	{
		var ind = 0;
		foreach (var choice in choices)
		{
			var choiceBtn = NewChoiceUI(choice, ind++);
			choiceBtn.Pressed += () => OnChoiceSelected(choiceBtn);
			ChoicesContainer.AddChild(choiceBtn);
		}
	}
	#endregion

	#region Speakers

	private void UpdateSpeakers()
	{
		ClearStack(SpeakerStack);
		foreach (var speaker in runner.Speakers)
		{
			SpeakerStack.AddChild(
				InitializeSpeakerShower(
					runner.SpeakerPortraits[speaker]
					));
		}
		
	}


	private DialogSpeakerShower InitializeSpeakerShower(Texture2D texture)
	{
		var tmp = GD.Load<PackedScene>("res://Scenes/Dialog/DialogSpeakerShower.tscn");
		var ret = tmp.Instantiate<DialogSpeakerShower>();
		ret.SpeakerTexture = texture;
		return ret;
	}
		

	#endregion
}
