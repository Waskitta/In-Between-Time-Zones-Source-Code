using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace BaldiPlusRandomZone.Menu
{
    public class ZoneLevelSelectMenu : MonoBehaviour
    {
        private void Awake() =>
            CursorController.Instance.transform.SetSiblingIndex(CursorController.Instance.transform.parent.childCount);     

        private void Start()
        {
            UpdateButtons();
            lifeModeText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Opt_LifeMode_" + lifeMode.ToString());
        }


        public void UpdateButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == selectedLevel)
                    buttons[i].text.text = $"<b>{buttons[i].text.text}</b>"; 
                else
                    buttons[i].text.text = buttons[i].text.text.Replace("<b>", "").Replace("</b>", "");
            }

            UpdateContinueButton();
        }

        public void UpdateContinueButton()
        {
            if (!HasSave())
            {
                continueButton.text.raycastTarget = false;
                continueButton.text.color = Color.gray;
                continueButton.underlineOnHigh = false;
                return;
            }

            continueButton.text.raycastTarget = true;
            continueButton.text.color = Color.black;
            continueButton.underlineOnHigh = true;
        }

        public bool HasSave() => File.Exists(Path.Combine(Application.persistentDataPath, $"LevelZoneSave_{Singleton<PlayerFileManager>.Instance.fileName}_{selectedLevel}_{lifeMode.ToString()}.lzsf"));

        public void SetLifeMode(int direction)
        {
            LifeMode lifeMode = ZoneLevelSelectMenu.lifeMode;
            lifeMode += direction;

            if (lifeMode > LifeMode.Explorer)
                lifeMode = LifeMode.Normal;
            else if (lifeMode < LifeMode.Normal)
                lifeMode = LifeMode.Explorer;

            ZoneLevelSelectMenu.lifeMode = lifeMode;
            lifeModeText.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Opt_LifeMode_" + lifeMode.ToString());
            UpdateContinueButton();
        }

        public List<StandardMenuButton> buttons = new List<StandardMenuButton>();
        public StandardMenuButton continueButton;
        public TMP_Text lifeModeText;
        public static int selectedLevel = 1;
        public static bool toLoad;
        public static LifeMode lifeMode;
    }
}
