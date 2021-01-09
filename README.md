# MMO Contributor's Guide

## Downloading the project

Navigate to your desired folder and clone the project:

`git clone https://github.com/ljarasunas2021/MMO.git`

If not working on main, checkout a new branch:

`git checkout -b [branch name here]`

## Scripting

### Quest System

#### Controls

The quest canvas is by default toggled with Q. This can be changed via `toggleKey` in `QuestSystem.cs`.

#### Scripting

`QuestSystem.cs` creates an `instance` singleton.

```cs
// Active property which automatically updates canvas visibility
public bool Active { get; set; }

// Creates a quest with given key, title, and description
public void CreateQuest(string key, string title, string description);

// Removes a quest by key
public void ResolveQuest(string key);
```

#### Usage Examples

```cs
// Activate quest system
QuestSystem.instance.Active = true;

// Create "Secret Door" quest
QuestSystem.instance.CreateQuest("door-1", "Secret Door", "Can you find the secret door?");

// Resolve "Secret Door" quest
QuestSystem.instance.ResolveQuest("door-1");
