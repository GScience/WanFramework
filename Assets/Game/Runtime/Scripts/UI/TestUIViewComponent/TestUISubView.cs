//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    TestUIBar.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 21:41
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WanFramework.Sequence;
using WanFramework.UI;
using WanFramework.UI.DataComponent;
using WanFramework.Utils;

namespace Game.UI.TestUIViewComponent
{
    public class FadeOutPlaying : SequencePlaying<FadeOutPlaying>
    {
        private CanvasGroup _canvasGroup;
        private float _speed;

        public Action Callback;
        
        public FadeOutPlaying(CanvasGroup canvasGroup, float speed) : base(new[]
        {
            FadeOut(c => c._canvasGroup, c => c._speed),
            Run(c => c.Callback?.Invoke())
        })
        {
            _canvasGroup = canvasGroup;
            _speed = speed;
        }
    }
    
    public class FadeInPlaying : SequencePlaying<FadeInPlaying>
    {
        private CanvasGroup _canvasGroup;
        private float _speed;

        public Action Callback;
        
        public FadeInPlaying(CanvasGroup canvasGroup, float speed) : base(new[]
        {
            FadeIn(c => c._canvasGroup, c => c._speed),
            Run(c => c.Callback?.Invoke())
        })
        {
            _canvasGroup = canvasGroup;
            _speed = speed;
        }
    }
    
    public class TestUISubView : UISubView
    {
        private FadeOutPlaying _fadeOutPlaying;
        private FadeInPlaying _fadeInPlaying;

        [SerializeField] 
        private CanvasGroup canvasGroup;
        
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text contentText;
        [SerializeField]
        private Button button;

        public UnityEvent<TestUISubView> onSelectElement = new();

        private void ResetSequencePlay()
        {
            this.StopAllSequence();
        }
        
        public override void OnShow()
        {
            ResetSequencePlay();
        }
        
        public override void OnHide()
        {
            ResetSequencePlay();
        }
        
        protected override void InitComponents()
        {
            base.InitComponents();
            Bind(nameof(TestCollectionDataModel.Title), m => titleText.text = m.As<TestCollectionDataModel>().Title);
            Bind(nameof(TestCollectionDataModel.Content), m => contentText.text = m.As<TestCollectionDataModel>().Content);
            button.onClick.AddListener(() => onSelectElement.Invoke(this));
            _fadeOutPlaying = new FadeOutPlaying(canvasGroup, 1);
            _fadeInPlaying = new FadeInPlaying(canvasGroup, 1);
        }
        
        public void DoFadeOut(Action callback)
        {
            _fadeOutPlaying.Callback = callback;
            button.enabled = false;
            ResetSequencePlay();
            _fadeOutPlaying.Play(this);
        }
        
        public void DoFadeIn(Action callback)
        {
            _fadeInPlaying.Callback = callback;
            button.enabled = true;
            ResetSequencePlay();
            _fadeInPlaying.Play(this);
        }
    }
}