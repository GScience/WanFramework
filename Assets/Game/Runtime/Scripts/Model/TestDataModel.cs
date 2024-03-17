// ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    TestDataModel.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   17
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using Game.UI.TestUIViewComponent;
using WanFramework.UI.DataComponent;

namespace Game.Model
{
    public class TestDataModel : DataModelBase
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChangedEvent();
            }
        }
        
        private float _barValue;

        public float BarValue
        {
            get => _barValue;
            set
            {
                _barValue = value;
                RaisePropertyChangedEvent();
            }
        }

        public DataCollection<TestCollectionDataModel> Collection { get; } = new();
    }
}