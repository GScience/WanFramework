//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    GameStateTitleMenu.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 18:54
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using Game.Model;
using Game.UI;
using WanFramework.Base;
using WanFramework.UI;
using WanFramework.UI.DataComponent;

namespace Game.SM.Gameplay
{
    public class GameStateTitleMenu : GameState
    {
        protected override void OnEnter(GameManager machine)
        {
            var testUIView = UISystem.Instance.Show("TestUIView");
            testUIView.DataModel = DataModel<TestDataModel>.Instance;
        }
        
        protected override void OnExit(GameManager machine)
        {
            UISystem.Instance.Hide("TestUIView");
            DataModel<TestDataModel>.Instance.Collection.Clear();
            DataModel<TestDataModel>.Instance.BarValue = 0;
            DataModel<TestDataModel>.Instance.Text = "";
        }
    }
}