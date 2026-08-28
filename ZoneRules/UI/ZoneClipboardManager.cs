using BaldiPlusRandomZone.EndlessSupport;
using BaldiPlusRandomZone.PitStop;
using BaldiPlusRandomZone.WeightedStuff;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldiPlusRandomZone.ZoneRules.UI
{
    public class ZoneClipboardManager : MonoBehaviour
    {
        private void Awake()
        {
            CursorController.Instance.transform.SetSiblingIndex(CursorController.Instance.transform.parent.childCount);
        }

        public void OnOpenScreen()
        {
            SetZoneRules();
            for (int i = 0; i < chosenZoneRules.Count; i++)
            {
                papers[i].color = GetColor(chosenZoneRules[i].type);
                buttons[i].image.sprite = Plugin.assetMan.Get<Sprite>("Drawing_" + chosenZoneRules[i].category.ToString());
            }

            ZonePitStopManager.SetSilenceMusic(true);
        }

        public void OnHoverSticker(StandardMenuButton button)
        {
            int index = buttons.IndexOf(button);
            descText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText($"ZoneSticker_{chosenZoneRules[index].category.ToString()}_{chosenZoneRules[index].type.ToString()}");
            pbText.text = chosenZoneRules[index].powerBonus.ToString("0.##", CultureInfo.InvariantCulture) + "x";

            if (lastSelectedButton != button)
            {
                Singleton<MusicManager>.Instance.PlaySoundEffect(Plugin.assetMan.Get<SoundObject>("PaperSelectSound"));
                lastSelectedButton = button;
            }
        }

        public void SelectSticker(StandardMenuButton button, ZonePitStopManager manager)
        {
            int index = buttons.IndexOf(button);
            Singleton<EndlessZoneManager>.Instance.zoneRules.Add(chosenZoneRules[index]);
            Singleton<MusicManager>.Instance.PlaySoundEffect(Plugin.assetMan.Get<SoundObject>("PaperApplySound"));
            manager.CloseSelectScreen();
        }

        public void SetZoneRules()
        {
            var random = new System.Random(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone + rerolls);
            List<ZoneRule> currentList = new List<ZoneRule>(EndlessZoneManager.possibleZoneRules);
            List<WeightedZoneRule> weightedList = new List<WeightedZoneRule>();
            Singleton<EndlessZoneManager>.Instance.zoneRules.Clear();
            chosenZoneRules.Clear();

            foreach (ZoneRule rule in currentList)
            {
                rule.LoadPreparation(random);
                weightedList.Add(new WeightedZoneRule(rule, rule.weight));
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (weightedList.Count > 0)
                {
                    ZoneRule rule = WeightedZoneRule.ControlledRandomSelectionList(WeightedZoneRule.Convert(weightedList), random);
                    chosenZoneRules.Add(rule);
                    weightedList.RemoveAll(x => x.selection == rule);
                }
            }
        }

        public Color GetColor(ZoneRuleType type)
        {
            switch (type)
            {
                case ZoneRuleType.Positive:
                    return new(156f / 255f, 247f / 255, 148f / 255);
                case ZoneRuleType.Negative:
                    return new(247f / 255f, 173f / 255, 173f / 255);
                default:
                    return new(255f / 255f, 148f / 255, 109f / 255);
            }
        }

        public List<StandardMenuButton> buttons = new List<StandardMenuButton>();
        public List<ZoneRule> chosenZoneRules = new List<ZoneRule>();
        public List<Image> papers = new List<Image>();
        public TMP_Text descText, pbText;
        public int rerolls = 0;
        public StandardMenuButton lastSelectedButton;
    }
}
