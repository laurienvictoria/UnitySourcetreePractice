using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Ink.Runtime;

public class DialogueAssetBuilder : MonoBehaviour
{

    private static int s_index = 0;
    private const string AUDIO_TAG_PREFIX = "#audio-";

    [MenuItem("Assets/Build to Dialogue Data Assets")]
    static void CreateDialogueAssets()
    {
        UnityEngine.Object selectedObject = Selection.activeObject;
        string _current_path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
        TextAsset m_textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(_current_path);

        Story _story = new Story(m_textAsset.text);
        string _path = string.Concat(Constants.Paths.PATH_TO_DIALOGUE_DATA, m_textAsset.name, Constants.Paths.ASSET_EXTENSION);
        DialogueDataBank _dialogueDataBank = AssetDatabase.LoadAssetAtPath<DialogueDataBank>(_path);

        if (_dialogueDataBank == null)
        {
            _dialogueDataBank = ScriptableObject.CreateInstance<DialogueDataBank>();
            AssetDatabase.CreateAsset(_dialogueDataBank, _path);
            _dialogueDataBank.lineDictionary = new DialogueDataBank.DialogueDictionary();
        }

        while (_story.canContinue)
        {
            string currentLine = _story.Continue();
            string lineKey = string.Format("{0}_{1}", m_textAsset.name, GetAudioTagID(_story));

            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(Constants.Paths.PATH_TO_AUDIO_FILES + lineKey + Constants.Paths.AUDIO_EXTENSION);
            if(clip == null)
            {
                continue;
            }

            float currentDuration = clip.length;
            DialogueData result = new DialogueData(currentLine, currentDuration,clip);

            if (_dialogueDataBank.lineDictionary.TryAdd(lineKey, result) == false)
            {
                _dialogueDataBank.lineDictionary[lineKey] = result;
            }
            s_index = s_index + 1;
        }
        EditorUtility.SetDirty(_dialogueDataBank);
        AssetDatabase.SaveAssets();
        s_index = 0;
    }

    [MenuItem("Assets/Build to Dialogue Data Assets", true)]
    static bool CreateDialogueAssetsValidation()
    {
        UnityEngine.Object selectedObject = Selection.activeObject;
        string path;

        if (selectedObject == null)
            return false;

        path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
        return path.EndsWith(Constants.Paths.JSON_EXTENSION);
    }

    [MenuItem("Assets/Add Audio Tags", true)]
    private static bool TagWithAudioValidation()
    {
        UnityEngine.Object selectedObject = Selection.activeObject;
        string path;

        if (selectedObject == null)
            return false;

        path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
        return path.EndsWith(Constants.Paths.INK_EXTENSION);
    }

    [MenuItem("Assets/Add Audio Tags")]
    public static void AddAudioTags()
    {
        foreach (UnityEngine.Object selected in Selection.objects)
        {
            string filePath = AssetDatabase.GetAssetPath(selected.GetInstanceID());
            AddAudioTags(filePath);
        }
    }
    public static void AddAudioTags(string filePath)
    {

        if (!filePath.EndsWith(Constants.Paths.INK_EXTENSION))
        {
            Debug.LogErrorFormat("Tried to tag non-Ink file {0} with audio tags!", filePath);
            return;
        }

        List<int> needsTagIndices = new List<int>();
        HashSet<int> availableTagIDs = new HashSet<int>(Enumerable.Range(0, 10000));   


        string[] lines = File.ReadAllLines(filePath);

        for(int i = 0; i<lines.Length; i++)
        {
            string currentLine = lines[i];
            if (!IsDialogue(currentLine))
            {
                continue;
            }

            MatchCollection matches = Regex.Matches(currentLine, Constants.Syntax.AUDIO_TAG_SYNTAX);
            if(matches == null || matches.Count<1)
            {
                needsTagIndices.Add(i);
                continue;
            }
        }

        if(needsTagIndices.Count < 1)
        {
            Debug.LogFormat("No new audio tags needed for {0}!", filePath);
            return;
        }

        List<int> availableTagIDsList = availableTagIDs.OrderBy(item => Random.Range(int.MinValue, int.MaxValue)).ToList();

        for (int i = 0; i<needsTagIndices.Count; ++i)
        {
            string audioTag = string.Format(" {0}{1}", AUDIO_TAG_PREFIX, availableTagIDsList[i].ToString("D4"));

            int lineIndex = needsTagIndices[i];
            string line = lines[lineIndex];
            int insertIndex = line.IndexOf("//");
            insertIndex = Mathf.Max(insertIndex, line.IndexOf("/*"));
            insertIndex = insertIndex >= 0 ? insertIndex : line.Length;

            lines[lineIndex] = line.Insert(insertIndex, audioTag);
        }
        
        File.WriteAllLines(filePath, lines);

        Debug.LogFormat("Audio tagging completed for {0}!", filePath);
    }

    public static bool IsDialogue(string line)
    {
        return !string.IsNullOrEmpty(line);       // TODO: Can add regex conditions to check if it is a dialogue line so that stage directions and VO notes can be added in
    }

    public static string GetAudioTag(Story _story)
    {
        List<string> tags = _story.currentTags;
        if(tags == null || tags.Count < 1)
        {
            return string.Empty;
        }

        for(int i=0; i<tags.Count; i++)
        {
           
            if(Regex.IsMatch(tags[i],Constants.Syntax.AUDIO_SYNTAX))
            {
                return tags[i];
            }
            
        }

        return string.Empty;
    }

    public static string GetAudioTagID(Story story)
    {
        string tag = GetAudioTag(story);
        if(tag == string.Empty)
        {
            return string.Empty;
        }

        Match m = Regex.Match(tag, Constants.Syntax.AUDIO_SYNTAX);
        if(m.Groups == null || m.Groups.Count<2 || m.Groups["dialogueID"] == null)
        {
            return string.Empty;
        }

        string id = m.Groups[2].ToString();
        return id;
    }
}
