using System;
using BepInEx;
using BepInEx.Configuration;
using FistVR;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

namespace H3VRMod
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class Plugin : BaseUnityPlugin
	{
		private readonly Hooks _hooks;

		public Plugin()
		{
			Logger.LogDebug("Initialised");
			var vanillaMode = Config.Bind("SosigRipper", "vanillaMode", true, "Setting this to false will ignore all sosig settings, you will be able to grab even if sosig has canBeGrabbed=false, he will get stunned always the same time (disregarding StunMultiplier). This will make some sosigs that rely on those trival to kill.");
			var stunOnGrab = Config.Bind("SosigRipper", "stunOnGrab", true, "Should sosigs get stunned on grab?");
			var stunDuration = Config.Bind("SosigRipper", "stunDuration", 3000, "How long will the stun last, after letting go. No effect is stunOnGrab:false!");
			var makeHandsMelee = Config.Bind("SosigRipper", "makeHandsMelee", false, "TODO!");
			_hooks = new Hooks(vanillaMode, stunOnGrab, stunDuration, makeHandsMelee, Logger);
			_hooks.Hook();
			EnableDebugSpheres = true;
		}

		private void Awake()
		{

		}

		private void OnDestroy()
		{
			_hooks.Unhook();
		}
		
		 // YOINKED for ntoolbox
        
        public const float HAND_SIZE = 0.045f;
        public bool EnableDebugSpheres { get; set; }
        
        private void Update()
        {
            // try
            // {
            //     //Hand collider interaction
            //     //Disable/enable based on Grip interaction
            //     if (LeftHandComp.m_state.Equals(FVRViveHand.HandState.GripInteracting))
            //         LeftCollider.SetActive(false);
            //     else if (!LeftCollider.activeSelf)
            //         LeftCollider.SetActive(true);
            //
            //     if (RightHandComp.m_state.Equals(FVRViveHand.HandState.GripInteracting))
            //         RightCollider.SetActive(false);
            //     else if (!RightCollider.activeSelf)
            //         RightCollider.SetActive(true);
            //
            //     //Show/hide spheres
            //     if (LeftCollider.activeSelf && LeftCollider.transform.parent != null && EnableDebugSpheres)
            //         Gizmos.Sphere(GM.CurrentPlayerBody.LeftHand.position, HAND_SIZE, Color.red);
            //     if (RightCollider.activeSelf && RightCollider.transform.parent != null && EnableDebugSpheres)
            //         Gizmos.Sphere(GM.CurrentPlayerBody.RightHand.position, HAND_SIZE, Color.blue);
            // } catch (NullReferenceException e)
            // {
            //     Debug.Log("---NToolbox caught null reference exception---");
            //     Debug.Log("(If you are seeing this, report it to the mod author)");
            //     Debug.Log("LeftHandComp: " + LeftHandComp != null ? "not null" : "null");
            //     Debug.Log("Actions.LeftCollider: " + LeftCollider != null ? "not null" : "null");
            //     Debug.Log("Error: " + e);
            //     Debug.Log("Attempting to reset...");
            //     ResetHandObjects();
            // }
        }
        

        public static void SetColliderObjects()
        {
            LeftCollider = GetColliderObject();
            RightCollider = GetColliderObject();
        }
        
        private FVRViveHand LeftHandComp = new FVRViveHand();
        private FVRViveHand RightHandComp = new FVRViveHand();
        private void ResetHandObjects()
        {
            LeftHandComp = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            RightHandComp = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();
            SetColliderObjects();
        }
        
        public static GameObject LeftCollider = GetColliderObject();
        public static GameObject RightCollider = GetColliderObject();
        
        public static void ToggleHandCollision()
        {
            LeftCollider.SetActive(!LeftCollider.activeSelf);
            LeftCollider.transform.SetParent(LeftCollider.transform.parent == null ? GM.CurrentPlayerBody.LeftHand : null, false);
            RightCollider.SetActive(!RightCollider.activeSelf);
            RightCollider.transform.SetParent(RightCollider.transform.parent == null ? GM.CurrentPlayerBody.RightHand : null, false);
        }
        
        private static GameObject GetColliderObject()
        {
            GameObject obj = new GameObject();
            obj.SetActive(false);
            var collider = obj.AddComponent<SphereCollider>();
            var rigid = obj.AddComponent<Rigidbody>();
            rigid.isKinematic = true;
            collider.radius = HAND_SIZE;
            obj.transform.position = new Vector3(0f, 0f, 0f);
            return obj;
        }
	}
}