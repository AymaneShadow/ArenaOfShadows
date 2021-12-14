using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    public class GameManager : MonoBehaviour
    {
        public GameObject PauseMenuUI;
        public GameObject MinionSwitchMenuUI;
        public GameObject IngameUI;
        public Image MinionSwitchButtonImage;

        private TMP_Text TargetPlayerMinionSwitchText;
        private Image TargetPlayerMinionSwitchImage;

        private TMP_Text[] PlayerMinionSwitchTexts;
        private Image[] PlayerMinionSwitchImages;

        private GameObject PagesContainer;
        private Image[] PagesImages;

        void Start() {
            MinionSwitchMenuUI.SetActive(true);
            TargetPlayerMinionSwitchText = GameObject.Find("Player1_Text").GetComponent<TMP_Text>();
            TargetPlayerMinionSwitchImage = GameObject.Find("Player1_SwitchImage").GetComponent<Image>();

            PlayerMinionSwitchTexts = new TMP_Text[2];
            PlayerMinionSwitchImages = new Image[2];
            PlayerMinionSwitchTexts[0] = GameObject.Find("Player2_Text").GetComponent<TMP_Text>();
            PlayerMinionSwitchImages[0] = GameObject.Find("Player2_SwitchImage").GetComponent<Image>();
            PlayerMinionSwitchTexts[1] = GameObject.Find("Player3_Text").GetComponent<TMP_Text>();
            PlayerMinionSwitchImages[1] = GameObject.Find("Player3_SwitchImage").GetComponent<Image>();
            MinionSwitchMenuUI.SetActive(false);

            SetMinionTarget(GetNextPlayerClockwise(GameData.Instance.currentTeam), true);


            PagesContainer = GameObject.Find("Pages Container");
            PagesImages = PagesContainer.GetComponentsInChildren<Image>();
            SetInitPages();
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log(GameData.Instance.CurrentLevel);
            if(GameData.Instance.CurrentLevel == GameData.Levels.MAP) {
                // if(Input.GetKeyDown(KeyCode.Escape)) {
                //     TogglePauseMenu();
                // }
            }
        }

        public void TogglePauseMenu() {
            if(GameData.Instance.GameIsPaused) {
                PauseMenuUI.SetActive(false);
                IngameUI.SetActive(true);
                GameData.Instance.GameIsPaused = false;
            } else {
                PauseMenuUI.SetActive(true);
                IngameUI.SetActive(false);
                GameData.Instance.GameIsPaused = true;
            }
        }

        public void ToggleMinionSwitchMenu() {
            if(GameData.Instance.IsMinionSwitchMenuOpen) {
                MinionSwitchMenuUI.SetActive(false);
                IngameUI.SetActive(true);
                GameData.Instance.IsMinionSwitchMenuOpen = false;
            } else {
                MinionSwitchMenuUI.SetActive(true);
                IngameUI.SetActive(false);
                GameData.Instance.IsMinionSwitchMenuOpen = true;
            }
        }

        public void SetMinionTarget(GameData.Team target, bool toggleMenu = true) {
            GameData.Instance.SelectedMinionTarget = target;
            MinionSwitchButtonImage.color = GetColor(target);

            UpdateLabels();

            if(toggleMenu) {
                ToggleMinionSwitchMenu();
            }
        }

        public void UpdateMinionTarget(int target) {
            int counter = 0;
            for(int i = 0; i < 4; i++) {
                if((GameData.Team)i != GameData.Instance.currentTeam && (GameData.Team)i != GameData.Instance.SelectedMinionTarget) {
                    if(target == counter) {
                        SetMinionTarget((GameData.Team)i);
                        return;
                    } else {
                        counter++;
                    }
                }

            }
        }

        public void UpdateLabels() {
            TargetPlayerMinionSwitchText.text = WrapText(GameData.Instance.PlayerNames[(int)GameData.Instance.SelectedMinionTarget]);
            TargetPlayerMinionSwitchImage.color = GetColor(GameData.Instance.SelectedMinionTarget);

            int counter = 0;
            for(int i = 0; i < 4; i++) {
                if((GameData.Team)i != GameData.Instance.currentTeam && (GameData.Team)i != GameData.Instance.SelectedMinionTarget) {
                    PlayerMinionSwitchTexts[counter].text = WrapText(GameData.Instance.PlayerNames[i]);
                    PlayerMinionSwitchImages[counter].color = GetColor((GameData.Team)i);
                    counter++;
                }

            }
        }

        public void increasePages() {
            if(GameData.Instance.NumberOfPages < 10) {
                PagesImages[GameData.Instance.NumberOfPages].enabled = true;
                GameData.Instance.NumberOfPages++;
            }
        }

        public void decreasePages() {
            if(GameData.Instance.NumberOfPages - 1 >= 0) {
                GameData.Instance.NumberOfPages--;
                PagesImages[GameData.Instance.NumberOfPages].enabled = false;
            }
        }

        void SetInitPages() {
            for(int i = 9; i >= GameData.Instance.NumberOfPages; i--) {
                PagesImages[i].enabled = false;
            }
        }

        GameData.Team GetNextPlayerClockwise(GameData.Team team) {
            return (GameData.Team) ((int)team - 1 % 4);
        }

        Color GetColor(GameData.Team team) {
            float[] color = GameData.PlayerColors[(int)team];
            return new Color(color[0], color[1], color[2], color[3]);
        }

        string WrapText(string text) {
            if(text.Length > 10) {
                return text.Substring(0, 10) + "...";
            } else {
                return text;
            }
        }
    }
}
