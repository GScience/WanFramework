//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    GameMain.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 17:28
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using Game.SM;
using Game.SM.Gameplay;
using WanFramework.Base;

namespace Game
{
    public class GameMain : GameEntryPoint
    {
        public override void Main()
        {
            GameManager.Current.EnterState<GameStateSplash>();
        }
    }
}