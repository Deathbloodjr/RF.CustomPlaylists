using HarmonyLib;
using Il2CppSystem.Xml.Serialization;
using Scripts.OutGame.SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPlaylists.Plugins
{
    internal class XmlSavePatch
    {
        [HarmonyPatch(typeof(EnumMap))]
        [HarmonyPatch(nameof(EnumMap.GetXmlName))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnumMap_GetXmlName_Prefix(ref string __result, string typeName, Il2CppSystem.Object enumValue)
        {
            if (typeName == "Scripts.OutGame.SongSelect.FilterTypes")
            {
                FilterTypes value = enumValue.Unbox<FilterTypes>();
                __result = value.ToString();
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(XmlSerializationReaderInterpreter))]
        [HarmonyPatch(nameof(XmlSerializationReaderInterpreter.GetEnumValue))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool XmlSerializationReaderInterpreter_GetEnumValue_Prefix(ref Il2CppSystem.Object __result, XmlTypeMapping typeMap, string val)
        {
            if (typeMap.ElementName == "FilterTypes")
            {
                if (int.TryParse(val, out int num))
                {
                    Il2CppSystem.Int32 newNum = new Il2CppSystem.Int32();
                    unsafe
                    {
                        *&newNum.m_value = num;
                    }
                    __result = newNum.BoxIl2CppObject();
                    return false;
                }
            }

            return true;
        }
    }
}
