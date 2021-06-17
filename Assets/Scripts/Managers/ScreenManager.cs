using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Managers
{
    public enum Screens
    {
        Main,
        Game,
        End,
        Return
    }
    
    public class ScreenManager : MonoBehaviour
    {

        [SerializeField] private List<GameObject> screens = new List<GameObject>();
        [SerializeField] private Button buyLengthButton;
        [SerializeField] private Button buyStrengthButton;
        [SerializeField] private Button buyOfflineEarningsButton;

        [SerializeField] private Text gameScreenMoney;
        [SerializeField] private Text lengthCostText;
        [SerializeField] private Text lengthValueText;
        [SerializeField] private Text strengthCostText;
        [SerializeField] private Text strengthValueText;
        [SerializeField] private Text offlineCostText;
        [SerializeField] private Text offlineValueText;
        [SerializeField] private Text endScreenMoney;
        [SerializeField] private Text returnScreenMoney;

        private Screens currentScreen;

        private int gameCount;

        private Action[] actions;

        private IdleManager idleManager;

        [Inject]
        public void Construct(IdleManager idleManagerZ)
        {
            idleManager = idleManagerZ;
        }

        private void Awake()
        {
            InitActions();
            currentScreen = Screens.Main;
        }

        private void InitActions()
        {
            actions = new Action[]
            {
                OnStartMainScreen,
                OnStartGameScreen,
                OnStartEndScreen,
                OnStartReturnScreen
            };
        }

        private void Start()
        {
            CheckIdles();
            UpdateTexts();
        }

        public void ChangeScreen(Screens newScreen)
        {
            screens[(int)currentScreen].SetActive(false);
            currentScreen = newScreen;
            actions[(int)currentScreen].Invoke();
            screens[(int)currentScreen].SetActive(true);
        }

        private void OnStartMainScreen()
        {
            UpdateTexts();
            CheckIdles();
        }

        private void OnStartGameScreen()
        {
            gameCount++;
        }
        private void OnStartEndScreen()
        {
            SetEndScreenMoney();
        }
        private void OnStartReturnScreen()
        {
            SetReturnScreenMoney();
        }

        public void SetEndScreenMoney()
        {
            endScreenMoney.text = $"${idleManager.TotalGain}";
        }
        
        public void SetReturnScreenMoney()
        {
            returnScreenMoney.text = $"${idleManager.TotalGain} gained while waiting!";
        }

        private void UpdateTexts()
        {
            gameScreenMoney.text = $"${idleManager.Wallet}";
        }

        private void CheckIdles()
        {
            throw new NotImplementedException();
        }
    }
}
