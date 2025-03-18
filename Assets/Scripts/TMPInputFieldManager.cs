using UnityEngine;
using TMPro;

public class TMPInputFieldManager : MonoBehaviour
{
    private TMP_InputField inputField;
    private TMP_Text textComponent;

    [SerializeField] private string placeholderChar = ".";
    [SerializeField] private Color placeholderColor = new Color(1, 1, 1, 0.1f);
    [SerializeField] private float placeholderSize = 1f;

    [SerializeField] private float normalSize = 14f;
    [SerializeField] private Color normalColor = Color.black;

    private bool isPlaceholderActive = true; // Flag to track if placeholder is active

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        textComponent = inputField.textComponent;

        // Configure the input field
        inputField.characterLimit = 1; // Limit to one character
        inputField.text = placeholderChar;
        textComponent.fontSize = placeholderSize;
        textComponent.color = placeholderColor;

        inputField.onValueChanged.AddListener(OnTextChanged);
        inputField.onSelect.AddListener(OnSelect);
    }

    void OnTextChanged(string newText)
    {
        if (isPlaceholderActive && newText.Length > 0)
        {
            // When the user types a character, remove the placeholder and apply normal settings
            inputField.text = newText.Substring(newText.Length - 1, 1); // Keep only the last entered character
            textComponent.fontSize = normalSize;
            textComponent.color = normalColor;
            isPlaceholderActive = false;
        }
    }

    void OnSelect(string _)
    {
        if (isPlaceholderActive)
        {
            inputField.text = ""; // Clear the placeholder when selecting the field
        }
    }
}
