using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using IBM.Cloud.SDK.Utilities;
using static System.Net.Mime.MediaTypeNames;

public class ViewModel : MonoBehaviour
{
    public Button btnMic;
    public Button btnSend;
    public TMP_InputField inpQuestion;
    private string textInp = "";
    private AssistantWithText _watsonAssistant;
    public UnityEngine.UI.Text warningRedord;
    public TMP_Text textListening;

    private VoiceController voiceController;

    void Awake()
    {
        _watsonAssistant = FindObjectOfType<AssistantWithText>();//Action IBM Watson
        voiceController = FindObjectOfType<VoiceController>();
    }

    void Start()
    {

        /*Add listeners*/
        btnSend.onClick.AddListener(BtnSenClick);
        inpQuestion.onValueChanged.AddListener(Input_Changed);

        /*Hide buttons*/
        ShowBtnMic(true);
        ShowBtnSend(false);
        ShowWarningRecording(false);
    }

    private void ShowBtnMic(bool active)
    {
        btnMic.gameObject.SetActive(active);
    }

    private void ShowBtnSend(bool active)
    {
        btnSend.gameObject.SetActive(active);
    }


    private void ShowWarningRecording(bool active)
    {
        warningRedord.gameObject.SetActive(active);
    }

    private void BtnMicClick()
    {

        textListening.text = "";
        voiceController.StartListening();
        /*Notify recording*/
        if (warningRedord != null)
        {
            ShowWarningRecording(true);
        }
        else { UnityEngine.Debug.Log("Error warning null"); }

    }

    private void InpSpeechOnChanged(string newText)
    {
        try
        {
            UnityEngine.Debug.Log("Change input: " + newText);
            if (!string.IsNullOrEmpty(newText))
            {
                /*Action to send the question*/
                if (_watsonAssistant != null)
                {
                    Runnable.Run(_watsonAssistant.ProcessChat(newText));
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
            UnityEngine.Debug.Log("Error Speech on change: " + e);
        }

    }


    private void BtnSenClick()

    {
        try
        {
            UnityEngine.Debug.Log("Texto enviado: " + textInp);

            /*Send question*/
            if (_watsonAssistant != null)
            {
                Runnable.Run(_watsonAssistant.ProcessChat(textInp));
            }
            else
            {
                UnityEngine.Debug.LogError("AssistantInputController: _watsonAssistant is null");
            }
            inpQuestion.text = "";
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("Send question: " + e);
        }

    }

    private void Input_Changed(string text)
    {
        ShowBtnMic(false);
        ShowBtnSend(true);

        if (!string.IsNullOrEmpty(text))
        {
            textInp = text;
        }
        else
        {
            UnityEngine.Debug.Log("The text is empty");
            ShowBtnMic(true);
            ShowBtnSend(false);
        }
    }

}
