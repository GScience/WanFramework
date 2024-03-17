//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    SplashView.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 20:18
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using Game.SM.Gameplay;
using UnityEngine;
using WanFramework.Base;
using WanFramework.Sequence;
using WanFramework.UI;
using WanFramework.Utils;

namespace Game.UI
{
    internal class PlaySplash : SequencePlaying<PlaySplash>
    {
        private readonly Animation _anim;
        private readonly string _name;
        public PlaySplash(Animation anim, string name, Action callback) 
            : base(new[]
            {
                PlayAnimation(c => c._anim, c => c._name),
                Run(c => callback?.Invoke())
            })
        {
            _anim = anim;
            _name = name;
        }
    }

    [RequireComponent(typeof(Animation))]
    public class SplashView : UIRootView
    {
        private Animation _anim;
        private PlaySplash _playSplash;

        protected override void InitComponents()
        {
            _anim = GetComponent<Animation>();
            _playSplash = new PlaySplash(_anim, "UI.SplashView.MainAnim", OnSplashFinished);
        }

        private void OnSplashFinished()
        {
            GameManager.Current.EnterState<GameStateTitleMenu>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _playSplash.Play(this);
        }
    }
}