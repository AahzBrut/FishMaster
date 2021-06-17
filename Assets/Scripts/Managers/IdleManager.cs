using System;
using UnityEngine;

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

        private int[] costs =
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

        private void Awake()
        {
            Length = PlayerPrefs.GetInt("Length", 30);
            Strength = PlayerPrefs.GetInt("Strength", 3);
            OfflineEarnings = PlayerPrefs.GetInt("OfflineEarnings", 3);
            LengthCost = costs[Length / 10 - 3];
            StrengthCost = costs[Strength - 3];
            OfflineEarningsCost = costs[OfflineEarnings - 3];
            Wallet = PlayerPrefs.GetInt("Wallet", 0);
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
            LengthCost = costs[Length / 10 - 3];
            PlayerPrefs.SetInt("Length", Length);
            PlayerPrefs.SetInt("Wallet", Wallet);
        }
        
        public void BuyStrength()
        {
            if (Wallet < StrengthCost) return;
            Strength++;
            Wallet -= StrengthCost;
            StrengthCost = costs[Strength - 3];
            PlayerPrefs.SetInt("Strength", Strength);
            PlayerPrefs.SetInt("Wallet", Wallet);
        }

        public void BuyOfflineEarnings()
        {
            if (Wallet < OfflineEarningsCost) return;
            OfflineEarnings++;
            Wallet -= OfflineEarningsCost;
            OfflineEarningsCost = costs[OfflineEarnings - 3];
            PlayerPrefs.SetInt("OfflineEarnings", OfflineEarnings);
            PlayerPrefs.SetInt("Wallet", Wallet);
        }

        public void Collect()
        {
            Wallet += TotalGain;
            PlayerPrefs.SetInt("Wallet", Wallet);
        }

        public void CollectDouble()
        {
            Wallet += TotalGain;
            PlayerPrefs.SetInt("Wallet", Wallet);
        }
    }
}