using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class DialogueData
{
    [JsonIgnore] private List<String> _dialogueList = new List<string>();
    public List<string> DialogueList
    {
        get => _dialogueList;
        set => _dialogueList = value;
    }

    public DialogueData(List<string> dialogueData)
    {
        this._dialogueList = dialogueData;
    }
}

public class DialogueDataLoader : Singleton<DialogueDataLoader>
{
    public void SaveDialogueData(DialogueData holder, string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Dialogue/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(holder, converter);
        File.WriteAllText(path, pDataStringSave);
    }

    public DialogueData LoadDialogueData(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Dialogue/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        DialogueData playerData = JsonConvert.DeserializeObject<DialogueData>(pDataStringLoad, converter);

        return playerData;
    }
}