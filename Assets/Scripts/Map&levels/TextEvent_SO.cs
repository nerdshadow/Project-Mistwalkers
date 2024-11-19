using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TextEvent_SO", menuName = "SO/Event_Text")]
public class TextEvent_SO : ScriptableObject, IRoadEvent
{
    public string eventText = "Text";
    public List<string> eventAnswers = new List<string>();

    public void EndEvent()
    {
        Debug.Log("Text event ended");
    }

    public void StartEvent()
    {
        Debug.Log("Text event started");
    }
    void ChangeText(TMP_Text tMP_Text)
    {
        tMP_Text.text = eventText;
    }
}
