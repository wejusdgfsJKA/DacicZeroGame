# scripts added in `ioanbranch`

## 📁 data
- **DialogSequenceSO.cs** - one dialog conversation. (ScriptableObject)
- **DialogNode.cs** - one dialog step within a dialog sequence.
- **DialogOption.cs** - one response option selectable during a dialog.
- **MissionDataSO.cs** - mission configuration and rewards. (ScriptableObject)
- **WeaponDataSO.cs** - base stats and visuals for a weapon. (ScriptableObject)
- **WeaponUpgradeSO.cs** - weapon upgrade requirements. (ScriptableObject)

## 📁 global
- **IsExternalInit.cs** - compiler polyfill allowing c# 9 `init` properties.
- **MissionParameters.cs** - static container passing data between scenes.
- **PlayerLoadout.cs** - static manager tracking weapon inventory.
- **PlayerResources.cs** - static class managing global player scrap.

## 📁 npc
- **NPCController.cs** - individual npc interaction and dialog triggers.
- **NPCDirector.cs** - scene manager for global npc visibility.

## 📁 player controller
- **PrepPlayerController.cs** - 2d prep phase movement and interaction.

## 📁 ui
- **ActionButtonUI.cs** - generic button firing unity/event bus actions.
- **IClosable.cs** - interface for closing ui panels.
- **UIManager.cs** - global manager tracking open menus and escape key stack.
- **UniversalCloseButtonUI.cs** - generic button closing the nearest parent panel.
- **DialogUIController.cs** - centralized visual dialog system controller.
- **DialogOptionUI.cs** - visual option button inside the dialog panel.
- **InteractableMapTable.cs** - environment object that opens the mission map.
- **MapContextMenuUI.cs** - context menu overlay for map options.
- **MissionLaunchOverlay.cs** - confirmation overlay before launching a mission.
- **MissionNode.cs** - selectable mission location on the map ui.
- **MissionSelectorUI.cs** - orchestrates the map canvas and overlays.
- **UpgradeMenuUI.cs** - weapon upgrade screen controller.
- **WeaponComparisonUI.cs** - dual-panel view comparing weapon stats.
- **WeaponListButtonUI.cs** - clickable button representing an available upgrade.

## to-updates
- **c# 10+ update**: convert `DialogNode` and `DialogOption` to structs.