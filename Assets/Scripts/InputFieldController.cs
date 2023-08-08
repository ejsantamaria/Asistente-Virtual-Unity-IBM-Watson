using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class InputFieldController : MonoBehaviour
{
    private InputField _textInput;
    
    // Start is called before the first frame update
    void Start()
    {
        _textInput = gameObject.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if this gameObject is selected and if the Return key is pressed
        if (EventSystem.current.currentSelectedGameObject == gameObject && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage(_textInput.text);
            _textInput.text = string.Empty;

            // Keep the current gameObject selected
            _textInput.ActivateInputField();
            _textInput.Select();
        }
    }

    protected abstract void SendMessage(string text);
}
