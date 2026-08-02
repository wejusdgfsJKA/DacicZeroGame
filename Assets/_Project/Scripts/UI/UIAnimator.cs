using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DacicZero.UI {
    public enum SlideDirection { 
        None, Up, Down, Left, Right, LeftUp, LeftDown, RightUp, RightDown
    }

    /// <summary> procedural animator for ui panels and buttons </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        
        #region Variables & Properties
        [Header("Transition Settings")]
        [Tooltip("Duration in seconds (unscaled time)")]
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private bool _fadeAlpha = true;
        [SerializeField] private SlideDirection _slideDirection = SlideDirection.Down;
        [Tooltip("Slide offset in pixels")]
        [SerializeField] private float _slideDistance = 100f;
        [SerializeField] private bool _disableOnHide = true;

        [Header("Hover Settings")]
        [SerializeField] private bool _scaleOnHover = true;
        [SerializeField] private float _hoverScaleMultiplier = 1.1f;
        [SerializeField] private float _hoverTransitionSpeed = 15f;

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        
        private Vector2 _originalAnchoredPosition;
        private Vector3 _originalScale;
        
        private Coroutine _currentTransition;
        private Coroutine _currentHover;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _rectTransform = transform as RectTransform; 
            
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _originalScale = _rectTransform.localScale;
        }

        private void OnEnable() => Show();

        private void OnDisable() {
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = _originalScale;
            if (_canvasGroup != null) {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
            }
            
            _currentTransition = null;
            _currentHover = null;
        }

        #endregion
        #region Transition Logic

        /// <summary> starts appear animation </summary>
        public void Show() {
            if (!gameObject.activeInHierarchy) return;
            if (_currentTransition != null) StopCoroutine(_currentTransition);
            _currentTransition = StartCoroutine(TransitionRoutine(true));
        }

        /// <summary> starts disappear animation </summary>
        public void Hide(System.Action onComplete = null) {
            if (!gameObject.activeInHierarchy) {
                onComplete?.Invoke();
                return;
            }
            if (_currentTransition != null) StopCoroutine(_currentTransition);
            _currentTransition = StartCoroutine(TransitionRoutine(false, onComplete));
        }

        private IEnumerator TransitionRoutine(bool isShowing, System.Action onComplete = null) {
            
            if (_canvasGroup != null) {
                _canvasGroup.blocksRaycasts = isShowing;
                _canvasGroup.interactable = isShowing;
            }

            float timeElapsed = 0f;
            float startAlpha = isShowing ? 0f : 1f;
            float targetAlpha = isShowing ? 1f : 0f;

            Vector2 startPos = _originalAnchoredPosition;
            Vector2 targetPos = _originalAnchoredPosition;

            if (_slideDirection != SlideDirection.None) {
                Vector2 offset = _slideDirection switch {
                    SlideDirection.Up => new Vector2(0, -_slideDistance),
                    SlideDirection.Down => new Vector2(0, _slideDistance),
                    SlideDirection.Left => new Vector2(_slideDistance, 0),
                    SlideDirection.Right => new Vector2(-_slideDistance, 0),
                    SlideDirection.LeftUp => new Vector2(_slideDistance, -_slideDistance),
                    SlideDirection.LeftDown => new Vector2(_slideDistance, _slideDistance),
                    SlideDirection.RightUp => new Vector2(-_slideDistance, -_slideDistance),
                    SlideDirection.RightDown => new Vector2(-_slideDistance, _slideDistance),
                    _ => Vector2.zero
                };
                
                startPos = isShowing ? _originalAnchoredPosition + offset : _originalAnchoredPosition;
                targetPos = isShowing ? _originalAnchoredPosition : _originalAnchoredPosition + offset;
            }

            if (_fadeAlpha) _canvasGroup.alpha = startAlpha;
            _rectTransform.anchoredPosition = startPos;

            while (timeElapsed < _duration) {
                timeElapsed += Time.unscaledDeltaTime; 
                float t = timeElapsed / _duration;
                
                float easeOutT = t * (2f - t); 

                if (_fadeAlpha) _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, easeOutT);
                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeOutT);
                
                yield return null;
            }

            if (_fadeAlpha) _canvasGroup.alpha = targetAlpha;
            _rectTransform.anchoredPosition = targetPos;
            _currentTransition = null;

            onComplete?.Invoke();
            if (!isShowing && _disableOnHide) 
                gameObject.SetActive(false);
        }

        #endregion
        #region Hover Logic

        public void OnPointerEnter(PointerEventData eventData) { if (_scaleOnHover) TriggerHover(_originalScale * _hoverScaleMultiplier); }
        public void OnPointerExit(PointerEventData eventData) { if (_scaleOnHover) TriggerHover(_originalScale); }

        private void TriggerHover(Vector3 targetScale) {
            if (_currentHover != null) StopCoroutine(_currentHover);
            if (gameObject.activeInHierarchy) {
                _currentHover = StartCoroutine(HoverRoutine(targetScale));
            }
        }

        private IEnumerator HoverRoutine(Vector3 targetScale) {
            while ((_rectTransform.localScale - targetScale).sqrMagnitude > 0.0001f) {
                
                // MATH FIX: Framerate-independent exponential lerp + Unscaled time
                float t = 1f - Mathf.Exp(-_hoverTransitionSpeed * Time.unscaledDeltaTime);
                _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, targetScale, t);
                
                yield return null;
            }
            _rectTransform.localScale = targetScale;
            _currentHover = null;
        }
        #endregion
    }
}