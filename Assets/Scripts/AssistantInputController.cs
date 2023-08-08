using IBM.Cloud.SDK.Utilities;
using UnityEngine;

public class AssistantInputController : InputFieldController
{
    private AssistantWithText _watsonAssistant;
    
    private void Awake()
    {
        _watsonAssistant = FindObjectOfType<AssistantWithText>();
    }

    protected override void SendMessage(string text)
    {
        if (_watsonAssistant != null)
        {
            Runnable.Run(_watsonAssistant.ProcessChat(text));
        }
        else
        {
            Debug.LogError("AssistantInputController: _watsonAssistant is null");
        }
    }
}
