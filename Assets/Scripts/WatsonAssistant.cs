using System;
using System.Collections;
using System.Diagnostics;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public abstract class WatsonAssistant : MonoBehaviour
{
    [SerializeField] private string apiKey;
    [SerializeField] private string serviceUrl;
    [SerializeField] private string assistantId;
    [SerializeField] private string serviceVersionDate = "2020-04-13";

    private AssistantService _assistantService;
    private IamAuthenticator _assistantAuthenticator;
    private EProcessingStatus _assistantStatus;
    private bool _createSessionTested = false;
    private string _sessionId;
    public UnityEngine.UI.Image targetImage;
    private string imageUrl = "";


    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateAuthenticateServices());
        _assistantStatus = EProcessingStatus.Idle;
    }

    private IEnumerator CreateAuthenticateServices()
    {
        _assistantAuthenticator = new IamAuthenticator(apiKey);

        while (!_assistantAuthenticator.CanAuthenticate())
        {
            yield return null;
        }

        _assistantService = new AssistantService(serviceVersionDate, _assistantAuthenticator);

        if (!string.IsNullOrEmpty(serviceUrl))
        {
            _assistantService.SetServiceUrl(serviceUrl);
        }
        else
        {
            UnityEngine.Debug.LogError("Assistant Service URL is null or empty");
        }

        _assistantService.CreateSession(OnCreateSession, assistantId);

        while (!_createSessionTested)
        {
            yield return null;
        }
    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("SimpleBot.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        _sessionId = response.Result.SessionId;
        _createSessionTested = true;
        Runnable.Run(ProcessChat(null, true));
    }

    public IEnumerator ProcessChat(string chatInput, bool welcome = false)
    {
        //UnityEngine.Debug.Log($"Processchat: {chatInput}");
        // Set status to show that the chat input is being processed.
        _assistantStatus = EProcessingStatus.Processing;

        if ((String.IsNullOrEmpty(chatInput) && !welcome) || _assistantService == null)
        {
            yield return null;
        }

        MessageInput messageInput = null;
        if (!welcome)
        {
            messageInput = new MessageInput()
            {
                Text = chatInput,
                Options = new MessageInputOptions()
                {
                    ReturnContext = true
                }
            };
        }

        MessageResponse messageResponse = null;
        _assistantService.Message(
            callback: OnMessage,
            assistantId: assistantId,
            sessionId: _sessionId,
            input: messageInput
        );

        while (messageResponse == null)
        {
            yield return null;
        }
    }

    private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {
        //UnityEngine.Debug.Log($"response = {response.Result}");

        string textResponse;
        bool statusImage = false;
        imageUrl = "";
        if (response.Result == null || response.Result.Output == null ||
            response.Result.Output.Generic == null || response.Result.Output.Generic.Count < 1)
        {
            textResponse = "No he entendido su pregunta, por favor intente de nuevo.";
        }
        else
        {
            textResponse = response.Result.Output.Generic[0].Text.ToString();
            if (response.Result.Output.Generic.Count > 1)
            {
                foreach (var genericResponse in response.Result.Output.Generic)
                {
                    if (genericResponse.ResponseType == "option")
                    {
                        foreach (var optionResponse in genericResponse.Options)
                        {
                            textResponse += optionResponse.Value.Input.Text.ToString() + ", ";
                        }
                    }
                    if (genericResponse.ResponseType == "image")
                    {
                        statusImage = true;//show image
                        //UnityEngine.Debug.Log($"Respuesta completa: {response.Result.Output.Generic[1].Source.ToString()}");
                        imageUrl = genericResponse.Source.ToString();
                    }
                }

            }

        }

        SendResponse(textResponse, imageUrl, statusImage);

        _assistantStatus = EProcessingStatus.Processed;

    }


    protected abstract void SendResponse(string text, string urlImage, bool statusImg);


}
