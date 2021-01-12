# MMO Contributor's Guide

*What is the game?*
- It’s an MMO based around characters who work in a hotel which houses various cryptids and monsters such as the Loch Ness Monster and Mothman.  It consists of quests, puzzles, and dungeons that the player must fight through in order to successfully progress in the game.  Think Club Penguin with a semilinear storyline and just a skosh more violence. The goal is to be promoted, through honest or dishonest means, to the head of the hotel.  The twist is that the game tracks when you make dialogue or other decisions that violate the social contract in some way (stealing, just being blatantly rude, like pseudo-nepotism? idk), and the more of those choices you make, the more health the final boss, the wendigo, has when you fight him in the last level.

*What does the development process look like?*
- As a class, we do periodic sprints, but at least this year different teams have been going at different speeds and thus at different stages of production (for example, the dialogue team is ahead of the location/systems design team, which is ahead of the assets team).  Therefore, the sprints haven’t really resulted in testable prototypes.  Since you’ll have a more extensive asset base than we had, you’ll want to test and revise rudimentary prototypes as soon and as often as possible.  We suggest putting together and testing the tutorial (redcap encounter).


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
- Toggle quests: Q

All controls should be registered by the Input Manager which can be found at `Edit > Project Settings > Input Manager`.

## Namespaces

Your scripts should be enclosed under the `MMO` namespace.

Namespaces are used for organization and prevention of scope conflicts.

- MMO
  - Actions
  - GOAP
  - Player
  - UI
    - Inventory
    - Map
    - QuestSystem

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

### Quest System (MMO.UI.QuestSystem)

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

### Map (MMO.UI.Map)

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

### UI Manager (MMO)

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

