using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
public class AssistantWithText : WatsonAssistant
{
    //private TextToSpeech _textToSpeech;
    private VoiceController _voiceController;
    public UnityEngine.UI.Text targetResponse;

    protected override void SendResponse(string text, string urlImage, bool statusImg)
    {
        targetResponse.text = text;
        StartCoroutine(LoadImage(urlImage));
        targetImage.enabled = statusImg;
    }

    /*
        Load image from url
    */

    IEnumerator LoadImage(string urlImage)
    {
        if (string.IsNullOrEmpty(urlImage))
        {
            yield return null;
        }
        else
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(urlImage);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.LogError("Error download image: " + www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                targetImage.sprite = sprite;
            }
        }

    }
}
