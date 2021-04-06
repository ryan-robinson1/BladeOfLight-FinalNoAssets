using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleToggleButton.Extensions;
using System.Text.RegularExpressions;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleToggleButton
{
    public class ToggleButton : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        /// <summary>
        ///  Event that occurs after
        ///  the button has been clicked.
        /// </summary>
        public event EventHandler<ToggleButtonClickedEventArgs> Clicked;

        public bool IsOn { get { return isOn; } }

        /// <summary>
        ///  Currently active background
        ///  of the button.
        /// </summary>
        public Image BackgroundImage { get { return backgroundImage; } }

        public Sprite BackgroundSprite { get { return BackgroundImage.sprite; } }

        /// <summary>
        ///  Currently active lever
        ///  of the button.
        /// </summary>
        public Image LeverImage { get { return leverImage; } }

        public Sprite LeverSprite { get { return LeverImage.sprite; } }

        public Color BackgroundColorOn { get { return backgroundColorOn; } }
        public Color BackgroundColorOff { get { return backgroundColorOff; } }
        public Color LeverColorOn { get { return leverColorOn; } }
        public Color LeverColorOff { get { return leverColorOff; } }
        public Color LeverColorHovered { get { return leverColorHovered; } }

        [SerializeField]
        private Image backgroundImage = null;

        [SerializeField]
        private Image leverImage = null;

        [SerializeField]
        private Material material = null;

        [SerializeField]
        private float size = 100f;

        [SerializeField]
        private Sprite backgroundOffSprite = null;
        [SerializeField]
        private Sprite backgroundOnSprite = null;

        [SerializeField]
        private Sprite leverOffSprite = null;
        [SerializeField]
        private Sprite leverOnSprite = null;

        [SerializeField]
        private Color backgroundColorOn = Color.white;
        [SerializeField]
        private Color backgroundColorOff = Color.white;

        [SerializeField]
        private Color leverColorOn = Color.gray;
        [SerializeField]
        private Color leverColorOff = Color.gray;
        [SerializeField]
        private Color leverColorHovered = Color.gray;

        [SerializeField]
        private ToggleButtonType type = ToggleButtonType.BigLever;

        private enum ToggleButtonHoverEffect
        {
            None,
            ChangeLeverColor
        }

        private enum TransitionType
        {
            None,
            ChangeColor,
            ChangeImage
        }

        [SerializeField]
        private ToggleButtonHoverEffect hoverEffect = ToggleButtonHoverEffect.None;

        [SerializeField]
        private TransitionType backgroundTransitionType = TransitionType.None;

        [SerializeField]
        private TransitionType leverTransitionType = TransitionType.None;

        [SerializeField]
        private bool animate = false;

        [SerializeField]
        private bool isOn = false;

        [SerializeField]
        private UnityEvent onClick = null;

        private Coroutine leverPositionCoroutine;
        private Coroutine leverCoroutine;
        private Coroutine backgroundCoroutine;

        private const float animationSpeed = 15f;
        private const float sizeModifier = 0.25f;

        private bool mouseIsOver = false;

        private Material LeverMaterial
        {
            get
            {
                if (_leverMaterial == null ||
                    _leverMaterial.shader != material.shader)
                {
                    _leverMaterial = Instantiate(material);
                    leverImage.material = _leverMaterial;
                }

                return _leverMaterial;
            }
        }
        private Material _leverMaterial;

        private Material BackgroundMaterial
        {
            get
            {
                if (_backgroundMaterial == null ||
                    _backgroundMaterial.shader != material.shader)
                {
                    _backgroundMaterial = Instantiate(material);
                    backgroundImage.material = _backgroundMaterial;
                }

                return _backgroundMaterial;
            }
        }
        private Material _backgroundMaterial;

        /// <summary>
        ///  Changes the state of the button
        ///  without calling any events (on/off).
        /// </summary>
        public void SetToggleState(bool isOn)
        {
            this.isOn = isOn;
            UpdateUI(animate);
        }

        /// <summary>
        ///  Simulates a click
        ///  on the toggle button.
        /// </summary>
        public void Click()
        {
            SetToggleState(!isOn);
            OnClicked();
        }

        /// <summary>
        ///  Called when the button is clicked
        ///  by the user.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            SetToggleState(!isOn);
            OnClicked();
        }

        /// <summary>
        ///  Called when the cursor move on
        ///  the button.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverEffect == ToggleButtonHoverEffect.ChangeLeverColor)
            {
                LeverMaterial.SetColor("_Color1", LeverColorHovered);
                LeverMaterial.SetColor("_Color2", LeverColorHovered);
            }

            mouseIsOver = true;
        }

        /// <summary>
        ///  Called when the cursor moves off
        ///  the button.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverEffect == ToggleButtonHoverEffect.ChangeLeverColor)
            {
                if (leverTransitionType == TransitionType.ChangeColor)
                {
                    LeverMaterial.SetColor("_Color1", LeverColorOff);
                    LeverMaterial.SetColor("_Color2", leverColorOn);
                }
                else
                {
                    LeverMaterial.SetColor("_Color1", Color.white);
                    LeverMaterial.SetColor("_Color2", Color.white);
                }
            }

            mouseIsOver = false;
        }

        /// <summary>
        ///  Updates the visuals of the button
        ///  depending on whether it's currently
        ///  on or off.
        /// </summary>
        /// <remarks>
        ///  If animate is false, then
        ///  the transition of the button visual
        ///  stuff is instant.
        /// </remarks>
        private void UpdateUI(bool animate)
        {
            if (leverTransitionType == TransitionType.ChangeColor)
            {
                // Different colors
                if (!mouseIsOver)
                {
                    LeverMaterial.SetColor("_Color1", LeverColorOff);
                    LeverMaterial.SetColor("_Color2", LeverColorOn);
                }
                // Same textures
                LeverMaterial.SetTexture("_MainTex1", leverOffSprite ? leverOffSprite.texture : null);
                LeverMaterial.SetTexture("_MainTex2", leverOffSprite ? leverOffSprite.texture : null);

            }
            else if (leverTransitionType == TransitionType.ChangeImage)
            {
                // Default colors
                if (!mouseIsOver)
                {
                    LeverMaterial.SetColor("_Color1", Color.white);
                    LeverMaterial.SetColor("_Color2", Color.white);
                }
                // Different textures
                LeverMaterial.SetTexture("_MainTex1", leverOffSprite ? leverOffSprite.texture : null);
                LeverMaterial.SetTexture("_MainTex2", leverOnSprite ? leverOnSprite.texture : null);
            }
            else
            {
                // Default colors
                if (!mouseIsOver)
                {
                    LeverMaterial.SetColor("_Color1", Color.white);
                    LeverMaterial.SetColor("_Color2", Color.white);
                }
                // Same textures
                LeverMaterial.SetTexture("_MainTex1", leverOffSprite ? leverOffSprite.texture : null);
                LeverMaterial.SetTexture("_MainTex2", leverOffSprite ? leverOffSprite.texture : null);
            }

            if (backgroundTransitionType == TransitionType.ChangeColor)
            {
                // Different colors
                BackgroundMaterial.SetColor("_Color1", BackgroundColorOff);
                BackgroundMaterial.SetColor("_Color2", BackgroundColorOn);
                // Same textures
                BackgroundMaterial.SetTexture("_MainTex1", backgroundOffSprite ? backgroundOffSprite.texture : null);
                BackgroundMaterial.SetTexture("_MainTex2", backgroundOffSprite ? backgroundOffSprite.texture : null);
            }
            else if (backgroundTransitionType == TransitionType.ChangeImage)
            {
                // Default colors
                BackgroundMaterial.SetColor("_Color1", Color.white);
                BackgroundMaterial.SetColor("_Color2", Color.white);
                // Different textures
                BackgroundMaterial.SetTexture("_MainTex1", backgroundOffSprite ? backgroundOffSprite.texture : null);
                BackgroundMaterial.SetTexture("_MainTex2", backgroundOnSprite ? backgroundOnSprite.texture : null);
            }
            else
            {
                // Default colors
                BackgroundMaterial.SetColor("_Color1", Color.white);
                BackgroundMaterial.SetColor("_Color2", Color.white);
                // Same textures
                BackgroundMaterial.SetTexture("_MainTex1", backgroundOffSprite ? backgroundOffSprite.texture : null);
                BackgroundMaterial.SetTexture("_MainTex2", backgroundOffSprite ? backgroundOffSprite.texture : null);
            }

            // Deciding lever direction
            float positionX = size / 2f;

            if (!isOn)
            {
                positionX = -positionX;
            }

            Vector2 targetPosition = new Vector2(positionX, 0f);

            if (animate)
            {
                MoveLeverAnimated(targetPosition);
            }
            else
            {
                MoveLeverInstantly(targetPosition);
            }
        }

        private void MoveLeverInstantly(Vector3 position)
        {
            leverImage.transform.localPosition = position;

            if (isOn)
            {
                LeverMaterial.SetFloat("_Blend", 1f);
                BackgroundMaterial.SetFloat("_Blend", 1f);
            }
            else
            {
                LeverMaterial.SetFloat("_Blend", 0f);
                BackgroundMaterial.SetFloat("_Blend", 0f);
            }
        }

        #region Animation

        private void MoveLeverAnimated(Vector3 position)
        {
            // Changing position
            RestartCoroutine(
                ref leverPositionCoroutine,
                ChangeLeverPosition(position));

            // Changing color or image
            RestartCoroutine(
                ref leverCoroutine,
                ChangeImageBlend(LeverMaterial, isOn ? 1f : 0f));
            RestartCoroutine(
                ref backgroundCoroutine,
                ChangeImageBlend(BackgroundMaterial, isOn ? 1f : 0f));
        }

        private IEnumerator ChangeLeverPosition(Vector3 position)
        {
            while (leverImage.transform.localPosition != position)
            {
                leverImage.transform.localPosition = Vector3.Lerp(
                    leverImage.transform.localPosition,
                    position,
                    Time.unscaledDeltaTime * animationSpeed);

                yield return null;
            }
        }

        private IEnumerator ChangeImageBlend(Material imageMaterial, float target)
        {
            while (imageMaterial.GetFloat("_Blend") != target)
            {
                imageMaterial.SetFloat(
                    "_Blend",
                    Mathf.Lerp(
                        imageMaterial.GetFloat("_Blend"),
                        target,
                        Time.unscaledDeltaTime * animationSpeed));

                yield return null;
            }
        }

        /// <summary>
        ///  Stops the specified coroutine
        ///  if it's not null and starts a new one.
        /// </summary>
        private void RestartCoroutine(
            ref Coroutine coroutineToRestart,
            IEnumerator newRoutine)
        {
            if (coroutineToRestart != null)
            {
                StopCoroutine(coroutineToRestart);
            }
            coroutineToRestart = StartCoroutine(newRoutine);
        }

        #endregion

        /// <summary>
        ///  Event function which gets called
        ///  when the button is clicked.
        /// </summary>
        private void OnClicked()
        {
            onClick.Invoke();
            EventHandler<ToggleButtonClickedEventArgs> handler = Clicked;
            if (handler != null)
            {
                handler(this, new ToggleButtonClickedEventArgs(isOn, this));
            }
        }

        /// <summary>
        ///  Called in the editor when a change is made
        ///  in the inspector.
        /// </summary>
        private void OnValidate()
        {
            if (backgroundImage != null && leverImage != null)
            {
                if (type == ToggleButtonType.BigLever)
                {
                    leverImage.rectTransform.sizeDelta = new Vector2(
                        size * (1 + sizeModifier),
                        size * (1 + sizeModifier));

                    backgroundImage.rectTransform.sizeDelta = new Vector2(size * 2f, size);
                }
                else if (type == ToggleButtonType.SmallLever)
                {
                    leverImage.rectTransform.sizeDelta = new Vector2(size, size);

                    backgroundImage.rectTransform.sizeDelta = new Vector2(
                        size * 2f * (1 + sizeModifier / 2f),
                        size * (1 + sizeModifier));
                }

                if (backgroundTransitionType != TransitionType.ChangeColor)
                {
                    backgroundImage.color = new Color(1f, 1f, 1f, 1f);
                }

                if (leverTransitionType != TransitionType.ChangeColor)
                {
                    leverImage.color = new Color(1f, 1f, 1f, 1f);
                }

                UpdateUI(false);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        ///  Responsible for rendering things in the inspector
        ///  and giving the ability to create the button
        ///  using the context menu.
        /// </summary>
        [CustomEditor(typeof(ToggleButton))]
        public partial class EditorToggleButton : Editor
        {
            private SerializedProperty backgroundImage;
            private SerializedProperty leverImage;
            private SerializedProperty material;
            private SerializedProperty size;
            private SerializedProperty backgroundColorOn;
            private SerializedProperty backgroundColorOff;
            private SerializedProperty leverColorOn;
            private SerializedProperty leverColorOff;
            private SerializedProperty backgroundOffSprite;
            private SerializedProperty backgroundOnSprite;
            private SerializedProperty leverOffSprite;
            private SerializedProperty leverOnSprite;
            private SerializedProperty leverColorHovered;
            private SerializedProperty type;
            private SerializedProperty hoverEffect;
            private SerializedProperty backgroundTransitionType;
            private SerializedProperty leverTransitionType;
            private SerializedProperty animate;
            private SerializedProperty isOn;

            private bool showReferences = false;

            private void OnEnable()
            {
                var toggleButton = target as ToggleButton;

                backgroundImage = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundImage));
                leverImage = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverImage));
                material = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.material));
                size = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.size));
                backgroundColorOn = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundColorOn));
                backgroundColorOff = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundColorOff));
                leverColorOn = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverColorOn));
                leverColorOff = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverColorOff));
                backgroundOffSprite = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundOffSprite));
                backgroundOnSprite = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundOnSprite));
                leverOffSprite = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverOffSprite));
                leverOnSprite = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverOnSprite));
                leverColorHovered = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverColorHovered));
                type = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.type));
                hoverEffect = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.hoverEffect));
                backgroundTransitionType = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.backgroundTransitionType));
                leverTransitionType = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.leverTransitionType));
                animate = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.animate));
                isOn = serializedObject.FindProperty(ExtraMethods
                    .NameOf(() => toggleButton.isOn));
            }

            public override void OnInspectorGUI()
            {
                // base.OnInspectorGUI();

                var toggleButton = target as ToggleButton;

                showReferences = EditorGUILayout.Foldout(
                    showReferences,
                    "References",
                    true);


                if (showReferences)
                {
                    // Moving stuff a bit to the right
                    EditorGUI.indentLevel++;

                    // "background" image object
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        ExtraMethods.NameOfC(() => toggleButton.backgroundImage));
                    backgroundImage.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.backgroundImage,
                        typeof(Image),
                        true) as Image;
                    EditorGUILayout.EndHorizontal();

                    // "lever" image object
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        ExtraMethods.NameOfC(() => toggleButton.leverImage));
                    leverImage.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.leverImage,
                        typeof(Image),
                        true) as Image;
                    EditorGUILayout.EndHorizontal();

                    // "material" reference
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        ExtraMethods.NameOfC(() => toggleButton.material));
                    material.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.material,
                        typeof(Material),
                        true) as Material;
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    // Changing back to original for the rest of the content
                    EditorGUI.indentLevel--;
                }


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

                // Size
                size.floatValue = EditorGUILayout.FloatField(
                    ExtraMethods.NameOfC(() => toggleButton.size),
                    toggleButton.size);

                // Type
                type.enumValueIndex = (int)(ToggleButtonType)
                    EditorGUILayout.EnumPopup(
                        ExtraMethods.NameOfC(() => toggleButton.type),
                        toggleButton.type);

                // Hover effect
                hoverEffect.enumValueIndex = (int)(ToggleButtonHoverEffect)
                    EditorGUILayout.EnumPopup(
                        ExtraMethods.NameOfC(() => toggleButton.hoverEffect),
                        toggleButton.hoverEffect);

                // Animate
                animate.boolValue = EditorGUILayout.Toggle(
                    ExtraMethods.NameOfC(() => toggleButton.animate),
                    toggleButton.animate);

                // Is on
                isOn.boolValue = EditorGUILayout.Toggle(
                    ExtraMethods.NameOfC(() => toggleButton.isOn),
                    toggleButton.isOn);


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Background", EditorStyles.boldLabel);

                backgroundTransitionType.enumValueIndex = (int)(TransitionType)
                    EditorGUILayout.EnumPopup(
                        "Transition Type",
                        toggleButton.backgroundTransitionType);

                if (toggleButton.backgroundTransitionType == TransitionType.ChangeColor)
                {
                    // Background off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Sprite");
                    backgroundOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.backgroundOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();

                    // Background color on
                    backgroundColorOn.colorValue = EditorGUILayout.ColorField(
                        "On Color",
                        toggleButton.backgroundColorOn);

                    // Background color off
                    backgroundColorOff.colorValue = EditorGUILayout.ColorField(
                        "Off Color",
                        toggleButton.backgroundColorOff);
                }
                else if (toggleButton.backgroundTransitionType == TransitionType.ChangeImage)
                {
                    // Background on sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("On Sprite");
                    backgroundOnSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.backgroundOnSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();

                    // Background off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Off Sprite");
                    backgroundOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.backgroundOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // Background off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Sprite");
                    backgroundOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.backgroundOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();
                }


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Lever", EditorStyles.boldLabel);

                leverTransitionType.enumValueIndex = (int)(TransitionType)
                    EditorGUILayout.EnumPopup(
                        "Transition Type",
                        toggleButton.leverTransitionType);

                if (toggleButton.leverTransitionType == TransitionType.ChangeColor)
                {
                    // Lever off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Sprite");
                    leverOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.leverOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();
                    
                    // Lever color on
                    leverColorOn.colorValue = EditorGUILayout.ColorField(
                        "On Color",
                        toggleButton.leverColorOn);

                    // Lever color off
                    leverColorOff.colorValue = EditorGUILayout.ColorField(
                        "Off Color",
                        toggleButton.leverColorOff);
                }
                else if (toggleButton.leverTransitionType == TransitionType.ChangeImage)
                {
                    // Lever on sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        "On Sprite");
                    leverOnSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.leverOnSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();

                    // Lever off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(
                        "Off Sprite");
                    leverOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.leverOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // Lever off sprite
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Sprite");
                    leverOffSprite.objectReferenceValue = EditorGUILayout.ObjectField(
                        toggleButton.leverOffSprite,
                        typeof(Sprite),
                        true) as Sprite;
                    EditorGUILayout.EndHorizontal();
                }

                if (toggleButton.hoverEffect == ToggleButtonHoverEffect.ChangeLeverColor)
                {
                    // Lever color hovered
                    leverColorHovered.colorValue = EditorGUILayout.ColorField(
                        "Hovered Color",
                        toggleButton.leverColorHovered);
                }
                EditorGUILayout.Space();


                // On click
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty(
                        ExtraMethods.NameOf(() => toggleButton.onClick)),
                    true);

                // Allows the events to be added
                serializedObject.ApplyModifiedProperties();
            }

            [MenuItem("GameObject/UI/Toggle Button")]
            private static void CreateButton()
            {
                var buttonPrefab = Resources.Load<ToggleButton>("Toggle Button");

                ToggleButton button = PrefabUtility
                    .InstantiatePrefab(buttonPrefab) as ToggleButton;

                var canvas = Find<Canvas>();
                if (canvas == null)
                {
                    canvas = CreateCanvas();
                    var eventSystem = Find<EventSystem>();
                    if (eventSystem == null)
                    {
                        eventSystem = CreateEventSystem();
                    }
                }

                button.transform.SetParent(canvas.transform);
                button.transform.localPosition = Vector2.zero;

                // Selecting the object at hierarchy
                Selection.activeGameObject = button.gameObject;

                Undo.RegisterCreatedObjectUndo(button.gameObject, "Create Toggle Button");
            }

            /// <summary>
            ///  Finds the first selected object
            ///  of the specified type. If there isn't
            ///  one, it returns any one that's available.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            private static T Find<T>() where T : Behaviour
            {
                // looking for a selected canvas object first
                T canvas = Selection.gameObjects
                    .Select(x => x.GetComponent<T>())
                    .FirstOrDefault(x => x != null);

                // alternatively, looking for any one that's available
                if (canvas == null)
                {
                    canvas = FindObjectOfType<T>();
                }

                return canvas;
            }

            /// <summary>
            ///  Creates an instance of canvas
            ///  object with all of the default values.
            /// </summary>
            private static Canvas CreateCanvas()
            {
                var canvasGameObject = new GameObject("Canvas");
                var canvas = canvasGameObject.AddComponent<Canvas>();
                canvasGameObject.AddComponent<CanvasScaler>();
                canvasGameObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create Canvas");
                return canvas;
            }

            /// <summary>
            ///  Creates an instance of event system
            ///  object with all of the default values.
            /// </summary>
            private static EventSystem CreateEventSystem()
            {
                var eventSystemGameObject = new GameObject("EventSystem");
                var eventSystem = eventSystemGameObject.AddComponent<EventSystem>();
                eventSystemGameObject.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventSystemGameObject, "Create EventSystem");
                return eventSystem;
            }
        }
#endif
    }

    public static class ExtraMethods
    {
        /// <summary>
        ///  Obtains the name of a variable,
        ///  capitalizes the first letter
        ///  and splits it into separate words.
        /// </summary>
        /// <remarks>
        ///  Usage: ThisFunction(() => VARIABLE)
        /// </remarks>
        public static string NameOfC<T>(Expression<Func<T>> expression)
        {
            string name = NameOf<T>(expression).ToUpperFirstLetter();

            return Regex
                .Replace(name, @"[A-Z]", m => $" {m.Value}")
                .Trim();
        }

        /// <summary>
        ///  Finds the name of a variable.
        /// </summary>
        /// <remarks>
        ///  Usage: ThisFunction(() => VARIABLE)
        /// </remarks>
        public static string NameOf<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        /// <summary>
        ///  Capitalizes the first letter
        ///  of a string.
        /// </summary>
        /// <param name="source">
        ///  String to modify.
        /// </param>
        /// <returns>
        ///  Initial string with the
        ///  first letter capitalized.
        /// </returns>
        private static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            char[] letters = source.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            return new string(letters);
        }
    }

    public enum ToggleButtonType
    {
        BigLever,
        SmallLever
    }
}
