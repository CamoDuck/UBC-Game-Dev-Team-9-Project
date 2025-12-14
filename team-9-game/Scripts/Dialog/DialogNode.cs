#nullable enable
using System.Collections.Generic;
using GDictionary = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;

namespace Team9Game.Scripts.Dialog;

public class DialogNode
{
    /// <summary>
    /// This is the ID for this dialog node.
    /// It should be unique within the dialog tree.
    /// </summary>
    public string ID { get; private set; } = "";

    public string Text { get; private set; } = "";

    /// <summary>
    /// If there is only one response,
    /// it means there is no choice to be made,
    /// and the dialog will proceed automatically.
    /// </summary>
    public List<(string option, string value)> Options = [];

    public string[] Speaker { get; private set; } = [];

    /// <summary>
    /// If it is an empty string or invalid, the background image will
    /// not change.
    /// </summary>
    public string? BackgroundChange;

    public Dictionary<string, string>? SpeakerChanges;
    
    public string? TitleChange;

    public DialogNode(){}
    
    public DialogNode(GDictionary data)
    {
        ID = data["id"].AsString();
        Text = data["text"].AsString();
        Speaker = data["speaker"].AsStringArray();
        InitializeOptions(data["options"].AsGodotArray());
        if (data.ContainsKey("changes"))
            InitializeChanges(data["changes"].AsGodotDictionary());
    }

    private void InitializeOptions(GArray data)
    {
        foreach (var item in data)
        {
            var opts = item.AsStringArray();
            Options.Add((opts[0], opts[1]));
        }
    }

    private void InitializeChanges(GDictionary data)
    {
        foreach (var item in data)
        {
            var key = item.Key.AsString();
            var value = item.Value;
            switch (key)
            {
                case "background":
                    BackgroundChange = value.AsString();
                    break;
                case "speakers":
                    InitializeSpeakerChanges(value.AsGodotDictionary());
                    break;
                case "title":
                    TitleChange = value.AsString();
                    break;
            }
        }
    }
    
    private void InitializeSpeakerChanges(GDictionary data)
    {
        SpeakerChanges = [];
        foreach (var item in data)
        {
            SpeakerChanges[item.Key.AsString()] = item.Value.AsString();
        }
    }
}