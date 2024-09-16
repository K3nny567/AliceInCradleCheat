using BepInEx.Configuration;
using HarmonyLib;
using nel;

namespace AliceInCradleCheat
{
    // ##############################
    // Lock basic status
    // ##############################
    public class LockStatus : BasePatchClass
    {
        private static ConfigEntry<bool> hp_switch_def;
        private static ConfigEntry<int> hp_def;
        private static ConfigEntry<bool> mp_switch_def;
        private static ConfigEntry<int> mp_def;
        private static ConfigEntry<bool> ep_switch_def;
        private static ConfigEntry<int> ep_def;
        public LockStatus()
        {
            string section = "BasicStatus";
            hp_switch_def = TrackBindConfig(section, "HPLockSwitch", false);
            hp_def = TrackBindConfig(section, "HP", 100, new AcceptableValueRange<int>(0, 100), false, true);
            mp_switch_def = TrackBindConfig(section, "MPLockSwitch", false);
            mp_def = TrackBindConfig(section, "MP", 100, new AcceptableValueRange<int>(0, 100), false, true);
            section = "PervertFunctions";
            ep_switch_def = TrackBindConfig(section, "EPLockSwitch", false);
            ep_def = TrackBindConfig(section, "EP", 0, new AcceptableValueRange<int>(0, 1000));
            TryPatch(GetType());
        }
        [HarmonyPostfix, HarmonyPatch(typeof(SceneGame), "runIRD")]
        private static void PatchContent()
        {
            PRNoel noel = MainReference.GetNoel();
            if (noel == null)
            {
                return;
            }
            if (hp_switch_def.Value)
            {
                int max_hp = (int)noel.get_maxhp();
                int set_hp = hp_def.Value * max_hp / 100;
                Traverse.Create(noel).Field("hp").SetValue(set_hp);
                if (noel.UP != null && noel.UP.isActive())
                {
                    UIStatus.Instance.fineHpRatio(true, false);
                }
            }
            if (mp_switch_def.Value)
            {
                int max_mp = (int)noel.get_maxmp();
                int set_mp = mp_def.Value * max_mp / 100;
                max_mp -= noel.EggCon.total;
                set_mp = set_mp < max_mp ? set_mp : max_mp;
                Traverse.Create(noel).Field("mp").SetValue(set_mp);
                if (noel.UP != null && noel.UP.isActive())
                {
                    UIStatus.Instance.fineMpRatio(true, false);
                }
            }
            if (ep_switch_def.Value)
            {
                noel.ep = ep_def.Value;
                noel.EpCon.fineCounter();
            }
        }
    }
}
