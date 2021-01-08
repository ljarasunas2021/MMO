using Mirror;
using UnityEngine;

public class QuestSystem : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Transform contentTransform = null;
    [SerializeField] private GameObject questItemPrefab = null;

    // Key which toggles quest system visibility
    private const KeyCode toggleKey = KeyCode.Q;

    // QuestSystem singleton
    public static QuestSystem instance;

    // Active property which automatically updates canvas visibility
    private bool _active = false;
    public bool Active
    {
        get { return _active; }
        set
        {
            // Return if value same, otherwise set value
            if (value == _active) return;
            _active = value;

            // Set canvas 
            canvas.enabled = Active;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // Return if not local player
        if (!isLocalPlayer) return;

        // If toggle key pressed, toggle active
        if (Input.GetKeyDown(toggleKey)) Active = !Active;
    }

    // Creates a quest with given key, title, and description
    public void CreateQuest(string key, string title, string description)
    {
        // Instantiate quest item at content
        Object.Instantiate(questItemPrefab, Vector3.zero, Quaternion.identity, contentTransform);
    }
}
