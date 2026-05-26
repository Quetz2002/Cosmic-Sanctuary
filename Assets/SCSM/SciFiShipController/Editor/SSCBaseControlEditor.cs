using UnityEditor;
using UnityEngine;

// Sci-Fi Ship Controller. Copyright (c) 2018-2025 SCSM Pty Ltd. All rights reserved.
namespace SciFiShipController
{
    [CustomEditor(typeof(SSCBaseControl))]
    [CanEditMultipleObjects]
    public class SSCBaseControlEditor : Editor
    {
        #region GUIContent - General
        protected readonly static GUIContent isMakeInteractiveContent = new GUIContent(" Make Interactive", "If Sticky3D Controller is installed, make this control interactive");
        protected readonly static GUIContent s3dObjectAnimSetListContent = new GUIContent(" Object Anim Sets", "The list of object anim sets for Sticky3D character models that apply to this interactive-enabled object");
        protected readonly static GUIContent nonClippableLayerContent = new GUIContent(" Non Clippable Layer", "Unity Layer for NonClippable objects. In SSC NonClippable is 24 if not already allocated. Collisions between this and the ship layer will be disabled.");
        protected readonly static GUIContent handHoldOffsetContent = new GUIContent(" Hand Hold Offset", "The hand hold local space offset.");
        protected readonly static GUIContent handHoldRotationContent = new GUIContent(" Hand Hold Rotation", "The hand hold local space rotation");
        protected readonly static GUIContent shipControlModuleContent = new GUIContent(" Ship to Control", "The control should be a child of this ship");
        protected readonly static GUIContent interactiveTagNameContent = new GUIContent(" Interactive Tag", "The interactive tag name to assign to this object. Typically this will come from the interactivetags collection that is associate with the Sticky3D pilot controller on the ship.");
        protected readonly static GUIContent isFlyShipWhenEngagedContent = new GUIContent(" Fly Ship when Engaged", "When a character is using the control, allow them to fly the ship. If there are multiple controls used at the same time to fly a ship, only one should have this enabled.");
        protected readonly static GUIContent companionControlContent = new GUIContent(" Companion Control", "Automatically enable or disable control when the current control is engaged or disengaged by a character. Be careful not to make a circular loop between the two controls.");
        protected readonly static GUIContent isVRModeContent = new GUIContent(" VR Mode", "The control is operating as a VR player control");
        protected readonly static GUIContent isInvertXContent = new GUIContent(" Invert X", "Flip or invert the x-axis values");
        protected readonly static GUIContent isInvertZContent = new GUIContent(" Invert Z", "Flip or invert the z-axis values");
        #endregion

        #region Custom Editor protected variables
        // These are visible to inherited classes

        protected SSCBaseControl baseControl;

        // Formatting and style variables
        protected string labelText;
        protected GUIStyle labelFieldRichText;
        protected GUIStyle helpBoxRichText;
        protected GUIStyle buttonCompact;
        protected float defaultEditorLabelWidth = 0f;
        protected float defaultEditorFieldWidth = 0f;
        protected bool isDebuggingEnabled = false;
        protected bool isStylesInitialised = false;
        protected bool isSceneModified = false;
        protected GUIStyle headingFieldRichText;
        protected GUIStyle foldoutStyleNoLabel;
        protected GUIStyle helpLabelRichText;
        protected GUIStyle toggleCompactButtonStyleNormal = null;  // Small Toggle button. e.g. G(izmo) on/off
        protected GUIStyle toggleCompactButtonStyleToggled = null;
        protected Color separatorColor = new Color();
        protected bool isSceneDirtyRequired = false;

        #endregion

        #region Serialized Properties - General
        protected SerializedProperty isMakeInteractiveProp;
        protected SerializedProperty s3dObjectAnimSetListProp;
        protected SerializedProperty nonClippableLayerProp;
        protected SerializedProperty handHoldOffsetProp;
        protected SerializedProperty handHoldRotationProp;
        protected SerializedProperty shipControlModuleProp;
        protected SerializedProperty interactiveTagNameProp;
        protected SerializedProperty isFlyShipWhenEngagedProp;
        protected SerializedProperty companionControlProp;
        protected SerializedProperty isVRModeProp;
        protected SerializedProperty isInvertXProp;
        protected SerializedProperty isInvertZProp;
        #endregion

        #region Events

        public virtual void OnEnable()
        {
            baseControl = (SSCBaseControl)target;

            defaultEditorLabelWidth = 150f; // EditorGUIUtility.labelWidth;
            defaultEditorFieldWidth = EditorGUIUtility.fieldWidth;

            separatorColor = EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f, 2f) : Color.grey;

            // Reset GUIStyles
            isStylesInitialised = false;
            toggleCompactButtonStyleNormal = null;
            toggleCompactButtonStyleToggled = null;
            foldoutStyleNoLabel = null;

            #region Find Properties - General
            isMakeInteractiveProp = serializedObject.FindProperty("isMakeInteractive");
            s3dObjectAnimSetListProp = serializedObject.FindProperty("s3dObjectAnimSetList");
            nonClippableLayerProp = serializedObject.FindProperty("nonClippableLayer");
            handHoldOffsetProp = serializedObject.FindProperty("handHoldOffset");
            handHoldRotationProp = serializedObject.FindProperty("handHoldRotation");
            shipControlModuleProp = serializedObject.FindProperty("shipControlModule");
            interactiveTagNameProp = serializedObject.FindProperty("interactiveTagName");
            isFlyShipWhenEngagedProp = serializedObject.FindProperty("isFlyShipWhenEngaged");
            companionControlProp = serializedObject.FindProperty("companionControl");
            isVRModeProp = serializedObject.FindProperty("isVRMode");
            isInvertXProp = serializedObject.FindProperty("isInvertX");
            isInvertZProp = serializedObject.FindProperty("isInvertZ");
            #endregion
        }

        #endregion

        #region Protected Non-virtual Methods

        /// <summary>
        /// Set up the buttons and styles used in OnInspectorGUI.
        /// Call this near the top of OnInspectorGUI.
        /// </summary>
        protected void ConfigureButtonsAndStyles()
        {
            // Set up rich text GUIStyles
            if (!isStylesInitialised)
            {
                helpBoxRichText = new GUIStyle("HelpBox");
                helpBoxRichText.richText = true;

                labelFieldRichText = new GUIStyle("Label");
                labelFieldRichText.richText = true;

                headingFieldRichText = new GUIStyle(UnityEditor.EditorStyles.miniLabel);
                headingFieldRichText.richText = true;
                headingFieldRichText.normal.textColor = helpBoxRichText.normal.textColor;

                // Overide default styles
                EditorStyles.foldout.fontStyle = FontStyle.Bold;

                // When using a no-label foldout, don't forget to set the global
                // EditorGUIUtility.fieldWidth to a small value like 15, then back
                // to the original afterward.
                foldoutStyleNoLabel = new GUIStyle(EditorStyles.foldout);
                foldoutStyleNoLabel.fixedWidth = 0.01f;

                helpLabelRichText = new GUIStyle("Label");
                helpLabelRichText.richText = true;
                helpLabelRichText.wordWrap = true;
                helpLabelRichText.font = helpBoxRichText.font;
                helpLabelRichText.fontSize = helpBoxRichText.fontSize;
                helpLabelRichText.fontStyle = helpBoxRichText.fontStyle;
                helpLabelRichText.normal = helpBoxRichText.normal;

                buttonCompact = new GUIStyle("Button");
                buttonCompact.fontSize = 10;

                // Create a new button or else will effect the Button style for other buttons too
                toggleCompactButtonStyleNormal = new GUIStyle("Button");
                toggleCompactButtonStyleToggled = new GUIStyle(toggleCompactButtonStyleNormal);
                toggleCompactButtonStyleNormal.fontStyle = FontStyle.Normal;
                toggleCompactButtonStyleToggled.fontStyle = FontStyle.Bold;
                toggleCompactButtonStyleToggled.normal.background = toggleCompactButtonStyleToggled.active.background;

                isStylesInitialised = true;
            }
        }

        /// <summary>
        /// Draw invert X values in the inspector
        /// </summary>
        protected void DrawInvertX()
        {
            EditorGUILayout.PropertyField(isInvertXProp, isInvertXContent);
        }

        /// <summary>
        /// Draw flip Z values in the inspector
        /// </summary>
        protected void DrawInvertZ()
        {
            EditorGUILayout.PropertyField(isInvertZProp, isInvertZContent);
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// This function overides what is normally seen in the inspector window
        /// This allows stuff like buttons to be drawn there.
        /// </summary>
        protected virtual void DrawBaseInspector()
        {
            #region Initialise
            EditorGUIUtility.labelWidth = defaultEditorLabelWidth;
            EditorGUIUtility.fieldWidth = defaultEditorFieldWidth;
            #endregion

            ConfigureButtonsAndStyles();

            // Read in all the properties
            serializedObject.Update();

            #region Headers and Info Buttons
            DrawVersionLabel();
            DrawComponentDescription();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawHelpButtons();
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.BeginVertical("HelpBox");
            DrawGeneralSettings();            
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw general settings in the inspector
        /// </summary>
        protected virtual void DrawGeneralSettings()
        {
            EditorGUILayout.PropertyField(isMakeInteractiveProp, isMakeInteractiveContent);         

            #if !SCSM_S3D
            if (isMakeInteractiveProp.boolValue)
            {
                SSCEditorHelper.DrawSticky3DNotInstalled();
            }
            #endif

            SSCEditorHelper.DrawSSCLayer(0f, nonClippableLayerProp, nonClippableLayerContent, defaultEditorLabelWidth);

            SSCEditorHelper.DrawSSCPropertyWithReset(0f, handHoldOffsetProp, handHoldOffsetContent, buttonCompact, defaultEditorLabelWidth, ResetHandHoldOffset);
            SSCEditorHelper.DrawSSCPropertyWithReset(0f, handHoldRotationProp, handHoldRotationContent, buttonCompact, defaultEditorLabelWidth, ResetHandHoldRotation);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(shipControlModuleProp, shipControlModuleContent);
            if (EditorGUI.EndChangeCheck() && shipControlModuleProp.objectReferenceValue != null)
            {
                if (!baseControl.transform.IsChildOf(((ShipControlModule)shipControlModuleProp.objectReferenceValue).transform))
                {
                    shipControlModuleProp.objectReferenceValue = null;
                    Debug.LogWarning("[ERROR] This control (" + baseControl.name + ") must be on child gameobject of the ship");
                }
            }

            EditorGUILayout.PropertyField(companionControlProp, companionControlContent);

            #if SCSM_XR && SSC_UIS
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isVRModeProp, isVRModeContent);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                baseControl.SetVRMode(isVRModeProp.boolValue);
                serializedObject.Update();
            }
            #endif

            #if SCSM_S3D
            EditorGUILayout.PropertyField(interactiveTagNameProp, interactiveTagNameContent);
            EditorGUILayout.PropertyField(isFlyShipWhenEngagedProp, isFlyShipWhenEngagedContent);
            SSCEditorHelper.DrawSSCHorizontalGap(3f);
            EditorGUILayout.PropertyField(s3dObjectAnimSetListProp, s3dObjectAnimSetListContent);
            #endif
        }

        /// <summary>
        /// Draw the description of the component in the inspector
        /// </summary>
        protected virtual void DrawComponentDescription()
        {

        }

        /// <summary>
        /// Draw the version label in the inspector
        /// </summary>
        protected virtual void DrawVersionLabel()
        {
            SSCEditorHelper.SSCVersionHeader(labelFieldRichText);
        }

        /// <summary>
        /// Draw the help buttons in the inspector
        /// </summary>
        protected virtual void DrawHelpButtons()
        {
            SSCEditorHelper.DrawSSCGetHelpButtons(buttonCompact);
        }

        /// <summary>
        /// Reset the hand hold offset to defaults
        /// </summary>
        protected virtual void ResetHandHoldOffset()
        {
            handHoldOffsetProp.vector3Value = Vector3.zero;
        }

        /// <summary>
        /// Reset the hand hold rotation Euler values to defaults
        /// </summary>
        protected virtual void ResetHandHoldRotation()
        {
            handHoldRotationProp.vector3Value = Vector3.zero;
        }

        #endregion

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            #if !SCSM_SSC
            EditorGUILayout.HelpBox("Sci-Fi Ship Controller asset is missing from the project.", MessageType.Error);
            #else
            DrawBaseInspector();
            #endif
        }

        #endregion
    }
}