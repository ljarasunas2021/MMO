# MMO (questSystem branch)

This branch implements `QuestSystem.cs`, `QuestItem.cs`, and their respective prefabs.

These assets can be found in `Assets/Scripts/UI/Quest System` and `Assets/Prefabs/UI/Quest System`.

## Controls

The quest canvas is by default toggled with Q. This can be changed via `toggleKey` in `QuestSystem.cs`.

## Scripting

`QuestItem.cs` creates an `instance` singleton.

```cs
// Creates a quest with given key, title, and description
public void CreateQuest(string key, string title, string description);

// Removes a quest by key
public void ResolveQuest(string key);
```

## Usage Examples

```cs
// Creates "Secret Door" quest
QuestSystem.instance.CreateQuest("door-1", "Secret Door", "Can you find the secret door?");

// Resolves "Secret Door" quest
QuestSystem.instance.ResolveQuest("door-1");
```
