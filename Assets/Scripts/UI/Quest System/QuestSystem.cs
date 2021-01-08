using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
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

    // Quest items stored by key and reference to GameObject
    private Dictionary<string, GameObject> questItems = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Initialize singleton
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void Update()
    {
        // If toggle key pressed, toggle active
        if (Input.GetKeyDown(toggleKey)) Active = !Active;
    }

    // Creates a quest with given key, title, and description
    public void CreateQuest(string key, string title, string description)
    {
        // If key already contained in quest items, return
        if (questItems.ContainsKey(key)) return;

        // Instantiate and initialize quest item at content
        GameObject questItem = Object.Instantiate(questItemPrefab, Vector3.zero, Quaternion.identity, contentTransform);
        questItem.GetComponent<QuestItem>().Initialize(title, description);

        // Add quest item to dictionary
        questItems.Add(key, questItem);
    }

    // Removes a quest by key
    public void ResolveQuest(string key)
    {
        // If key not in quest items, return
        if (!questItems.ContainsKey(key)) return;

        // Destroy corresponding GameObject and remove from dictionary
        Destroy(questItems[key]);
        questItems.Remove(key);
    }
}
