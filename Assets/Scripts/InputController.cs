using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputController : MonoBehaviour
{
    private Text textComponent;
    private string previousText;
    private VoiceController _voiceController;

    private void Start()
    {
        // Obtener el componente Text del objeto
        textComponent = GetComponent<Text>();
        _voiceController = FindObjectOfType<VoiceController>();

        // Guardar el texto inicial como referencia
        previousText = textComponent.text;
    }

    private void Update()
    {
        // Comprobar si el texto ha cambiado
        if (textComponent.text != previousText)
        {
            // Si ha cambiado, invocar el evento TextChanged
            OnTextChanged();

            // Actualizar el texto anterior con el nuevo texto
            previousText = textComponent.text;

            //Debug.Log("Texto cambiado");
            _voiceController.StartSpeaking(textComponent.text);
        }
    }

    // Definir un evento que otros scripts pueden suscribirse
    public delegate void TextChangedHandler(string newText);
    public event TextChangedHandler TextChanged;

    // Método para invocar el evento TextChanged
    protected virtual void OnTextChanged()
    {
        // Comprobar si hay suscriptores antes de invocar el evento
        if (TextChanged != null)
        {
            TextChanged(textComponent.text);
        }
    }
}
