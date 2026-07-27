using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using EventBus;
using DacicZero.Data.Dialog;
using DacicZero.NPC;
using System;
using System.Collections.Generic;

namespace DacicZero.UI.Dialog {
    #region Events
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


    /// <summary> main controller for visual dialog system </summary>
    public class DialogUIController : MonoBehaviour, IClosable {
        #region Variables & Properties
        /// <summary> checks if dialog canvas is active </summary>
        public bool IsOpen => _dialogCanvas != null && _dialogCanvas.activeSelf;

        [Header("UI Panels")]
        [SerializeField] private GameObject _dialogCanvas;
        [SerializeField] private GameObject _optionsPanel;

        [Header("Input")]
        [SerializeField] private PlayerController.InputReader _inputReader;

        [Header("Text & Images")]
        [SerializeField] private TextMeshProUGUI _dialogText;
        [SerializeField] private Image _npcImage;
        [SerializeField] private Image _playerImage;

        [Header("Options")]
        [SerializeField] private Transform _optionsContainer;
        [SerializeField] private GameObject _optionButtonPrefab;
        /// <summary> vertical gap between button edges </summary>
        [SerializeField] private float _optionSpacing = 10f;
        [Tooltip("padding inside the options container background.")]
        [SerializeField] private float _containerPadding = 20f;
        [Tooltip("whether to show the image background on the options container.")]
        [SerializeField] private bool _showOptionsBackground = true;

        [Header("Visual Settings")]
        [SerializeField] private Color _activeSpeakerColor = Color.white;
        [SerializeField] private Color _inactiveSpeakerColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private DialogSequenceSO _currentSequence;
        private int _currentNodeIndex;
        private readonly List<GameObject> _activeOptionButtons = new List<GameObject>();
        private readonly List<GameObject> _optionButtonPool = new List<GameObject>();

        private int _frameOpened = -1;
        private Sprite _defaultNpcSprite;
        private Sprite _defaultPlayerSprite;
        private Sprite _originalDefaultNpcSprite;
        
        private Image _optionsContainerBg;
        private RectTransform _containerRT;
        private float _buttonHeight = 50f;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            EventBus<StartDialogEvent>.AddActions(0, OnStartDialog);
            _dialogCanvas.SetActive(false);

            if (_inputReader != null) _inputReader.Interact += OnAdvanceText;

            if (_playerImage != null) _defaultPlayerSprite = _playerImage.sprite;
            if (_npcImage != null) _originalDefaultNpcSprite = _npcImage.sprite;

            _optionsContainerBg = _optionsContainer != null ? _optionsContainer.GetComponent<Image>() : null;
            _containerRT = _optionsContainer as RectTransform;
            if (_optionButtonPrefab != null && _optionButtonPrefab.TryGetComponent(out RectTransform prefabRT)) {
                _buttonHeight = prefabRT.sizeDelta.y;
            }
        }

        private void OnDestroy() {
            EventBus<StartDialogEvent>.RemoveActions(0, OnStartDialog);
            if (_inputReader != null) _inputReader.Interact -= OnAdvanceText;
        }
        #endregion

        #region Sprite Management
        private Sprite GetSpriteFromNPC(NPCController npc) {
            if (npc == null) return null;
            if (npc.TryGetComponent(out SpriteRenderer sr) || (sr = npc.GetComponentInChildren<SpriteRenderer>()))
                return sr.sprite;
            if (npc.TryGetComponent(out Image img) || (img = npc.GetComponentInChildren<Image>()))
                return img.sprite;
            return null;
        }

        private Sprite GetSpriteForSpeaker(string speakerID) {
            NPCDirector director = FindObjectOfType<NPCDirector>();
            if (director != null) {
                foreach (var npc in director.NpcsInScene) {
                    if (string.Equals(npc.gameObject.name, speakerID, StringComparison.OrdinalIgnoreCase) || 
                        npc.gameObject.name.IndexOf(speakerID, StringComparison.OrdinalIgnoreCase) >= 0) {
                        return GetSpriteFromNPC(npc);
                    }
                }
            }
            return null;
        }
        #endregion

        #region Dialog Logic
        private void OnStartDialog(StartDialogEvent evt) {
            if (evt.Sequence == null || evt.Sequence.Nodes.Count == 0) return;

            _currentSequence = evt.Sequence;
            _currentNodeIndex = 0;
            _frameOpened = Time.frameCount;

            Sprite dynamicSprite = GetSpriteFromNPC(evt.NPC);

            _defaultNpcSprite = _currentSequence.DefaultSprite != null ? _currentSequence.DefaultSprite :
                               (dynamicSprite != null ? dynamicSprite : _originalDefaultNpcSprite);

            _dialogCanvas.SetActive(true);
            UIManager.RegisterMenu(this);
            ShowNode(_currentNodeIndex);
        }

        private void ShowNode(int index) {
            if (index < 0 || index >= _currentSequence.Nodes.Count) {
                EndDialog();
                return;
            }

            DialogNode node = _currentSequence.Nodes[index];
            _dialogText.text = node.DialogText;

            bool isPlayer = string.Equals(node.SpeakerID, "Player", StringComparison.OrdinalIgnoreCase);
            Sprite displaySprite = node.SpeakerSprite;

            if (displaySprite == null) {
                if (isPlayer) {
                    displaySprite = _defaultPlayerSprite;
                } else {
                    displaySprite = GetSpriteForSpeaker(node.SpeakerID);
                    if (displaySprite == null) displaySprite = _defaultNpcSprite;
                }
            }

            if (_npcImage != null) {
                _npcImage.color = isPlayer ? _inactiveSpeakerColor : _activeSpeakerColor;
                if (!isPlayer) _npcImage.sprite = displaySprite;
            }

            if (_playerImage != null) {
                _playerImage.color = isPlayer ? _activeSpeakerColor : _inactiveSpeakerColor;
                if (isPlayer) _playerImage.sprite = node.SpeakerSprite != null ? node.SpeakerSprite : _defaultPlayerSprite;
            }

            foreach (var btn in _activeOptionButtons) {
                btn.SetActive(false);
                _optionButtonPool.Add(btn);
            }
            _activeOptionButtons.Clear();

            if (node.Options != null && node.Options.Count > 0) {
                _optionsPanel.SetActive(true);
                GameObject firstButton = null;

                if (_optionsContainerBg != null) {
                    _optionsContainerBg.enabled = _showOptionsBackground;
                }

                int n = node.Options.Count;
                float containerWidth = _containerRT != null ? _containerRT.rect.width : 300f;
                
                float centerSpacing = _optionSpacing + _buttonHeight;
                // calculate starting Y so the whole group is perfectly centered around Y=0
                float startY = ((n - 1) * centerSpacing) / 2f;

                if (_containerRT != null) {
                    float totalHeight = (n * _buttonHeight) + ((n - 1) * _optionSpacing) + (_containerPadding * 2f);
                    _containerRT.sizeDelta = new Vector2(containerWidth, totalHeight);
                }

                for (int i = 0; i < n; i++) {
                    var opt = node.Options[i];
                    GameObject go;

                    if (_optionButtonPool.Count > 0) {
                        int lastIdx = _optionButtonPool.Count - 1;
                        go = _optionButtonPool[lastIdx];
                        _optionButtonPool.RemoveAt(lastIdx);
                        go.SetActive(true);
                    }
                    else
                        go = Instantiate(_optionButtonPrefab, _optionsContainer);

                    if (go.TryGetComponent(out RectTransform rt)) {
                        // match container width minus padding, keep original height
                        rt.sizeDelta = new Vector2(containerWidth - (_containerPadding * 2f), rt.sizeDelta.y);
                        // anchor to the exact center of the container
                        rt.anchoredPosition = new Vector2(0f, startY - (i * centerSpacing));
                    }
                    
                    // ensure the button goes to the bottom of the layout group
                    go.transform.SetAsLastSibling();

                    if (go.TryGetComponent(out DialogOptionUI optUI))
                        optUI.Setup(opt, this);

                    _activeOptionButtons.Add(go);
                    if (firstButton == null) firstButton = go;
                }

                if (firstButton != null) EventSystem.current.SetSelectedGameObject(firstButton);
            }
            else {
                _optionsPanel.SetActive(false);
            }
        }

        public void OnAdvanceText() {
            if (!_dialogCanvas.activeSelf || _optionsPanel.activeSelf || Time.frameCount <= _frameOpened + 1) return;

            ShowNode(++_currentNodeIndex);
        }

        public void OnOptionSelected(DialogOption option) {
            if (!string.IsNullOrEmpty(option.ActionEventID))
                EventBus<DialogActionFiredEvent>.Raise(0, new DialogActionFiredEvent(option.ActionEventID, option.ActionParams));

            if (option.JumpToNodeIndex >= 0) ShowNode(_currentNodeIndex = option.JumpToNodeIndex);
            else EndDialog();
        }

        public void Close() => EndDialog();

        public void EndDialog() {
            if (_dialogCanvas != null) _dialogCanvas.SetActive(false);
            _currentSequence = null;
            UIManager.UnregisterMenu(this);
            EventBus<EndDialogEvent>.Raise(0, new EndDialogEvent());
        }
        #endregion
    }
}
