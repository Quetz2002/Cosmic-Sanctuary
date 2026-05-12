using UnityEngine;
using System.Collections.Generic;
#if SCSM_S3D
using scsmmedia;
#endif

// Sci-Fi Ship Controller. Copyright (c) 2018-2025 SCSM Pty Ltd. All rights reserved.
namespace SciFiShipController
{
    /// <summary>
    /// A base component that make an in-game ship controls (like joysticks and throttles) be interactive.
    /// Typically used with the Sticky3D Controller asset.
    /// Used in the cockpit or on a bridge of a space craft.
    /// </summary>
    [HelpURL("https://scsmmedia.com/ssc-documentation")]
    public class SSCBaseControl : MonoBehaviour
    {
        #region Public Variables

        /// <summary>
        /// If Sticky3D Controller is installed, make this control interactive
        /// </summary>
        public bool isMakeInteractive = false;

        /// <summary>
        /// Unity Layer for NonClippable objects. In SSC, NonClippable is 24 if not already allocated. Collisions between this and the ship layer will be disabled.
        /// </summary>
        [Range(0, 31)] public int nonClippableLayer = 24;

        /// <summary>
        /// The hand hold local space offset.
        /// </summary>
        public Vector3 handHoldOffset = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// The hand hold relative rotation. The rotation is stored as Euler angles (degrees).
        /// </summary>
        public Vector3 handHoldRotation = new Vector3(0f, 0f, 0f);

        #endregion

        #region Public Properties

        /// <summary>
        /// The default hand hold offset for this control
        /// </summary>
        public virtual Vector3 DefaultHandHoldOffset { get => Vector3.zero; }

        /// <summary>
        /// The default hand hold Euler angles rotation for this control
        /// </summary>
        public virtual Vector3 DefaultHandHoldRotation { get => Vector3.zero; }

        /// <summary>
        /// Does this control have a companion control that is automatically enabled or disabled when the current control is engaged or disengaged by a character.
        /// </summary>
        public bool HasCompanionControl { get  => companionControl != null; }

        /// <summary>
        /// Is the component initialised?
        /// </summary>
        public bool IsInitialised { get { return isInitialised; } }

        /// <summary>
        /// Is the component interactive-enabled?
        /// </summary>
        public bool IsInteractive { get { return isInteractive; } }

        #if SCSM_S3D

        /// <summary>
        /// Return the interactive component if it has been added
        /// </summary>
        public StickyInteractive ControlInteractive { get { return controlInteractive; } }

        /// <summary>
        /// Return the StickyInteractiveID if the StickyInteractive component has been added
        /// </summary>
        public int ControlInteractiveID { get { return controlInteractive == null ? 0 : controlInteractive.StickyInteractiveID; } }

        #endif

        #endregion

        #region Protected Variables - General

        /// <summary>
        /// The list of object anim sets for Sticky3D character models that apply to this interactive-enabled object
        /// </summary>
        [SerializeField] protected List<ScriptableObject> s3dObjectAnimSetList;

        /// <summary>
        /// The control should be a child of this ship
        /// </summary>
        [SerializeField] protected ShipControlModule shipControlModule;

        /// <summary>
        /// The interactive tag name to assign to this object. Typically this will come from the interactivetags
        /// collection that is associate with the Sticky3D pilot controller on the ship.
        /// </summary>
        [SerializeField] protected string interactiveTagName = "In-game Control";

        /// <summary>
        /// When a character is using the control, allow them to fly the ship. If there are multiple controls used at the same time to fly a ship, only one should have this enabled.
        /// </summary>
        [SerializeField] protected bool isFlyShipWhenEngaged = false;

        /// <summary>
        /// Automatically enable or disable control when the current control is engaged or disengaged by a character.
        /// Be careful not to make a circular loop between the two controls.
        /// </summary>
        [SerializeField] protected SSCBaseControl companionControl = null;

        /// <summary>
        /// Flip or invert the x-axis readable values
        /// </summary>
        [SerializeField] protected bool isInvertX = false;

        /// <summary>
        /// Flip or invert the z-axis readable values
        /// </summary>
        [SerializeField] protected bool isInvertZ = false;

        /// <summary>
        /// The control is operating as a VR player control
        /// </summary>
        [SerializeField] protected bool isVRMode = false;

        /// <summary>
        /// A reference back to the parent body module. Not sure if there is a better
        /// way to avoid the parent-child-parent references.
        /// </summary>
        [System.NonSerialized] protected ShipBodyModuleBase shipBodyModuleBase;

        [System.NonSerialized] protected bool isInitialised = false;
        [System.NonSerialized] protected bool isInteractive = false;

        #if SCSM_S3D
        [System.NonSerialized] protected StickyInteractive controlInteractive = null;
        
        /// <summary>
        /// Typically used in VR to send data to a ship
        /// </summary>
        [System.NonSerialized] protected SSCInputBridge inputBridge = null;

        /// <summary>
        /// Typically used to get data from a ship
        /// </summary>
        [System.NonSerialized] protected SSCOutputBridge outputBridge = null;
        #endif
        #endregion

        #region Protected and Internal Methods - General

        /// <summary>
        /// Override this for the OnEnable events
        /// </summary>
        protected virtual void OnEnableControl()
        {

        }

        /// <summary>
        /// Override this for OnDestroy events
        /// </summary>
        protected virtual void OnDestroyControl()
        {
            RemoveListeners();
        }

        #endregion

        #region Protected and Internal Methods - Sticky3D

        #if SCSM_S3D

        /// <summary>
        /// Add any Object Anim Sets to the interactive object
        /// </summary>
        protected void AddObjectAnimSets()
        {
            int numObjAnimSets = s3dObjectAnimSetList == null ? 0 : s3dObjectAnimSetList.Count;

            for (int asIdx = 0; asIdx < numObjAnimSets; asIdx++)
            {
                controlInteractive.AddObjectAnimSet(s3dObjectAnimSetList[asIdx] as S3DObjectAnimSet);
            }
        }

        /// <summary>
        /// Get the Sticky3D SSCInputBridge component on the control.
        /// If it does not exist, add it. Typically used for Player VR in-game ship controls.
        /// </summary>
        /// <returns></returns>
        protected bool CheckInputBridge()
        {
            bool hasInputBridge = controlInteractive.TryGetComponent(out inputBridge);

            if (!hasInputBridge)
            {
                inputBridge = controlInteractive.gameObject.AddComponent<SSCInputBridge>();
                hasInputBridge = inputBridge != null;
            }

            return hasInputBridge;
        }

        /// <summary>
        /// Get the Sticky3D SSCOutputBridge component on the control.
        /// If it does not exist, add it.
        /// </summary>
        /// <returns></returns>
        protected bool CheckOutputBridge()
        {
            bool hasOutputBridge = controlInteractive.TryGetComponent(out outputBridge);

            if (!hasOutputBridge)
            {
                outputBridge = controlInteractive.gameObject.AddComponent<SSCOutputBridge>();
                hasOutputBridge = outputBridge != null;
            }

            return hasOutputBridge;
        }

        /// <summary>
        /// Override this to finish configuring the interactive object
        /// </summary>
        protected virtual void ConfigureInteractive()
        {
            controlInteractive.gameObject.layer = nonClippableLayer;
            controlInteractive.SetHandHoldOffset(handHoldOffset, false);
            controlInteractive.SetHandHoldRotation(handHoldRotation, false);

            // If there is a player S3D character that can interact with this ship
            // the control is on, attempt to assign the interactive tag to this control.
            StickyControlModule s3dCharacter;
            if (shipBodyModuleBase.GetPlayerCharacterS3D(out s3dCharacter))
            {
                RefreshInteractiveTag(s3dCharacter.InteractiveTags);
            }

            if (isVRMode)
            {
                controlInteractive.onPostGrabbed = new S3DInteractiveEvt1();
                controlInteractive.onPostGrabbed.AddListener(CharacterGrabbed);

                controlInteractive.onDropped = new S3DInteractiveEvt2();
                controlInteractive.onDropped.AddListener(CharacterStopGrabbing);
            }
            else
            {
                // Get notified when a character starts touching the control
                controlInteractive.onTouched = new S3DInteractiveEvt1();
                controlInteractive.onTouched.AddListener(CharacterStartTouching);

                // Get notified when a character stops touching the control
                controlInteractive.onStoppedTouching = new S3DInteractiveEvt2();
                controlInteractive.onStoppedTouching.AddListener(CharacterStopTouching);
            }
        }

        /// <summary>
        /// Check if the companion's companion is this control (which would be bad and could lead to endless loop)
        /// </summary>
        /// <returns></returns>
        protected bool IsCompanionDeadlock()
        {
            return companionControl.companionControl != null && companionControl.companionControl.GetHashCode() == GetHashCode();
        }

        #endif

        #endregion

        #region Events

        private void OnDestroy()
        {
            OnDestroyControl(); 
        }

        private void OnEnable()
        {
            OnEnableControl();
        }

        #endregion

        #region Public API Methods

        /// <summary>
        /// Attempt to initialise the interactive joystick control
        /// </summary>
        /// <param name="bodyModule"></param>
        public virtual void Initialise (ShipBodyModuleBase bodyModuleBase)
        {
            if (isInitialised || bodyModuleBase == null || !enabled || !gameObject.activeInHierarchy) { return; }

            // Store a reference to the parent module so we can get player character, NPCs etc
            shipBodyModuleBase = bodyModuleBase;

            if (isMakeInteractive)
            {
                MakeInteractive();
            }

            isInitialised = true;
        }

        /// <summary>
        /// Attempt to make the control interactive (requires Sticky3D Controller)
        /// </summary>
        /// <returns></returns>
        public void MakeInteractive()
        {
            if (isInteractive) { return; }

            #if SCSM_S3D                

            if (shipControlModule == null)
            {
                Debug.LogWarning("[ERROR] SSCBaseControl.MakeInteractive() - the control requires the Ship field on the General tab to be populated on " + name + " in " + gameObject.scene.name);
            }
            else if (StickyInteractive.CheckInteractive(gameObject, ref controlInteractive))
            {
                ConfigureInteractive();

                // If it hasn't been initialised, do it now
                controlInteractive.Initialise();
            }

            #else
            Debug.LogWarning("[ERROR] " + name + " in " + gameObject.scene.name + " cannot be made interactive as Sticky3D Controller does not appear to be installed");
            #endif
        }

        #if SCSM_S3D
        /// <summary>
        /// Given an S3DInteractiveTags scriptableoject, refresh the tag using the
        /// interactiveTagName for this object.
        /// </summary>
        /// <param name="interactiveTags"></param>
        public void RefreshInteractiveTag (S3DInteractiveTags interactiveTags)
        {
            if (interactiveTags != null && controlInteractive != null)
            {
                int interactiveTag = interactiveTags.GetMask(interactiveTagName);

                controlInteractive.SetInteractiveTags(interactiveTags);
                controlInteractive.SetInteractiveTag(interactiveTag);
            }
        }
        #endif

        /// <summary>
        /// Gets automatically called when a VR player grabs the control.
        /// Override to take different action when this happens.
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <param name="hitNormal"></param>
        /// <param name="stickyInteractiveID"></param>
        /// <param name="stickyID"></param>
        public virtual void CharacterGrabbed (Vector3 hitPoint, Vector3 hitNormal, int stickyInteractiveID, int stickyID)
        {
            #if SCSM_S3D
            StickyControlModule s3dCharacter;

            if (shipBodyModuleBase.GetCharacter(stickyID, out s3dCharacter))
            {
                bool hasSwitchedPilots = false;

                //Debug.Log("[DEBUG] " + s3dCharacter.name + " started grabbing " + name + " T:" + Time.time);

                if (isFlyShipWhenEngaged)
                {
                    // Check if someone else is flying the ship. They most
                    // likely will be using a different set of controls.
                    if (shipBodyModuleBase.IsOtherFlyingShip(stickyID))
                    {
                        shipBodyModuleBase.SetPilotFlyingShip(stickyID);
                        hasSwitchedPilots = true;
                    }
                }

                if (s3dCharacter.IsAnimateEnabled)
                {
                    s3dCharacter.StopRevertAnimClips();

                    // TODO - Check for pending anim clip replacements.
                    // See CharacterStopTouching(..)
                    //if (s3dPlayerAnimDelay1 != null)
                    //{
                    //    StopCoroutine(s3dPlayerAnimDelay1);
                    //    s3dPlayerAnimDelay1 = null;
                    //}

                    controlInteractive.ApplyObjectAnimSets(s3dCharacter);
                    controlInteractive.ApplyObjectAnimActions(s3dCharacter);
                }

                if (inputBridge != null) { inputBridge.UnPause(); }

                // If required, mark this character as one flying ship in ShipBodyModule
                if (isFlyShipWhenEngaged && !hasSwitchedPilots) { shipBodyModuleBase.SetPilotFlyingShip(s3dCharacter.StickyID); }

            }

            #endif
        }

        /// <summary>
        /// Gets automatically called when a character starts touching the control.
        /// Override to take different action when this happens.
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <param name="hitNormal"></param>
        /// <param name="stickyInteractiveID"></param>
        /// <param name="stickyID"></param>
        public virtual void CharacterStartTouching (Vector3 hitPoint, Vector3 hitNormal, int stickyInteractiveID, int stickyID)
        {
            #if SCSM_S3D
            StickyControlModule s3dCharacter;

            if (shipBodyModuleBase.GetCharacter(stickyID, out s3dCharacter))
            {
                bool hasSwitchedPilots = false;
                if (isFlyShipWhenEngaged)
                {
                    // Check if someone else is flying the ship. They most
                    // likely will be using a different set of controls.
                    if (shipBodyModuleBase.IsOtherFlyingShip(stickyID))
                    {
                        shipBodyModuleBase.SetPilotFlyingShip(stickyID);
                        hasSwitchedPilots = true;
                    }
                }

                if (s3dCharacter.IsAnimateEnabled)
                {
                    s3dCharacter.StopRevertAnimClips();

                    // TODO - Check for pending anim clip replacements.
                    // See CharacterStopTouching(..)
                    //if (s3dPlayerAnimDelay1 != null)
                    //{
                    //    StopCoroutine(s3dPlayerAnimDelay1);
                    //    s3dPlayerAnimDelay1 = null;
                    //}

                    controlInteractive.ApplyObjectAnimSets(s3dCharacter);
                    controlInteractive.ApplyObjectAnimActions(s3dCharacter);
                }

                if (companionControl != null && companionControl.IsInitialised)
                {
                    // Is the right hand of the character reaching for the control?
                    if (s3dCharacter.GetRightHandIKTargetInteractiveID() == controlInteractive.StickyInteractiveID)
                    {
                        // Use the left hand to reach for the companion control
                        s3dCharacter.SetLeftHandIKTargetInteractive(companionControl.controlInteractive, false, true);
                    }
                    else
                    {
                        // S3D character must have reached for this control with the left hand
                        // so reach for companion control with right hand.
                        s3dCharacter.SetRightHandIKTargetInteractive(companionControl.controlInteractive, false, true);
                    }
                }

                if (outputBridge != null) { outputBridge.UnPause(); }

                // If required, mark this character as one flying ship in ShipBodyModule
                if (isFlyShipWhenEngaged && !hasSwitchedPilots) { shipBodyModuleBase.SetPilotFlyingShip(s3dCharacter.StickyID); }
            }
            #endif
        }

        /// <summary>
        /// Gets automatically called when a VR player lets go of the control.
        /// Override to take different action when this happens.
        /// </summary>
        /// <param name="stickyInteractiveID"></param>
        /// <param name="stickyID"></param>
        public virtual void CharacterStopGrabbing (int stickyInteractiveID, int stickyID)
        {
            #if SCSM_S3D
            StickyControlModule s3dCharacter;

            if (shipBodyModuleBase.GetCharacter(stickyID, out s3dCharacter))
            {
                //Debug.Log("[DEBUG] " + s3dCharacter.name + " stopped grabbing " + name + " T:" + Time.time);

                controlInteractive.RevertObjectAnimActions(s3dCharacter);

                // This will revert the animations to default clips with delay if required.
                // Anim Actions based on the Object Anim Set will NOT be reset as not held.
                controlInteractive.RevertObjectAnimSets(s3dCharacter);
            }

            // We always want to pause the inputBridge.
            if (inputBridge != null) { inputBridge.Pause(); }

            // We always want to stop flying the ship if this is the primary control.
            if (isFlyShipWhenEngaged && !shipBodyModuleBase.IsChangingPilot) { shipBodyModuleBase.SetPilotFlyingShip(0); }

            #endif
        }

        /// <summary>
        /// Gets automatically called when a character stops touching the control.
        /// Override to take different action when this happens.
        /// </summary>
        /// <param name="stickyInteractiveID"></param>
        /// <param name="stickyID"></param>
        public virtual void CharacterStopTouching (int stickyInteractiveID, int stickyID)
        {
            #if SCSM_S3D
            StickyControlModule s3dCharacter;

            if (shipBodyModuleBase.GetCharacter(stickyID, out s3dCharacter))
            {
                bool isRHIK = stickyInteractiveID == s3dCharacter.GetRightHandIKTargetInteractiveID();
                bool isLHIK = stickyInteractiveID == s3dCharacter.GetLeftHandIKTargetInteractiveID();

                controlInteractive.RevertObjectAnimActions(s3dCharacter);

                // This will revert the animations to default clips with delay if required.
                // Anim Actions based on the Object Anim Set will NOT be reset as not held.
                controlInteractive.RevertObjectAnimSets(s3dCharacter);

                if (isRHIK)
                {
                    s3dCharacter.SetRightHandIKTargetInteractive(null, false, false);
                }
                else if (isLHIK)
                {
                    s3dCharacter.SetLeftHandIKTargetInteractive(null, false, false);
                }

                //Debug.Log("[DEBUG] " + s3dCharacter.name + " CharacterStopTouching " + (isRHIK ? "RH" : isLHIK ? "LH" : "--"));
                 
                // Check companion's companion is not self
                if (companionControl != null && companionControl.IsInitialised && !IsCompanionDeadlock())
                {
                    // Has the the right hand of the character stopped reaching for the control?
                    // NOTE: As a double check, could see if other control is being reached for.
                    if (isRHIK)
                    {
                        // Stop reaching for the companion control
                        StickyInteractive companionInteractive = s3dCharacter.GetLeftHandIKTargetInteractive();

                        if (companionInteractive != null)
                        {
                            companionControl.CharacterStopTouching(companionInteractive.StickyInteractiveID, stickyID);
                        }
                        else
                        {
                            s3dCharacter.SetLeftHandIKTargetInteractive(null, false, true);
                        }
                    }
                    else
                    {
                        // S3D character must have been reaching for this control with the left hand
                        // so stop reaching for companion control with right hand.
                        // Stop reaching for the companion control
                        StickyInteractive companionInteractive = s3dCharacter.GetRightHandIKTargetInteractive();

                        if (companionInteractive != null)
                        {
                            companionControl.CharacterStopTouching(companionInteractive.StickyInteractiveID, stickyID);
                        }
                        else
                        {
                            s3dCharacter.SetRightHandIKTargetInteractive(null, false, true);
                        }
                    }
                }
            }

            // Even if we don't know which character stopped touching control
            // (maybe they were destroyed), we always want to pause the outputBridge.
            if (outputBridge != null) { outputBridge.Pause(); }

            // We always want to stop flying the ship if this is the primary control.
            if (isFlyShipWhenEngaged && !shipBodyModuleBase.IsChangingPilot) { shipBodyModuleBase.SetPilotFlyingShip(0); }
            #endif
        }

        #if SCSM_S3D
        /// <summary>
        /// Attempt to hook up a listener for the popup menu.
        /// USAGE: SSCJoystickControl.AddListeners(joystickControl, taskModulePlayer);
        /// </summary>
        /// <param name="baseControl"></param>
        /// <param name="taskModulePlayer"></param>
        public static void AddListeners (SSCBaseControl baseControl, StickyTaskModule taskModulePlayer)
        {
            if (baseControl != null && taskModulePlayer != null && baseControl.controlInteractive != null)
            {
                // Hook up a listener for the popup menu
                // See also OnDestroy() which removes the listeners
                if (baseControl.controlInteractive.onHoverEnter == null) { baseControl.controlInteractive.onHoverEnter = new S3DInteractiveEvt1(); }
                baseControl.controlInteractive.onHoverEnter.AddListener(delegate { taskModulePlayer.ShowPopup(baseControl.controlInteractive); });
            }
        }
        #endif

        /// <summary>
        /// Remove any non-persistent listeners from events
        /// </summary>
        public virtual void RemoveListeners()
        {
            #if SCSM_S3D
            if (controlInteractive != null)
            {
                controlInteractive.RemoveListeners();
            }
            #endif
        }

        /// <summary>
        /// Attempt to set the VR mode of this control
        /// </summary>
        /// <param name="newValue"></param>
        public virtual void SetVRMode (bool newValue)
        {
            #if SCSM_XR && SSC_UIS
            isVRMode = newValue;
            #else
            isVRMode = false;
            #endif
        }

        #endregion
    }
}