//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    MainGameState.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 17:37
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using UnityEngine;
using WanFramework.Base;
using WanFramework.SM;
using WanFramework.UI;
using WanFramework.Utils;

namespace Game.SM.Gameplay
{
    /// <summary>
    /// 闪屏
    /// </summary>
    public class GameStateSplash : GameState
    {
        protected override void OnEnter(GameManager machine)
        {
            Debug.Log("Enter main game");
            UISystem.Instance.Show("SplashView");
        }

        protected override void OnExit(GameManager machine)
        {
            UISystem.Instance.Hide("SplashView");
        }
    }
}