using System;
using BepInEx.Configuration;
using BepInEx.Logging;
using FistVR;
using UnityEngine;
using Logger = UnityEngine.Logger;

namespace H3VRMod
{
    public class Hooks
    {
        private readonly ConfigEntry<bool> _vanillaMode;
        private readonly ConfigEntry<bool> _stunOnGrab;
        private readonly ConfigEntry<int> _stunDuration;
        private readonly ConfigEntry<bool> _makeHandsMelee;
        private readonly ManualLogSource _manualLogSource;
        private readonly ConfigFile _config;
        
        

        public Hooks(ConfigEntry<bool> vanillaMode, ConfigEntry<bool> stunOnGrab, ConfigEntry<int> stunDuration,
            ConfigEntry<bool> makeHandsMelee, ManualLogSource manualLogSource)
        {
            _vanillaMode = vanillaMode;
            _stunOnGrab = stunOnGrab;
            _stunDuration = stunDuration;
            _makeHandsMelee = makeHandsMelee;
            _manualLogSource = manualLogSource;
        }

        private bool HookCanCurrentlyBeHeld(On.FistVR.Sosig.orig_CanCurrentlyBeHeld orig, FistVR.Sosig self)
        {
            if (_vanillaMode.Value)
            {
                if (self.CanBeGrabbed)
                {
                    _manualLogSource.LogDebug("Vanilla can be grabbed");
                    return true;
                }
                else
                {
                    _manualLogSource.LogDebug("Vanilla cant be grabbed");
                    return false;
                }
            }
            else
            {
                _manualLogSource.LogDebug("Not vanilla cant be grabbed");
                return true;
            }
        }

        public void Hook()
        {
            On.FistVR.Sosig.CanCurrentlyBeHeld += HookCanCurrentlyBeHeld;
            On.FistVR.SosigLink.Update += OnSosigLinkOnUpdate;
            _manualLogSource.LogDebug("Hooked");
        }

        private void OnSosigLinkOnUpdate(On.FistVR.SosigLink.orig_Update orig, FistVR.SosigLink self)
        {
            if (self.O.IsHeld)
            {
                _manualLogSource.LogDebug("Grabbed");

                if (_stunOnGrab.Value)
                {
                    _manualLogSource.LogDebug("Stun on grabbed");

                    if (_vanillaMode.Value)
                    {
                        _manualLogSource.LogDebug("Stun on grabbed vanilla");
                        self.S.Stun(Mathf.Clamp(_stunDuration.Value * self.S.StunMultiplier, -1, self.S.m_maxStunTime));
                    }
                    else
                    {
                        _manualLogSource.LogDebug("Stun on grabbed not vanilla");
                        // This is not great, but its already counting on ppl not using vanillaMode, so like, whatever.
                        if (self.S.m_maxStunTime < _stunDuration.Value)
                            self.S.m_maxStunTime = _stunDuration.Value;

                        self.S.Stun(_stunDuration.Value);
                    }
                }
            }

            orig(self);
        }

        public void Unhook()
        {
            On.FistVR.Sosig.CanCurrentlyBeHeld -= HookCanCurrentlyBeHeld;
            _manualLogSource.LogDebug("Unhooked");
        }
    }
}