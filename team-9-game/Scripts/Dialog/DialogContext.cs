using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Team9Game.Scripts.Dialog;

public class DialogContext
{
    public string Title { get; private set; }
    
    public string ID { get; private set; }

    public System.Collections.Generic.Dictionary<string, DialogNode> Nodes = [];
    
    public string StartID { get;private set; }
    
    public string InitialBackGroundUri { get; private set; }

    /// <summary>
    /// key: character name
    /// <br/>
    /// value: character portrait uri
    /// </summary>
    public System.Collections.Generic.Dictionary<string, string> Characters = [];

    public DialogContext(Dictionary dialogData)
    {
        if (!CheckData(dialogData))
            throw new ArgumentException("Input dictionary is not a valid dialog data");
        Title = dialogData["title"].AsString();
        ID = dialogData["id"].AsString();
        StartID = dialogData["startId"].AsString();
        InitialBackGroundUri = dialogData["background"].AsString();
        InitializeChar(dialogData["characters"].AsGodotDictionary());
        InitializeNode(dialogData["nodes"].AsGodotArray());
    }
    
    public DialogContext(){}

    private bool CheckData(Dictionary dialogData)
    {
        var type = dialogData["type"].AsString();
        return type == "dialog";
    }

    private void InitializeChar(Dictionary dialogData)
    {
        foreach (var item in dialogData)
        {
            var chr = item.Key.AsString();
            var uri = item.Value.AsString();
            Characters.Add(chr, uri);
        }
    }

    private void InitializeNode(Array array)
    {
        foreach (var item in array)
        {
            var dialogNode = new DialogNode(item.AsGodotDictionary());
            Nodes.Add(dialogNode.ID, dialogNode);
        }
    }
}