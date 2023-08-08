using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using TextSpeech;
using TMPro;
using IBM.Cloud.SDK.Utilities;

public class VoiceController : MonoBehaviour
{
    // Start is called before the first frame update
    const string LANG_CODE = "es-419";
    private AssistantWithText _watsonAssistant;
    public GameObject modelAnimation;

    [SerializeField]
    TMP_Text uiText;
    private void Start()
    {
        Setup(LANG_CODE);
        TextSpeech.SpeechToText.Instance.onPartialResultsCallback = OnPartialSpeechResult;
        TextSpeech.SpeechToText.Instance.onResultCallback = OnFinalSpeechResult;
        TextSpeech.TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextSpeech.TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
        _watsonAssistant = FindObjectOfType<AssistantWithText>();//Action IBM Watson
        modelAnimation = GameObject.Find("MainCharacter");
        CheckPermission();
    }

    void CheckPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    #region Text to Speech
    public void StartSpeaking(string message)
    {
        TextSpeech.TextToSpeech.Instance.StartSpeak(message);
    }
    public void StopSpeaking()
    {
        TextSpeech.TextToSpeech.Instance.StopSpeak();
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking started");
        modelAnimation.GetComponent<Animator>().SetBool("IsTalking", true);//animation talking
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking stopped");
        modelAnimation.GetComponent<Animator>().SetBool("IsTalking", false);//animation stop talking, idle
    }

    #endregion

    #region Speech to Text
    public void StartListening()
    {
        TextSpeech.SpeechToText.Instance.StartRecording();
    }

    public void StopListening()
    {
        TextSpeech.SpeechToText.Instance.StopRecording();
    }

    void OnFinalSpeechResult(string result)
    {
        try
        {
            uiText.text = result;
            //Send the text to IBM Watson
            if (!string.IsNullOrEmpty(result))
            {
                /*Action to send the question*/
                if (_watsonAssistant != null)
                {
                    Runnable.Run(_watsonAssistant.ProcessChat(result));
                }
                else
                {
                    UnityEngine.Debug.LogError("AssistantInputController: _watsonAssistant is null");
                }
            }
            else
            {
                UnityEngine.Debug.Log("The text is empty");
            }
        }
        catch (System.Exception e)
        {

            UnityEngine.Debug.Log("Send questions speech: " + e);
        }
    }

    void OnPartialSpeechResult(string result)
    {
        uiText.text = result;
    }

    #endregion

    void Setup(string code)
    {
        TextSpeech.TextToSpeech.Instance.Setting(code, 1, 1);
        TextSpeech.SpeechToText.Instance.Setting(code);
    }
}
