using System;
using BepInEx.Configuration;
using FistVR;
using UnityEngine;
namespace H3VRMod
{
    public class Hooks
    {
        private readonly ConfigEntry<bool> _vanillaMode;
        private readonly ConfigEntry<bool> _stunOnGrab;
        private readonly ConfigEntry<int> _stunDuration;
        private readonly ConfigEntry<bool> _makeHandsMelee;
        private readonly ConfigFile _config;
        
        

        public Hooks(ConfigEntry<bool> vanillaMode, ConfigEntry<bool> stunOnGrab, ConfigEntry<int> stunDuration,
            ConfigEntry<bool> makeHandsMelee)
        {
            _vanillaMode = vanillaMode;
            _stunOnGrab = stunOnGrab;
            _stunDuration = stunDuration;
            _makeHandsMelee = makeHandsMelee;
        }

        private bool HookCanCurrentlyBeHeld(On.FistVR.Sosig.orig_CanCurrentlyBeHeld orig, FistVR.Sosig self)
        {
            if (_vanillaMode.Value)
            {
                if (self.CanBeGrabbed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void Hook()
        {
            On.FistVR.Sosig.CanCurrentlyBeHeld += HookCanCurrentlyBeHeld;
            On.FistVR.SosigLink.Update += OnSosigLinkOnUpdate;
        }

        private void OnSosigLinkOnUpdate(On.FistVR.SosigLink.orig_Update orig, FistVR.SosigLink self)
        {
            if (self.O.IsHeld)
                if (_stunOnGrab.Value)
                    if (_vanillaMode.Value)
                    {
                        self.S.Stun(Mathf.Clamp(_stunDuration.Value * self.S.StunMultiplier, -1, self.S.m_maxStunTime));
                    }
                    else
                    {
                        // This is not great, but its already counting on ppl not using vanillaMode, so like, whatever.
                        if(self.S.m_maxStunTime<_stunDuration.Value)
                            self.S.m_maxStunTime = _stunDuration.Value;
                        
                        self.S.Stun(_stunDuration.Value);
                    }

            orig(self);
        }

        public void Unhook()
        {
            On.FistVR.Sosig.CanCurrentlyBeHeld -= HookCanCurrentlyBeHeld;
        }
    }
}