//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    TestUIView.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/10/2024 18:48
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using System.Globalization;
using Game.Model;
using Game.SM.Gameplay;
using Game.UI.TestUIViewComponent;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WanFramework.Base;
using WanFramework.UI;
using WanFramework.UI.DataComponent;
using Random = UnityEngine.Random;

namespace Game.UI
{
    public partial class TestUIView : UIRootView
    {
        private int _index = 0;
        [SerializeField]
        private Text testText;
        [SerializeField]
        private InputField inputField;
        [SerializeField]
        private TestUICollectionView collectionView;
        [SerializeField]
        private Scrollbar scrollBar;
        [SerializeField]
        private Text testText2;
        
        [SerializeField]
        private Button resetButton;
        
        private List<string> _randString = new();
        
        protected override void InitComponents()
        {
            base.InitComponents();
            
            Bind(nameof(TestDataModel.Text), m => testText.text = m.As<TestDataModel>().Text);
            Bind(nameof(TestDataModel.BarValue), m => testText2.text = m.As<TestDataModel>().BarValue.ToString());
            
            onDataModelChanged.AddListener(m => collectionView.ItemSource = m.As<TestDataModel>().Collection);
            scrollBar.onValueChanged.AddListener(val => DataModel.As<TestDataModel>().BarValue = val);
            inputField.onValueChanged.AddListener(str => DataModel.As<TestDataModel>().Text = str);
            resetButton.onClick.AddListener(OnResetButtonClick);
        }

        private void OnResetButtonClick()
        {
            GameManager.Current.EnterState<GameStateSplash>();
        }

        public override void OnShow()
        {
            base.OnShow();
            _index = 0;
        }

        private void Update()
        {
            if (_index < 100)
            {
                var insertAt = DataModel.As<TestDataModel>().Collection.Count > 1 ? 1 : 0;
                DataModel.As<TestDataModel>().Collection.Insert(insertAt, 
                    new TestCollectionDataModel()
                    {
                        Title = _index++.ToString(),
                        Content = "Content"
                    });
                _randString.Add(Random.value.ToString(CultureInfo.InvariantCulture));
            }
            else if (DataModel.As<TestDataModel>().Collection.Count < 95)
            {
                var collection = DataModel.As<TestDataModel>().Collection;
                for (var i = 0; i < collection.Count; ++i)
                    collection[i].Content = _randString[Random.Range(0, 100)];
            }
        }
    }
}