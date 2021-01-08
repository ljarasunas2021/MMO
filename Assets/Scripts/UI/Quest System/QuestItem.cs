using TMPro;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI titleText = null;
    [SerializeField] private TextMeshProUGUI descriptionText = null;

    // Initialize quest item with title and description
    public void Initialize(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
}
