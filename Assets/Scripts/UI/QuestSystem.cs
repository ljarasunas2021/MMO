using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas;

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
        // If toggle key pressed, toggle active
        if (Input.GetKeyDown(toggleKey)) Active = !Active;
    }
}
