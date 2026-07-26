# Dialog System & Interaction Updates

## Summary
We successfully implemented a fully functional branching dialog system with scriptable objects, built an interactive Map Table, created an NPC that gives dialog, and wrote a top-down 2D player controller that uses an interaction radius to interact with the world via Unity's EventBus.

## Created Files

**Data & Architecture**
- `Assets/_Project/Scripts/Data/Dialog/DialogNode.cs`
  *Represents a single block of dialog text and a list of branching options.*
- `Assets/_Project/Scripts/Data/Dialog/DialogOption.cs`
  *Represents a player choice, tracking the text, the next node index to jump to, and an optional Action string (e.g., "OpenMap").*
- `Assets/_Project/Scripts/Data/Dialog/DialogSequenceSO.cs`
  *A ScriptableObject that stores the full conversation tree and the NPC's speaking Sprite.*

**UI Controllers**
- `Assets/_Project/Scripts/UI/Dialog/DialogUIController.cs`
  *The core dialog logic. It listens for `StartDialogEvent`, types text out character-by-character, spawns option buttons, routes actions (like opening the map), and freezes/unfreezes the player.*
- `Assets/_Project/Scripts/UI/Dialog/DialogOptionUI.cs`
  *Attached to the prefabs for the dialog choice buttons. Passes the player's choice back to the controller when clicked.*
- `Assets/_Project/Scripts/UI/MissionSelector/MissionSelectorUI.cs`
  *Listens for the "OpenMap" action from the dialog system to display the Map Canvas.*

**Gameplay & World Objects**
- `Assets/_Project/Scripts/NPC/NPCController.cs`
  *Attached to physical NPCs. Listens for `InteractionEvent`s from the player and raises a `StartDialogEvent` with its specific ScriptableObject sequence.*
- `Assets/_Project/Scripts/UI/MissionSelector/InteractableMapTable.cs`
  *An interactive table that skips dialog and directly opens the map when interacted with.*
- `Assets/_Project/Scripts/PlayerController/Controls/PrepPlayerController.cs`
  *A simplified top-down 2D player controller. Uses `Physics.OverlapSphere` to detect `Interactable` objects in range when the player presses 'E'. Also ignores input when frozen by a dialog.*

## Modified Files

- `Assets/_Project/Scripts/PlayerController/Interaction/Interactable.cs`
  *Removed a `Debug.LogError` inside `OnDisable()`. Because Unity clears the EventBus when Play Mode ends before destroying objects, the error was harmless but spammed the console.*

## Scenes & Prefabs (Untracked in scripts, created in Unity)
- `Assets/_Project/Scenes/PrepScene.unity` (The main testing scene for the UI and Player)
- `Assets/_Project/Prefabs/DialogOptionButton.prefab` (The button spawned dynamically for dialog choices)
