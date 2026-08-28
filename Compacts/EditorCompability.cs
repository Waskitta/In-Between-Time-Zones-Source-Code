using BaldiPlusRandomZone.CustomCharacters;
using BaldiPlusRandomZone.Extensions;
using MonoMod.Utils;
using MTM101BaldAPI.AssetTools;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusLevelStudio.Editor.Tools;
using PlusStudioLevelLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.Compacts
{
    public static class EditorCompability
    {
        public static void LoadCompact()
        {
            AddNPC("bussinesman", "npc_bussinessman", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "BussinesMan").replacement);
            AddNPC("educatedtime", "npc_educatedtime", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "EducatedTime").replacement);
            AddNPC("classicsweep", "npc_classicsweep", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "ClassicSweep").replacement);
            AddNPC("caffeinepomp", "npc_caffinatedpomp", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "CaffeinePomp").replacement);
            AddNPC("goodguy", "npc_goodguy", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "GoodGuy").replacement);
            AddNPC("cloudya", "npc_cloudya", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "Cloudya").replacement);
            AddNPC("coolbeans", "npc_coolbeans", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "CoolBeans").replacement);
            AddNPC("chalkdemon", "npc_demonchalk", CharacterCreator.replacementCharacters.FirstOrDefault(x => x.name == "Chalk-Demon").replacement);

            foreach (var textureSet in RoomTexturesHandler.textures.Values)
            {
                foreach (var pair in textureSet.walls)
                {
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add(pair.selection.name.ToLower(), pair.selection);
                    LevelStudioPlugin.Instance.selectableTextures.Add(pair.selection.name.ToLower());
                }
                foreach (var pair in textureSet.floors)
                {
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add(pair.selection.name.ToLower(), pair.selection);
                    LevelStudioPlugin.Instance.selectableTextures.Add(pair.selection.name.ToLower());
                }
                foreach (var pair in textureSet.ceiling)
                {
                    LevelLoaderPlugin.Instance.roomTextureAliases.Add(pair.selection.name.ToLower(), pair.selection);
                    LevelStudioPlugin.Instance.selectableTextures.Add(pair.selection.name.ToLower());
                }
            }

            EditorInterfaceModes.AddModeCallback((mode, vanilla) =>
            {
                EditorInterfaceModes.AddToolsToCategory(mode, "npcs", npcs.Select(x => new NPCTool(x.name, x.icon)));
                EditorInterfaceModes.AddToolsToCategory(mode, "posters", npcs.Select(x => new PosterTool(x.name + "_officeposter")));
            });
        }


        public static void AddNPC(string name, string iconName, NPC npc)
        {
            if (npc.Character != Character.Chalkles)
                EditorInterface.AddNPCVisual(name, npc);
            else
            {
                GameObject chalklesVisual = EditorInterface.AddNPCVisual(name, npc);
                chalklesVisual.transform.Find("SpriteBase").Find("Sprite").gameObject.SetActive(true);
                chalklesVisual.GetComponent<EditorRendererContainer>().AddRenderer(chalklesVisual.GetComponentInChildren<Renderer>(), "none");
            }

            LevelLoaderPlugin.Instance.npcAliases.Add(name, npc);
            LevelLoaderPlugin.Instance.posterAliases.Add(name + "_officeposter", npc.Poster);
            npcs.Add((npc, name, AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 1f, "CustomCharacters", iconName + ".png")));
        }

        public static List<(NPC npc, string name, Sprite icon)> npcs = new List<(NPC npc, string name, Sprite icon)>();
    }
}
