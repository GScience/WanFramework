//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    Sequence.Condition.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   03/09/2024 12:04
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;

namespace WanFramework.Sequence
{
    /// <summary>
    /// 此处负责条件执行相关的实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class SequencePlaying<T>
    {
        /// <summary>
        /// 运行条件播放
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="playing"></param>
        /// <returns></returns>
        protected static IPlaying<T> RunConditional(Getter<bool> condition, IPlaying<T> playing)
        {
            return 
                new ConditionalPlaying<T>(condition, playing);
        }
        
        /// <summary>
        /// 循环播放
        /// </summary>
        /// <param name="count">次数</param>
        /// <param name="playing">可播放对象</param>
        /// <returns></returns>
        protected static IPlaying<T> RunLoop(Getter<int> count, IPlaying<T> playing)
        {
            return
                new LoopPlaying<T>(count, playing);
        }
        
        /// <summary>
        /// 循环播放
        /// </summary>
        /// <param name="condition">循环条件</param>
        /// <param name="playing">可播放对象</param>
        /// <returns></returns>
        protected static IPlaying<T> RunConditionalLoop(Getter<bool> condition, IPlaying<T> playing)
        {
            return
                new ConditionalLoopPlaying<T>(condition, playing);
        }
    }
    
    class ConditionalLoopPlaying<T> : IPlaying<T> where T : SequencePlaying<T>
    {
        private readonly SequencePlaying<T>.Getter<bool> _condition;
        private readonly IPlaying<T> _playing;
        
        public ConditionalLoopPlaying(SequencePlaying<T>.Getter<bool> condition, IPlaying<T> playing)
        {
            _condition = condition;
            _playing = playing;
        }
        
        public void OnUpdate(T context, out bool isFinished)
        {
            isFinished = false;
            _playing.OnUpdate(context, out var isInnerFinished);
            if (!isInnerFinished) return;
            _playing.OnExit(context);
            if (_condition?.Invoke(context) == true)
                _playing.OnEnter(context);
            else isFinished = true;
        }

        public void OnEnter(T context)
        {
            if (_condition?.Invoke(context) == true)
                _playing.OnEnter(context);
        }

        public void OnExit(T context)
        {
        }
    }
    
    class LoopPlaying<T> : IPlaying<T> where T : SequencePlaying<T>
    {
        private readonly SequencePlaying<T>.Getter<int> _count;
        private readonly IPlaying<T> _playing;
        private int _loopCount;
        
        public LoopPlaying(SequencePlaying<T>.Getter<int> count, IPlaying<T> playing)
        {
            _count = count;
            _playing = playing;
        }
        
        public void OnUpdate(T context, out bool isFinished)
        {
            isFinished = false;
            _playing.OnUpdate(context, out var isInnerFinished);
            if (!isInnerFinished) return;
            --_loopCount;
            _playing.OnExit(context);
            if (_loopCount > 0)
                _playing.OnEnter(context);
            else isFinished = true;
        }

        public void OnEnter(T context)
        {
            _loopCount = _count.Invoke(context);
            if (_loopCount != 0)
                _playing.OnEnter(context);
        }

        public void OnExit(T context)
        {
        }
    }
    
    class ConditionalPlaying<T> : IPlaying<T> where T : SequencePlaying<T>
    {
        private readonly SequencePlaying<T>.Getter<bool> _condition;
        private readonly IPlaying<T> _playing;
        private bool _isConditionOk;
        
        public ConditionalPlaying(SequencePlaying<T>.Getter<bool> condition, IPlaying<T> playing)
        {
            _condition = condition;
            _playing = playing;
        }
        
        public void OnUpdate(T context, out bool isFinished)
        {
            if (_isConditionOk)
                _playing.OnUpdate(context, out isFinished);
            else
                isFinished = !_isConditionOk;
        }

        public void OnEnter(T context)
        {
            _isConditionOk = _condition.Invoke(context);
            if (_isConditionOk)
                _playing.OnEnter(context);
        }

        public void OnExit(T context)
        {
            if (_isConditionOk)
                _playing.OnExit(context);
        }
    }
}