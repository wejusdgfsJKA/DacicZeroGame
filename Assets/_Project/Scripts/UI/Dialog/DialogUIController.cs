using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EventBus;
using DacicZero.Data.Dialog;
using System;
using System.Collections.Generic;

namespace DacicZero.UI.Dialog {

    #region Events
#warning when upgrading to c# 10, uncomment the record struct below and remove the standard struct to reduce boilerplate.
    // public readonly record struct DialogActionFiredEvent(string ActionID, string ActionParams = "") : IEvent;
    
    public readonly struct DialogActionFiredEvent : IEvent {
        public string ActionID { get; }
        public string ActionParams { get; }
        public DialogActionFiredEvent(string actionID, string actionParams = "") {
            ActionID = actionID;
            ActionParams = actionParams;
        }
    }

    public readonly struct EndDialogEvent : IEvent { }
    #endregion

    public class DialogUIController : MonoBehaviour {

        #region Variables & Properties
        public enum ExtendDirection { Up, Down, Both }

        [Header("Input")]
        [SerializeField] private PlayerController.InputReader _inputReader;

        [Header("Text & Images")]
        [SerializeField] private TextMeshProUGUI _dialogText;
        [SerializeField] private Image img_LeftPortrait;
        [SerializeField] private Image img_RightPortrait;

        [Header("Options")]
        [SerializeField] private RectTransform _optionsContainer;
        [SerializeField] private GameObject _optionButtonPrefab;
        [SerializeField] private Image _containerBackground;
        [SerializeField] private ExtendDirection _extendDirection = ExtendDirection.Both;
        [SerializeField] private bool _showOptionsBackground = true;

        [Header("Visual Settings")]
        [SerializeField] private Color _activeSpeakerColor = Color.white;
        [SerializeField] private Color _inactiveSpeakerColor = Color.gray;

        private DialogSequenceSO _currentSequence;
        private int _currentNodeIndex;
        
        private readonly List<GameObject> _activeOptionButtons = new();
        private readonly List<GameObject> _optionButtonPool = new();
        
        private readonly Dictionary<string, Sprite> _portraitCache = new(StringComparer.OrdinalIgnoreCase);

        private int _frameOpened = -1;
        private Image _optionsContainerBg;
        private UnityEngine.UI.VerticalLayoutGroup _cachedLayoutGroup;
        private float _cachedButtonWidth = 130f;
        private float _cachedButtonHeight = 40f;
        
        private Transform _cachedRoomTransform;
        private bool _isInitialized;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            _isInitialized = true;
            EventBus<NPC.StartDialogEvent>.AddActions(0, OnStartDialog);

            if (_optionsContainer != null) {
                _optionsContainer.TryGetComponent(out _optionsContainerBg);
                _optionsContainer.TryGetComponent(out _cachedLayoutGroup);
            }

            if (_optionButtonPrefab != null && _optionButtonPrefab.TryGetComponent(out RectTransform prefabRT)) {
                _cachedButtonHeight = prefabRT.sizeDelta.y;
                _cachedButtonWidth = prefabRT.sizeDelta.x;
            }
                
            if (transform.parent != null) _cachedRoomTransform = transform.parent.Find("pnl_Room");
        }

        private void OnEnable() { 
            if (!_isInitialized) {
                Debug.LogError($"[DialogUI] awake did not run", gameObject);
            }
            if (_inputReader != null) _inputReader.Interact += OnAdvanceText; 
        }

        private void OnDisable() { 
            if (_inputReader != null) _inputReader.Interact -= OnAdvanceText; 
            if (_currentSequence != null) EndDialog();
        }

        private void OnDestroy() {  EventBus<NPC.StartDialogEvent>.RemoveActions(0, OnStartDialog);  }
        #endregion

        #region Sprite Management
        private Sprite GetSpriteForID(string id) {
            if (string.IsNullOrEmpty(id) || _cachedRoomTransform == null) return null;

            if (_portraitCache.TryGetValue(id, out Sprite cachedSprite)) 
                return cachedSprite;

            string cleanId = id.Replace("NPC_", "").Replace("img_", "");
            Sprite foundSprite = null;
            
            if (cleanId.Equals("Player", StringComparison.OrdinalIgnoreCase)) {
                Transform playerImg = _cachedRoomTransform.Find("img_Player");
                if (playerImg != null) foundSprite = GetSpriteFromGameObject(playerImg.gameObject);
            } else {
                Transform grpNPCs = _cachedRoomTransform.Find("grp_NPCs");
                if (grpNPCs != null) {
                    Transform npc = grpNPCs.Find($"NPC_{cleanId}") ?? grpNPCs.Find($"img_{cleanId}") ?? grpNPCs.Find(cleanId);
                    if (npc != null) foundSprite = GetSpriteFromGameObject(npc.gameObject);
                }
            }

            if (foundSprite == null) {
                GameObject fallbackObj = GameObject.Find($"NPC_{cleanId}") ?? GameObject.Find($"img_{cleanId}");
                if (fallbackObj != null) foundSprite = GetSpriteFromGameObject(fallbackObj);
            }

            if (foundSprite != null) {
                _portraitCache[id] = foundSprite;
            }

            return foundSprite;
        }

        private Sprite GetSpriteFromGameObject(GameObject go) {
            if (go == null) return null;
            if (go.TryGetComponent(out SpriteRenderer sr) && sr.sprite != null) return sr.sprite;
            if (go.TryGetComponent(out Image img) && img.sprite != null) return img.sprite;
            
            sr = go.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.sprite != null) return sr.sprite;
            
            img = go.GetComponentInChildren<Image>();
            if (img != null && img.sprite != null) return img.sprite;
            
            return null;
        }
        #endregion

        #region Dialog Logic
        private void OnStartDialog(NPC.StartDialogEvent evt) {
            if (evt.Sequence == null || evt.Sequence.Nodes.Count == 0) return;

            _currentSequence = evt.Sequence;
            _currentNodeIndex = 0;
            _frameOpened = Time.frameCount;

            ShowNode(_currentNodeIndex);
        }

        private void ShowNode(int index) {
            if (index < 0 || index >= _currentSequence.Nodes.Count) {
                EndDialog();
                return;
            }

            DialogNode node = _currentSequence.Nodes[index];
            if (_dialogText != null) _dialogText.text = node.DialogText;

            bool isLeftSpeaker = node.ActiveSpeaker is SpeakerSide.Left or SpeakerSide.Both;
            bool isRightSpeaker = node.ActiveSpeaker is SpeakerSide.Right or SpeakerSide.Both;

            if (img_LeftPortrait != null) {
                img_LeftPortrait.color = isLeftSpeaker ? _activeSpeakerColor : _inactiveSpeakerColor;
                Sprite finalLeftSprite = node.LeftSprite != null ? node.LeftSprite : GetSpriteForID(node.LeftID);
                if (finalLeftSprite != null) img_LeftPortrait.sprite = finalLeftSprite;
            }

            if (img_RightPortrait != null) {
                img_RightPortrait.color = isRightSpeaker ? _activeSpeakerColor : _inactiveSpeakerColor;
                Sprite finalRightSprite = node.RightSprite != null ? node.RightSprite : GetSpriteForID(node.RightID);
                if (finalRightSprite != null) img_RightPortrait.sprite = finalRightSprite;
            }

            foreach (var btn in _activeOptionButtons) {
                btn.SetActive(false);
                _optionButtonPool.Add(btn);
            }
            _activeOptionButtons.Clear();

            if (node.Options != null && node.Options.Count > 0) {
                _optionsContainer.gameObject.SetActive(true);
                if (_optionsContainerBg != null) _optionsContainerBg.enabled = _showOptionsBackground;

                int n = node.Options.Count;
        
                if (_containerBackground != null) {
                    _containerBackground.enabled = _showOptionsBackground;
                    
                    if (_extendDirection == ExtendDirection.Both)
                        _containerBackground.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    else if (_extendDirection == ExtendDirection.Up) 
                        _containerBackground.rectTransform.pivot = new Vector2(0.5f, 0f);
                    else if (_extendDirection == ExtendDirection.Down)
                        _containerBackground.rectTransform.pivot = new Vector2(0.5f, 1f);
                    
                    float activeSpacing = 0f;
                    float padX = 0f, padY = 0f;

                    if (_cachedLayoutGroup != null) {
                        activeSpacing = _cachedLayoutGroup.spacing;
                        padX = _cachedLayoutGroup.padding.left + _cachedLayoutGroup.padding.right;
                        padY = _cachedLayoutGroup.padding.top + _cachedLayoutGroup.padding.bottom;
                    }

                    float newWidth = _cachedButtonWidth + padX;
                    float newHeight = (_cachedButtonHeight * n) + (activeSpacing * (n - 1)) + padY;
                    _containerBackground.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
                }

                GameObject firstButton = null;

                for (int i = 0; i < n; i++) {
                    GameObject go;
                    if (_optionButtonPool.Count > 0) {
                        int lastIdx = _optionButtonPool.Count - 1;
                        go = _optionButtonPool[lastIdx];
                        _optionButtonPool.RemoveAt(lastIdx);
                        go.SetActive(true);
                    } else 
                        go = Instantiate(_optionButtonPrefab, _optionsContainer);
                    
                    if (go.TryGetComponent(out DialogOptionUI optUI))  optUI.Setup(node.Options[i], this);
                     else   Debug.LogError("[DialogUI] Option Button Prefab is missing the DialogOptionUI component!");
                    
                    go.transform.SetAsLastSibling();
                    _activeOptionButtons.Add(go);
                    firstButton ??= go; 
                }

                if (firstButton != null) EventSystem.current.SetSelectedGameObject(firstButton);
            } else 
                if (_optionsContainer != null) _optionsContainer.gameObject.SetActive(false);
        }

        public void OnAdvanceText() {
            if (_currentSequence == null || !gameObject.activeInHierarchy || _optionsContainer.gameObject.activeSelf || Time.frameCount <= _frameOpened + 1) return;
            ShowNode(++_currentNodeIndex);
        }

        public void OnOptionSelected(DialogOption option) {
            if (!string.IsNullOrEmpty(option.ActionEventID))
                EventBus<DialogActionFiredEvent>.Raise(0, new DialogActionFiredEvent(option.ActionEventID, option.ActionParams));

            if (option.JumpToNodeIndex >= 0) ShowNode(_currentNodeIndex = option.JumpToNodeIndex);
            else EndDialog();
        }

        public void EndDialog() {
            _currentSequence = null;
            EventBus<EndDialogEvent>.Raise(0, new EndDialogEvent());
        }
        #endregion
    }
}