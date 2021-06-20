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

        private Screens _currentScreen;

        private int _gameCount;

        private Action[] _actions;

        private IdleManager _idleManager;

        [Inject]
        public void Construct(IdleManager idleManager)
        {
            _idleManager = idleManager;
        }

        private void Awake()
        {
            InitActions();
            _currentScreen = Screens.Main;
        }

        private void InitActions()
        {
            _actions = new Action[]
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
            ChangeScreen(Screens.Main);
        }

        public void ChangeScreen(Screens newScreen)
        {
            screens[(int)_currentScreen].SetActive(false);
            _currentScreen = newScreen;
            _actions[(int)_currentScreen].Invoke();
            screens[(int)_currentScreen].SetActive(true);
        }

        private void OnStartMainScreen()
        {
            UpdateTexts();
            CheckIdles();
        }

        private void OnStartGameScreen()
        {
            _gameCount++;
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
            endScreenMoney.text = $"${_idleManager.TotalGain}";
        }
        
        public void SetReturnScreenMoney()
        {
            returnScreenMoney.text = $"${_idleManager.TotalGain} gained while waiting!";
        }

        private void UpdateTexts()
        {
            gameScreenMoney.text = $"${_idleManager.Wallet}";
            lengthCostText.text = $"${_idleManager.LengthCost}";
            lengthValueText.text = $"{_idleManager.Length} m";
            strengthCostText.text = $"${_idleManager.StrengthCost}";
            strengthValueText.text = $"{_idleManager.Strength} fishes";
            offlineCostText.text = $"${_idleManager.OfflineEarningsCost}";
            offlineValueText.text = $"${_idleManager.OfflineEarnings}/min";
        }

        private void CheckIdles()
        {
            buyLengthButton.interactable = _idleManager.Wallet >= _idleManager.LengthCost;
            buyStrengthButton.interactable = _idleManager.Wallet >= _idleManager.StrengthCost;
            buyOfflineEarningsButton.interactable = _idleManager.Wallet >= _idleManager.OfflineEarningsCost;
        }
    }
}
