using System;
using UnityEngine;
using Zenject;

namespace Managers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IdleManager : MonoBehaviour
    {
        public int Length { get; set; }
        public int LengthCost { get; set; }
        public int Strength { get; set; }
        public int StrengthCost { get; set; }
        public int OfflineEarnings { get; set; }
        public int OfflineEarningsCost { get; set; }
        public int Wallet { get; set; }
        public int TotalGain { get; set; }

        public Action CastHook;

        private readonly int[] _costs =
        {
            120,
            151,
            197,
            250,
            324,
            414,
            537,
            687,
            892,
            1145,
            1484,
            1911,
            2497,
            3196,
            4148,
            5359,
            6954,
            9000,
            11687
        };

        private ScreenManager _screenManager;

        [Inject]
        public void Construct(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        private void Awake()
        {
            Length = PlayerPrefs.GetInt("Length", 30);
            Strength = PlayerPrefs.GetInt("Strength", 3);
            OfflineEarnings = PlayerPrefs.GetInt("OfflineEarnings", 3);
            LengthCost = _costs[Length / 10 - 3];
            StrengthCost = _costs[Strength - 3];
            OfflineEarningsCost = _costs[OfflineEarnings - 3];
            Wallet = PlayerPrefs.GetInt("Wallet", 0);
            DontDestroyOnLoad(this);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                var now = DateTime.Now;
                PlayerPrefs.SetString("Date", $"{now}");
            }
            else
            {
                var stringDate = PlayerPrefs.GetString("Date", string.Empty);
                if (stringDate != string.Empty)
                {
                    var dateTime = DateTime.Parse(stringDate);
                    TotalGain = (int) ((DateTime.Now - dateTime).TotalMinutes * OfflineEarnings + 1f);
                    _screenManager?.ChangeScreen(Screens.Return);
                }
            }
        }

        private void OnApplicationQuit()
        {
            OnApplicationPause(true);
        }

        public void BuyLength()
        {
            if (Wallet < LengthCost) return;
            Length += 10;
            Wallet -= LengthCost;
            LengthCost = _costs[Length / 10 - 3];
            PlayerPrefs.SetInt("Length", Length);
            PlayerPrefs.SetInt("Wallet", Wallet);
            _screenManager.ChangeScreen(Screens.Main);
        }
        
        public void BuyStrength()
        {
            if (Wallet < StrengthCost) return;
            Strength++;
            Wallet -= StrengthCost;
            StrengthCost = _costs[Strength - 3];
            PlayerPrefs.SetInt("Strength", Strength);
            PlayerPrefs.SetInt("Wallet", Wallet);
            _screenManager.ChangeScreen(Screens.Main);
        }

        public void BuyOfflineEarnings()
        {
            if (Wallet < OfflineEarningsCost) return;
            OfflineEarnings++;
            Wallet -= OfflineEarningsCost;
            OfflineEarningsCost = _costs[OfflineEarnings - 3];
            PlayerPrefs.SetInt("OfflineEarnings", OfflineEarnings);
            PlayerPrefs.SetInt("Wallet", Wallet);
            _screenManager.ChangeScreen(Screens.Main);
        }

        public void Collect()
        {
            Wallet += TotalGain;
            PlayerPrefs.SetInt("Wallet", Wallet);
            _screenManager.ChangeScreen(Screens.Main);
        }

        public void CollectDouble()
        {
            Wallet += TotalGain;
            PlayerPrefs.SetInt("Wallet", Wallet);
            _screenManager.ChangeScreen(Screens.Main);
        }

        public void DoCastHook()
        {
            CastHook?.Invoke();
        }
    }
}