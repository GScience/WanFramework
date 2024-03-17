//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    TestUISubView.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 21:40
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using UnityEngine;
using WanFramework.UI;
using WanFramework.UI.DataComponent;
using WanFramework.Utils;

namespace Game.UI.TestUIViewComponent
{
    public class TestCollectionDataModel : DataModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChangedEvent();
            }
        }
        
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                RaisePropertyChangedEvent();
            }
        }
    }
    
    public class TestUICollectionView : UICollectionView<TestCollectionDataModel>
    {
        protected override void OnElementAdding(UISubView subView)
        {
            ((TestUISubView)subView).onSelectElement.AddListener(OnSelectElement);
            ((TestUISubView)subView).DoFadeIn(null);
        }
        
        protected override void OnElementRemoving(UISubView subView)
        {
            ((TestUISubView)subView).onSelectElement.RemoveListener(OnSelectElement);
            base.OnElementRemoving(subView);
        }

        private void OnSelectElement(TestUISubView view)
        {
            view.DoFadeOut(() => ItemSource.RemoveAt(IndexOf(view)));
        }
    }
}