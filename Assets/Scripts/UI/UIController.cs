﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum WindowsEnum
    {
        MainMenu,
        InGame,
    }

    public class UIController : MBSingleton<UIController>
    {
        [SerializeField] private BaseWindow[] _windows;

        private WindowsEnum currentUI;

        private void Awake()
        {
            currentUI = WindowsEnum.MainMenu;
            SetWindow(currentUI, false);
        }

        public void SetWindow(WindowsEnum window, bool withEffect = false)
        {
            if (withEffect)
                AudioManager.Instance().Play(AudioClips.Click);

            for (int i = 0; i < _windows.Length; i++)
            {
                if ((int)window == i)
                    _windows[i].Open();
                else
                    _windows[i].Close();
            }
        }
    }
}
