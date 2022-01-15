using System;
using System.Collections;
using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameManagement
{
    public class GameManager : MonoBehaviour
    {
        public GameObject PauseMenuUI;
        public GameObject IngameUI;
        public Image MinionSwitchButtonImage;

        private GameObject PagesContainer;
        private Image[] PagesImages;

        private GameObject AutoAttackOnImage;
        private GameObject AutoAttackOffImage;

        private TextMeshProUGUI LevelUpLabel;
        private GameObject[] levelUpButtons;

        [CanBeNull] private MasterController controller;

        [SerializeField] private MinionValues minionValues;
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private GameObject spawnPointHolder;
        [SerializeField] private GameObject minionPaths;

        void Start()
        {
            PauseMenuUI = GameObject.Find("PauseMenu_UI");
            PauseMenuUI.SetActive(false);
            IngameUI = GameObject.Find("Ingame_UI");
            MinionSwitchButtonImage = GameObject.Find("Player_Color_Sprite").GetComponent<Image>();
            AutoAttackOnImage = GameObject.Find("ON_Sprite");
            AutoAttackOffImage = GameObject.Find("OFF_Sprite");
            LevelUpLabel = GameObject.Find("LevelUp_Label").GetComponent<TextMeshProUGUI>();
            levelUpButtons = GameObject.FindGameObjectsWithTag("LevelUpButton");
            SetVisibilityOfLevelUpButtons(false);


            SetMinionTarget(GetNextPlayerClockwise(GameData.Instance.currentTeam, true));

            PagesContainer = GameObject.Find("Pages Container");
            PagesImages = PagesContainer.GetComponentsInChildren<Image>();

            SetAutoAttack(GameData.Instance.AutoAttack);

            SetInitPages();
            
            try
            {
                controller = gameObject.AddComponent<MasterController>() ?? throw new NullReferenceException();
                controller.Init(minionValues, minionPrefab, spawnPointHolder, minionPaths);

                controller.StartMinionSpawning(2000);
            }
            catch
            {
                Debug.LogError("Could not create master controller. Server functionality will not work");
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void TogglePauseMenu()
        {
            if (GameData.Instance.GameIsPaused)
            {
                PauseMenuUI.SetActive(false);
                IngameUI.SetActive(true);
                GameData.Instance.GameIsPaused = false;
            }
            else
            {
                PauseMenuUI.SetActive(true);
                IngameUI.SetActive(false);
                GameData.Instance.GameIsPaused = true;
            }
        }

        public void SetMinionTarget(GameData.Team target)
        {
            GameData.Instance.SelectedMinionTarget = target;
            MinionSwitchButtonImage.color = GetColor(target);
        }

        public void SetMinionsToNextTarget()
        {
            SetMinionTarget(GetNextPlayerClockwise(GameData.Instance.SelectedMinionTarget, true));
        }

        public void SetAutoAttack(bool state)
        {
            GameData.Instance.AutoAttack = state;
            if (state)
            {
                AutoAttackOnImage.SetActive(true);
                AutoAttackOffImage.SetActive(false);
            }
            else
            {
                AutoAttackOnImage.SetActive(false);
                AutoAttackOffImage.SetActive(true);
            }
        }

        public void SwitchAutoAttack()
        {
            SetAutoAttack(!GameData.Instance.AutoAttack);
        }

        public void increasePages()
        {
            if (GameData.Instance.NumberOfPages < 10)
            {
                PagesImages[GameData.Instance.NumberOfPages].enabled = true;
                GameData.Instance.NumberOfPages++;
            }
        }

        public void decreasePages()
        {
            if (GameData.Instance.NumberOfPages - 1 >= 0)
            {
                GameData.Instance.NumberOfPages--;
                PagesImages[GameData.Instance.NumberOfPages].enabled = false;
            }
        }

        void SetInitPages()
        {
            for (int i = 9; i >= GameData.Instance.NumberOfPages; i--)
            {
                PagesImages[i].enabled = false;
            }
        }

        GameData.Team GetNextPlayerClockwise(GameData.Team team, bool CountOwnPlayer)
        {
            int temp = (((int) team - 1) + 4) % 4;
            if (!CountOwnPlayer)
            {
                return (GameData.Team) temp;
            }
            else
            {
                if ((GameData.Team) temp == GameData.Instance.currentTeam)
                {
                    temp = (((int) temp - 1) + 4) % 4;
                }
            }

            return (GameData.Team) temp;
        }

        Color GetColor(GameData.Team team)
        {
            float[] color = GameData.PlayerColors[(int) team];
            return new Color(color[0], color[1], color[2], color[3]);
        }

        public IEnumerator ShowLevelUpLabel()
        {
            LevelUpLabel.enabled = true;
            SetVisibilityOfLevelUpButtons(true);
            yield return new WaitForSeconds(4);
            LevelUpLabel.enabled = false;
        }

        private void SetVisibilityOfLevelUpButtons(bool visibility)
        {
            foreach (GameObject button in levelUpButtons)
            {
                button.SetActive(visibility);
            }
        }
    }
}