# MMO Contributor's Guide

*What is the game?*
- It’s an MMO based around characters who work in a hotel which houses various cryptids and monsters such as the Loch Ness Monster and Mothman.  It consists of quests, puzzles, and dungeons that the player must fight through in order to successfully progress in the game.  Think Club Penguin with a semilinear storyline and just a skosh more violence. The goal is to be promoted, through honest or dishonest means, to the head of the hotel.  The twist is that the game tracks when you make dialogue or other decisions that violate the social contract in some way (stealing, just being blatantly rude, like pseudo-nepotism? idk), and the more of those choices you make, the more health the final boss, the wendigo, has when you fight him in the last level.

*What does the development process look like?*
- As a class, we do periodic sprints, but at least this year different teams have been going at different speeds and thus at different stages of production (for example, the dialogue team is ahead of the location/systems design team, which is ahead of the assets team).  Therefore, the sprints haven’t really resulted in testable prototypes.  Since you’ll have a more extensive asset base than we had, you’ll want to test and revise rudimentary prototypes as soon and as often as possible.  We suggest putting together and testing the prototpye (`Scenes/lobby` which has all fundamental player gameplay features and a working npc and enemy ai) as well as the tutorial (`Scenes/lobby1` which has example dialogue and quest systems).


## Cloning the project

Navigate to your desired folder and run `git clone`:

`git clone https://github.com/ljarasunas2021/MMO.git`

If not working on main, `git checkout` a new branch:

`git checkout -b [branch name here]`

The project uses Unity version 2019.4.12f1.

## Controls

- Movement: WASD/arrow keys
- Jump: space
- Sprint: left shift
- Pick up: E
- Skip dialogue: return
- Toggle inventory: tab
- Toggle map: M
- Draw markers on map and compass: left-click on the map
- Remove markers on map and compass: left-click on the marker (which is on the map)
- Move around inventory items: drag-and-drop
- Toggle quests: Q
- Pause (client-side): P
- Interact with target (i.e. talk to npc): left-click on target
- Attack with left inventory item: left-click
- Attack with right inventory item: right-click

All controls should be registered by the Input Manager which can be found at `Edit > Project Settings > Input Manager`.

## Networking

This project uses the Mirror Networking Framework. This is a third party replacement for UNet, which Unity has stopped production of for a few years now. Mirror will come in the project, but it must be updated frequently (you can usually update it through the asset store). Be comfortable with the concepts of Mirror, since the majority of the scripts do use networking in some shape or form. The mmo will be run on a server, and any Chadwick student will be able to join the server. That being said, we do not plan to use that large of a server, and therefore certain aspects of the game, such as player movement, are client authoritative rather than server authoritative. 

## Namespaces

Your scripts should be enclosed under the `MMO` namespace.

Namespaces are used for organization and prevention of scope conflicts.

- MMO
  - Actions
  - Animation
  - GOAP
  - NPC
  - Player
  - UI
    - Inventory
    - Maps
    - Quests

You may encounter the error message `The type or namespace [x] could not be found.`

If this happens, you most likely have missed an import statement.

For example, if you require an `Inventory` script, make sure to include:

```cs
using MMO.UI.Inventory;
```

At the top of your script.

## Scripting

### Inventory Manager (MMO.UI.Inventory)

Manages the inventory.

`InventoryManager.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Methods and classes:

```cs
// Toggles the inventory on and off
public void ChangeEnabled();

// Equips given hotbar slot
public void EquipSlot(int slot);

// An inventory item by index and its icon
public InventoryItemAndIcon(int itemIndex, Sprite icon) {}
```

Usage examples:

```cs
// Toggle inventory
InventoryManager.instance.ChangeEnabled();

// Equips hotbar slot 0
InventoryManager.instance.EquipSlot(0);

// Create an empty inventory item
InventoryItemAndIcon emptyItem = new InventoryItemAndIcon(-1, null);
```

### Health Bar (MMO.Player)

Manages the health bar UI.

`HealthBar.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Methods:

```cs
// Sets the health of the health bar to given value
public void SetHealth(float health);
```

Usage examples:

```cs
// Set player health to 0
HealthBar.instance.SetHealth(0);
```

### Quest System (MMO.UI.Quests)

Manages player quests.

`QuestSystem.cs` inherits from `NetworkBehaviour` and creates an `instance` singleton.

Properties and methods:

```cs
// Active property which automatically updates canvas visibility
public bool Active { get; set; }

// Creates a quest with given key, title, and description
public void CreateQuest(string key, string title, string description, bool progressBar);

// Sets quest progress by key
public void SetQuestProgress(string key, float val);

// Removes a quest by key
public void ResolveQuest(string key);
```

Usage examples:

```cs
// Activate quest system
QuestSystem.instance.Active = true;

// Create "Secret Door" progress quest
QuestSystem.instance.CreateQuest("door-1", "Secret Door", "Can you find the secret door?", true);

// Set "Secret Door" quest progress to 60%
QuestSystem.instance.SetQuestProgress("door-1", 0.6f);

// Resolve "Secret Door" quest
QuestSystem.instance.ResolveQuest("door-1");
```

### Compass (MMO.UI)

Manages the compass UI.

`Compass.cs` inherits from `NetworkBehaviour` and creates an `instance` singleton.

Methods:

```cs
// Adds a waypoint to the waypoint list and displays it accordingly
public void AddWaypoint(Waypoint waypoint, MapMarker mapMarker);

// Removes a waypoint and destroys the appropriate GameObjects
public void RemoveWaypoint(MapMarker mapMarker);
```

### Map (MMO.UI.Maps)

Handles the UI with the map.

`Map.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Methods:

```cs
// Enables map and refreshes player's position on marker
public void Enable();

// Disables map and destroys player's map marker
public void Disable();
```

### Input Handler (MMO.Player)

Used to manage all of the player's input.

`InputHandler.cs` inherits from `NetworkBehaviour`.

Usage examples:

```cs
// Tests for UI input
private void TestUI()
{
    ...
    if (Input.GetButtonDown("ToggleMap")) uIScript.ToggleMap();
    if (Input.GetButtonDown("SkipDialogue")) uIScript.CheckForSkipDialogue();
    if (Input.GetButtonDown("ToggleQuests")) QuestSystem.instance.Active = !QuestSystem.instance.Active;
}
```

All inputs should be taken through `InputHandler.cs`.

### Players Controller (MMO)

Stores every player in a singleton.

`PlayersController.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Variables:

```cs
// List of all players
public List<GameObject> players;
```

Usage examples:

```cs
// Returns a list of all players
private List<GameObject> GetPlayers()
{
    return PlayersController.instance.players;
}
```

### Target (MMO)

Ovveridable class used to allow the player to interact with targets. 

`Target.cs` inherits from `MonoBehavior`.

Method:

```cs
// Called when player clicks on target
public virtual void Interact() {
    Debug.Log("Override this");
}
```

Example:

```cs
/// <summary> NPC interact with the player </summary>
public override void Interact()
{
    if (IsPlayerCloseEnough())
    {
        if (UIManager.instance.canMove)
        {
            interacting = true;

            StartCoroutine(RotateNPC());

            UIScript.audioSource = GetComponent<AudioSource>();
            UIScript.ToggleDialogue(dialogue[currentDialogIndex], this);

            if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
        }
    }
}
```

### Outline Object (MMO)

Primarily used on target objects to highlight an object when the mouse hovers over the object (used with Outline Script).

`OutlineObject.cs` inherits from `MonoBehaviour`.

Methods:

```cs
/// <summary> Is the player close enough for it to be outlined </summary>
/// <returns> whether the player is close enough for the gameobject to be outlined </returns>
private bool IsPlayerCloseEnough()
{
    float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
    return (playerDist < radius);
}

/// <summary> When the mouse hovers over a gameobject, outline the object </summary>
void OnMouseOver() {
    if (IsPlayerCloseEnough()) {
        outline.enabled = true;
    } else {
        outline.enabled = false;
    }
}

/// <summary> Disable the outline when the mouse exits the gameobject </summary>
void OnMouseExit() {
    outline.enabled = false;
}
```

### UI Manager (MMO.UI)

Manages the UI for the entire game.

`UIManager.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Methods:

```cs
// Toggles dialogue audio on or off
public void ToggleDialogue(Dialogue dialogue, NPCInteract nPC);

// Toggles the pause menu on or off
public void TogglePauseMenu();

// Toggles the map on or off
public void ToggleMap();

// Locks or unlocks the cursor
public void LockCursor(bool locked);

// Turns the Inventory on or off
public void ToggleInventory();

// Checks whether a dialogue can be skipped
public void CheckForSkipDialogue();
```

### Layer Mask Controller (MMO)

Static class which contains references to all layers.

`LayerMaskController.cs` is static.

Variables:

```cs
public static LayerMask environment = 10;
public static LayerMask player = 9;
public static LayerMask item = 11;
public static LayerMask playerNonRagdoll = 13;
```

Usage examples:

```cs
// Gets the player LayerMask
private LayerMask playerLayerMask = LayerMaskController.player;
```

### Item Prefabs Controller (MMO)

Array of item prefabs for commands to pull from.

`ItemPrefabsController.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Variables:

```cs
// Array of item prefabs
public GameObject[] itemPrefabs;
```

Usage examples:

```cs
// Returns item prefab for item with given index
private GameObject ItemPrefabAtIndex(int index)
{
    return ItemPrefabsController.instance.itemPrefabs[index];
}
```

### Effects Prefabs Controller (MMO)

Singleton which stores every effect prefab.

`EffectsPrefabsController.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Variables:

```cs
// Array of effects
public GameObject[] effectPrefabs;
```

Usage examples:

```cs
// Returns effect prefab for effect with given index
private GameObject EffectPrefabAtIndex(int index)
{
    return EffectsPrefabsController.instance.effectPrefabs[index];
}
```

### Audio Prefabs Controller (MMO)

Singleton which holds array of all in-game audio files.

`AudioPrefabsController.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Variables:

```cs
// Array of playable audio clips
public AudioClip[] audioClipPrefabs;
```

Usage examples:

```cs
// Returns audio clip with given index
private AudioClip AudioPrefabAtIndex(int index)
{
    return AudioPrefabsController.instance.audioClipPrefabs[index];
}
```

### Camera Controller (MMO)

Singleton which stores every camera.

`CameraController.cs` inherits from `MonoBehaviour` and creates an `instance` singleton.

Variables:

```cs
public GameObject cinematicCam;
public GameObject closeUpCam;
public GameObject lockedCam;
```

### NPC Interact (MMO.NPC)

Put on an NPC enabling it to interact with the player. 

`NPCInteract` inherits from `Target`.

Methods:

```cs
/// <summary> Npc interact with the player </summary>
public override void Interact()
{
    if (IsPlayerCloseEnough())
    {
        if (UIManager.instance.canMove)
        {
            interacting = true;

            StartCoroutine(RotateNPC());

            UIScript.audioSource = GetComponent<AudioSource>();
            UIScript.ToggleDialogue(dialogue[currentDialogIndex], this);

            if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
        }
    }
}
```

### NPC Dialogue (MMO.NPC)

Holds variables for an npc's dialogue. View `Scenes/Lobby1` to get a good example of how an npc's dialogue can be setup. It is essentially a series of npc and player dialogues (player dialogue is discussed in the next section).

`NPCDialogue.cs` inherits from `MonoBehaviour`.

Variables:

```cs
// the words that will show up in the NPC's textbox when it speaks
public string text;
// the audio that plays when the NPC speaks
public AudioClip audioClip;
```

### Player Dialogue (MMO.Player)

Holds variables for a player's dialogue.

`PlayerDialogue.cs` inherits from `MonoBehaviour`.

Variables:

```cs
// Whether the player should have different dialogue options
public bool options;

// If nno options, this text will be displayed for player
public string text;

// Amount of time the text is on the screen
public float time;
```

### Action (MMO)

Overridable class that represents an action that can occur inbetween dialogue.

`Action.cs` inherits from `MonoBehaviour`.

Methods:

```cs
/// <summary> Execute the action, such that when it finishes, the coroutine finishes as well and the dialogue can continue </summary>
/// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
public virtual IEnumerator Execute() {
    Debug.LogWarning("Overide this execute function");
    yield break;
}
```

### TransformDeepChildExtension (MMO)

Finds a child of a Transform with a certain name.

`TransformDeepChildExtension.cs` is static.

Methods:

```cs
// Finds a child of a Transform with a certain name
public static Transform FindDeepChild(this Transform parent, string name);
```

Usage examples:

```cs
// Finds Transform of "elem" object within self
Transform elemTransform = transform.FindDeepChild("elem");
```

### Goap Agent (SwordGC.AI.Goap)

GOAP stands for Goal Oriented Action Planning. To learn about GOAP AI, visit https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793, but essentially GOAP is an AI system used to easily create complex AI behaviors through determining the most important goal and the easiest sequence of actions to reach that goal. Again, `Scenes/Lobby` has a good version of a AI that utilizes GOAP through its EnemyAI1 script that derives from GoapAgent. 

`GoapAgent` inherits from `MonoBehavior`.

### Goap Goal (SwordGC.AI.Goap)

A Goal Goal is a goal that an AI tries to accomplish. Again, there is an example of such goal attached to the AI in `Scenes/Lobby` called `KillPlayerGoal`.

### Goap Action (SwordGC.AI.Goap)

A Goal Action is an action that an AI can complete in order to get closer to finishing/finish a goap goal. Again, there is an example of such goal attached to the AI in `Scenes/Lobby` called `KillAction`.
