# MMO (questSystem branch)

This branch implements `QuestSystem.cs`, `QuestItem.cs`, and their respective prefabs.

These scripts and prefabs can be found in `Assets/Scripts/UI/Quest System` and `Assets/Prefabs/UI/Quest System`.

## Controls

The Quest System canvas is by default toggled by pressing Q. This can be changed via `toggleKey` in `QuestSystem.cs`.

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
// Creates "Secret Keys" quest
QuestSystem.instance.CreateQuest("keys-1", "Secret Keys", "Can you find the secret keys?");

// Resolves "Secret Keys" quest
QuestSystem.instance.ResolveQuest("keys-1");
```
