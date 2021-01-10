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

## Scripting

### Inventory Manager

`InventoryManager.cs` creates an `instance` singleton.

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

### Health Bar

`HealthBar.cs` creates an `instance` singleton.

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

### Quest System

`QuestSystem.cs` creates an `instance` singleton.

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
QuestSystem.instance.CreateQuest("door-1", 0.6f);

// Resolve "Secret Door" quest
QuestSystem.instance.ResolveQuest("door-1");
```

### Compass

`Compass.cs` creates an `instance` singleton.

Methods:

```cs
// Adds a waypoint to the waypoint list and displays it accordingly
public void AddWaypoint(Waypoint waypoint, MapMarker mapMarker);

// Removes a waypoint and destroys the appropriate GameObjects
public void RemoveWaypoint(MapMarker mapMarker);
```
