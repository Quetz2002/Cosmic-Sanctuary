using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SCSM_S3D
using scsmmedia;
#endif

// Sci-Fi Ship Controller. Copyright (c) 2018-2025 SCSM Pty Ltd. All rights reserved.
namespace SciFiShipController
{
    /// <summary>
    /// This is the base script used to manage ship body components
    /// used with the Sticky3D Controller (S3D) asset.
    /// This can include S3D characters and interactive objects like
    /// doors, consoles, seats, sockets etc.
    /// </summary>
    [HelpURL("https://scsmmedia.com/ssc-documentation")]
    public class ShipBodyModuleBase : MonoBehaviour
    {
        #region Public Variables

        #endregion

        #region Public Properties

        /// <summary>
        /// Does the ship have a Sticky3D player associated with the ship?
        /// </summary>
        #if SCSM_S3D
        public bool HasStickyPlayer { get => isInitialised ? hasS3DPlayer && playerS3DComponents != null && playerS3DComponents.stickyControlModule != null : playerS3DComponents != null && playerS3DComponents.stickyControlModule != null; }
        #else
        public bool HasStickyPlayer { get => false; }
        #endif

        // <summary>
        /// Is the pilot currently being changed? Use this to prevent trying
        /// to change the pilot (or clear it) while another method is changing
        /// the pilot.
        /// </summary>
        public bool IsChangingPilot => isChangingPilot;

        public bool IsInitialised { get { return isInitialised; } }

        #if SCSM_S3D
        /// <summary>
        /// Is the Sticky3D character the PilotController and flying the ship?
        /// </summary>
        public bool IsPlayerFlyingShip { get { return pilotFlyingShipID != 0 && playerS3DComponents != null && playerS3DComponents.StickyID == pilotFlyingShipID; } }
        #endif

        #if SCSM_S3D
        /// <summary>
        /// Get a list of the NPCs and components that can interact with this ship
        /// </summary>
        public List<S3DComponents> NPCList { get { return npcList; } }
        #endif

        #if SCSM_S3D
        /// <summary>
        /// Get the number of Sticky3D NPCs assigned to the ship
        /// </summary>
        public int NumberOfNPCs { get { return npcList == null ? 0 : npcList.Count; } }
        #endif

        #if SCSM_S3D
        /// <summary>
        /// Used when Sticky3D Controller is in project, this is player Sticky3D character from the scene.
        /// </summary>
        public StickyControlModule PlayerCharacter { get { return hasS3DPlayer ? playerS3DComponents.stickyControlModule : null; } set { SetPlayerCharacter(value); } }
        #endif

        #if SCSM_S3D
        public int PlayerCharacterID { get { return hasS3DPlayer ? playerS3DComponents.StickyID : 0; } }
        #else
        public int PlayerCharacterID { get { return 0; } }
        #endif

        #if SCSM_S3D
        /// <summary>
        /// Attempt to get the StickyTaskModule attached to the Sticky3D player character
        /// </summary>
        public StickyTaskModule PlayerTaskModule { get { return hasS3DPlayer && playerS3DComponents.HasTaskModule ? playerS3DComponents.S3DTaskModule : null; } }
        #endif

        #if SCSM_S3D
        /// <summary>
        /// The unique ID of the pilot character currently flying the ship. It could be the PilotController (player) or an NPC.
        /// </summary>
        public int PilotFlyingShipID { get { return pilotFlyingShipID; } set { SetPilotFlyingShip(value); } }
        #endif

        #endregion

        #region Public Static Variables

        #endregion

        #region Protected Variables - Sticky3D

        [System.NonSerialized] protected bool hasS3DPlayer = false;
        [System.NonSerialized] protected bool isInitialised = false;
        [System.NonSerialized] protected bool isStickyPlayerActive = false;

        /// <summary>
        /// Is the pilot currently being changed? Use this to prevent trying
        /// to change the pilot (or clear it) while another method is changing
        /// the pilot.
        /// </summary>
        [System.NonSerialized] protected bool isChangingPilot = false;

        /// <summary>
        /// The unique ID of the pilot character currently flying the ship.
        /// It could be the PilotController or an NPC.
        /// </summary>
        [System.NonSerialized] protected int pilotFlyingShipID = 0;

        #if SCSM_S3D
        /// <summary>
        /// Used when Sticky3D Controller is in project, this is the player Sticky3D character from the scene.
        /// </summary>
        [SerializeField] protected S3DComponents playerS3DComponents = new S3DComponents();

        /// <summary>
        /// Used when Sticky3D Controller is in the project, this is the list of Sticky3D NPCs assigned to the ship from the scene
        /// </summary>
        [SerializeField] protected List<S3DComponents> npcList = new List<S3DComponents>();
        #endif

        #endregion

        #region Public Delegates

        #endregion

        #region Protected Initialise Methods

        #endregion

        #region Events

        #endregion

        #region Protected Methods - Sticky3D Controller
        #if SCSM_S3D

        /// <summary>
        /// Override this to take action when a player or NPC is added to the ship
        /// </summary>
        /// <param name="s3dComponents"></param>
        /// <param name="isPlayer"></param>
        protected virtual void AddedCharacter(S3DComponents s3dComponents, bool isPlayer)
        {
            
        }


        protected void RefreshSticky3DPlayer()
        {
            if (playerS3DComponents == null) { playerS3DComponents = new S3DComponents(); }

            hasS3DPlayer = playerS3DComponents != null && playerS3DComponents.stickyControlModule != null;

            if (hasS3DPlayer)
            {
                playerS3DComponents.Refresh(playerS3DComponents.stickyControlModule);
            }
            else
            {
                playerS3DComponents.SetDefaults();
            }
        }

        /// <summary>
        /// Override this to take action when a player or NPC is removed from the ship.
        /// This can also be called when the character is destroyed.
        /// </summary>
        /// <param name="s3dComponents"></param>
        /// <param name="isPlayer"></param>
        protected virtual void RemovedCharacter(S3DComponents s3dComponents, bool isPlayer)
        {
            
        }

        #endif
        #endregion

        #region Public API Methods - Sticky3D Controller
        #if SCSM_S3D

        /// <summary>
        /// Attempt to add the Sticky3D character to the list
        /// of NPCs.
        /// </summary>
        /// <param name="s3dNPC"></param>
        /// <returns></returns>
        public bool AddNPC (StickyControlModule s3dNPC)
        {
            bool wasAdded = false;

            if (s3dNPC != null && !HasNPC(s3dNPC.StickyID))
            {
                S3DComponents s3dComponents = new S3DComponents(s3dNPC);

                npcList.Add(s3dComponents);
                wasAdded = true;

                AddedCharacter(s3dComponents, false);
            }

            return wasAdded;
        }

        /// <summary>
        /// Attempt to get a Sticky3D that can engage with this ship
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public StickyControlModule GetCharacter (int stickyID)
        {
            StickyControlModule s3dCharacter;
            if (GetCharacter(stickyID, out s3dCharacter)) { }
            return s3dCharacter;
        }

        /// <summary>
        /// Attempt to get a Sticky3D with components that can engage with this ship
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public S3DComponents GetCharacterComponents (int stickyID)
        {
            S3DComponents s3dCharacter;
            if (GetCharacterComponents(stickyID, out s3dCharacter)) { }
            return s3dCharacter;
        }

        /// <summary>
        /// Attempt to get a Sticky3D that can engage with this ship
        /// </summary>
        /// <param name="stickyID"></param>
        /// <param name="s3dCharacter"></param>
        /// <returns></returns>
        public bool GetCharacter (int stickyID, out StickyControlModule s3dCharacter)
        {
            s3dCharacter = null;
            bool isFound = false;

            if (hasS3DPlayer && playerS3DComponents.StickyID == stickyID)
            {
                s3dCharacter = playerS3DComponents.stickyControlModule;
                isFound = true;
            }
            else
            {
                int numNPCs = npcList.Count;

                // Loop backward so we can cleanup null or missing entries as we go
                for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
                {
                    S3DComponents npc = npcList[npcIdx];

                    // If we found an empty slot, remove it
                    if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                    else if (stickyID == npc.StickyID)
                    {
                        s3dCharacter = npc.stickyControlModule;
                        isFound = true;
                        break;
                    }
                }
            }

            return isFound;
        }

        /// <summary>
        /// Attempt to get a Sticky3D (and components) that can engage with this ship
        /// </summary>
        /// <param name="stickyID"></param>
        /// <param name="s3dComponents"></param>
        /// <returns></returns>
        public bool GetCharacterComponents (int stickyID, out S3DComponents s3dComponents)
        {
            s3dComponents = null;
            bool isFound = false;

            if (hasS3DPlayer && playerS3DComponents.StickyID == stickyID)
            {
                s3dComponents = playerS3DComponents;
                isFound = true;
            }
            else
            {
                int numNPCs = npcList.Count;

                // Loop backward so we can cleanup null or missing entries as we go
                for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
                {
                    S3DComponents npc = npcList[npcIdx];

                    // If we found an empty slot, remove it
                    if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                    else if (stickyID == npc.StickyID)
                    {
                        s3dComponents = npc;
                        isFound = true;
                        break;
                    }
                }
            }

            return isFound;
        }

        /// <summary>
        /// Attempt to get a Sticky3D (and components) that can engage with this ship
        /// </summary>
        /// <param name="modelID"></param>
        /// <param name="s3dComponents"></param>
        /// <returns></returns>
        public bool GetCharacterComponentsByModelID (int modelID, out S3DComponents s3dComponents)
        {
            s3dComponents = null;
            bool isFound = false;

            if (hasS3DPlayer && playerS3DComponents.stickyControlModule != null && playerS3DComponents.stickyControlModule.modelId == modelID)
            {
                s3dComponents = playerS3DComponents;
                isFound = true;
            }
            else
            {
                int numNPCs = npcList.Count;

                // Loop backward so we can cleanup null or missing entries as we go
                for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
                {
                    S3DComponents npc = npcList[npcIdx];

                    // If we found an empty slot, remove it
                    if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                    else if (npc.stickyControlModule.modelId == modelID)
                    {
                        s3dComponents = npc;
                        isFound = true;
                        break;
                    }
                }
            }

            return isFound;
        }

        /// <summary>
        /// Get the Sticky3D character (and components) for the pilot (if any) that
        /// is flying the ship.
        /// </summary>
        /// <param name="s3dComponents"></param>
        /// <returns></returns>
        public bool GetPilotComponentsFlyingShip (out S3DComponents s3dComponents)
        {
            bool isCharacterFlyingShip = false;
            s3dComponents = null;

            if (pilotFlyingShipID == 0)
            {
                // no character is flying the ship
            }
            else if (IsPlayerFlyingShip)
            {
                s3dComponents = playerS3DComponents;
            }
            else
            {
                int numNPCs = npcList.Count;

                // Loop backward so we can cleanup null or missing entries as we go
                for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
                {
                    S3DComponents npc = npcList[npcIdx];

                    // If we found an empty slot, remove it
                    if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                    else if (pilotFlyingShipID == npc.StickyID)
                    {
                        s3dComponents = npc;
                        isCharacterFlyingShip = true;
                        break;
                    }
                }
            }

            return isCharacterFlyingShip;
        }

        /// <summary>
        /// Attempt to get the player Sticky3D character associated with this ship.
        /// NOTE: They may not actually be flying the ship at this time.
        /// See also IsPlayerFlyingShip.
        /// </summary>
        /// <param name="s3dCharacter"></param>
        /// <returns></returns>
        public bool GetPlayerCharacterS3D (out StickyControlModule s3dCharacter)
        {
            s3dCharacter = PlayerCharacter;
            return s3dCharacter != null;
        }

        /// <summary>
        /// Attempt to get the player Sticky3D components that can engage with this ship
        /// NOTE: They may not actually be flying the ship at this time.
        /// See also IsPlayerFlyingShip.
        /// </summary>
        /// <param name="s3dComponents"></param>
        /// <returns></returns>
        public bool GetPlayerComponents (out S3DComponents s3dComponents)
        {
            s3dComponents = hasS3DPlayer ? playerS3DComponents : null;
            return playerS3DComponents != null;
        }

        /// <summary>
        /// Is an NPC allocated to this ship?
        /// USAGE: if (HasNPC(mySticky3DNPC.StickyId)) { }
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public bool HasNPC (int stickyID)
        {
            bool isFound = false;
            int numNPCs = npcList.Count;

            // Loop backward so we can cleanup null or missing entries as we go
            for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
            {
                S3DComponents npc = npcList[npcIdx];

                // If we found an empty slot, remove it
                if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                else if (stickyID == npc.StickyID)
                {
                    isFound = true;
                    break;
                }
            }

            return isFound;
        }

        /// <summary>
        /// Is an NPC allocated to this ship?
        /// USAGE: if (HasNPC(mySticky3DNPC)) { }
        /// </summary>
        /// <param name="s3dNPC"></param>
        /// <returns></returns>
        public bool HasNPC (StickyControlModule s3dNPC)
        {
            if (s3dNPC != null)
            {
                return HasNPC(s3dNPC.StickyID);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is the Sticky3D character flying the ship?
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public bool IsFlyingShip (int stickyID)
        {
            return pilotFlyingShipID != 0 && pilotFlyingShipID == stickyID;
        }

        /// <summary>
        /// Is a different Sticky3D character currently flying the ship?
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public bool IsOtherFlyingShip (int stickyID)
        {
            return pilotFlyingShipID != 0 && pilotFlyingShipID != stickyID;
        }

        /// <summary>
        /// Does this StickyID match the player Sticky3D character?
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns></returns>
        public bool IsPlayerCharacter (int stickyID)
        {
            return isInitialised && stickyID != 0 && playerS3DComponents != null && playerS3DComponents.StickyID == stickyID;
        }

        /// <summary>
        /// You could call this if suddenly moving the ship a significant distance while characters assigned
        /// to the ship are using (Hand) IK to touch interactive objects within the ship.
        /// The characters might loose touch with those objects because LateUpdate() will run before
        /// the next FixedUpdate() is able to run on each character and sync their positions with
        /// the (ship) reference frame.
        /// </summary>
        public void RefreshCharacterPositions()
        {
            // Prevent any characters that are touching objects on the ship
            // with Hand IK from loosing touch with them after the sudden
            // ship position change.

            if (isInitialised)
            {
                if (hasS3DPlayer && playerS3DComponents != null && playerS3DComponents.stickyControlModule != null)
                {
                    playerS3DComponents.stickyControlModule.RefreshPosition();
                }

                int numNPCs = npcList.Count;

                // Loop backward so we can cleanup null or missing entries as we go
                for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
                {
                    S3DComponents npc = npcList[npcIdx];

                    // If we found an empty slot, remove it
                    if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                    else
                    {
                        npc.stickyControlModule.RefreshPosition();
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to remove a NPC assigned to this ship
        /// </summary>
        /// <param name="stickyID"></param>
        /// <returns>True if found and removed</returns>
        public bool RemoveNPC (int stickyID)
        {
            bool wasRemoved = false;
            int numNPCs = npcList.Count;

            // Loop backward so we can cleanup null or missing entries as we go
            for (int npcIdx = numNPCs - 1; npcIdx >= 0; npcIdx--)
            {
                S3DComponents npc = npcList[npcIdx];

                // If we found an empty slot, remove it
                if (npc == null || npc.stickyControlModule == null) { npcList.RemoveAt(npcIdx); }
                else if (stickyID == npc.stickyControlModule.StickyID)
                {
                    wasRemoved = true;
                    break;
                }
            }

            return wasRemoved;
        }

        /// <summary>
        /// Attempt to remove an NPC which was assigned to this ship
        /// </summary>
        /// <param name="s3dNPC"></param>
        /// <returns>True if found and removed</returns>
        public bool RemoveNPC (StickyControlModule s3dNPC)
        {
            if (s3dNPC != null)
            {
                return RemoveNPC(s3dNPC.StickyID);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to update the ship with a Sticky3D player character that will interact with the ship.
        /// Used with Sticky3D Controller if installed in the project. 
        /// </summary>
        /// <param name="newPilotController"></param>
        public virtual void SetPlayerCharacter (StickyControlModule newCharacter)
        {
            int oldStickyID = PlayerCharacterID;

            playerS3DComponents.stickyControlModule = newCharacter;
          
            if (isInitialised)
            {
                RefreshSticky3DPlayer();

                // Has a different player been set for the ship?
                if (hasS3DPlayer && PlayerCharacterID != oldStickyID)
                {
                    AddedCharacter(playerS3DComponents, true);
                }

                /// TODO - If we changed pilots we may wish to remove non-persistent listeners from
                /// interactive objects. E.g., seats and beds
            }
        }

        /// <summary>
        /// Set the StickyID of the character flying the ship
        /// </summary>
        /// <param name="newPilotID"></param>
        public virtual void SetPilotFlyingShip (int newPilotID)
        {
            pilotFlyingShipID = newPilotID;
        }

        #endif

        #endregion
    }
}