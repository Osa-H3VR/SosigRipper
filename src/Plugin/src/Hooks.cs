using System;
using On.FistVR;

namespace H3VRMod
{
	public class Hooks
	{
		private readonly Sosig.hook_CanCurrentlyBeHeld _override =(orig, self) => true;
		public void Hook()
		{
			On.FistVR.Sosig.CanCurrentlyBeHeld += _override;
		}

		public void Unhook()
		{
			On.FistVR.Sosig.CanCurrentlyBeHeld -= _override;
		}
	}
}