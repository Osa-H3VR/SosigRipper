using BepInEx;
using BepInEx.Configuration;

namespace H3VRMod
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class Plugin : BaseUnityPlugin
	{
		private readonly Hooks _hooks;

		public Plugin()
		{
			var vanillaMode = Config.Bind("SosigRipper", "vanillaMode", true, "Setting this to false will ignore all sosig settings, you will be able to grab even if sosig has canBeGrabbed=false, he will get stunned always the same time (disregarding StunMultiplier). This will make some sosigs that rely on those trival to kill.");
			var stunOnGrab = Config.Bind("SosigRipper", "stunOnGrab", true, "Should sosigs get stunned on grab?");
			var stunDuration = Config.Bind("SosigRipper", "stunDuration", 3000, "How long will the stun last, after letting go. No effect is stunOnGrab:false!");
			var makeHandsMelee = Config.Bind("SosigRipper", "makeHandsMelee", false, "TODO!");
			_hooks = new Hooks(vanillaMode, stunOnGrab, stunDuration, makeHandsMelee);
			_hooks.Hook();
		}

		private void Awake()
		{

		}

		private void Update()
		{

		}

		private void OnDestroy()
		{
			_hooks.Unhook();
		}
	}
}