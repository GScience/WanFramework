//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    IStateMachine.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/11/2024 16:47
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System.Collections.Generic;
using UnityEngine;

namespace WanFramework.SM
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public interface IState
    {
        public int Id { get; internal set; }
        
        /// <summary>
        /// 当第一次进入状态时调用
        /// </summary>
        /// <param name="machine"></param>
        public void OnInit(IStateMachine machine);
        
        /// <summary>
        /// 当进入状态时且初始化结束后调用
        /// </summary>
        /// <param name="machine"></param>
        public void OnEnter(IStateMachine machine);
        
        /// <summary>
        /// 当离开状态时调用
        /// </summary>
        /// <param name="machine"></param>
        public void OnExit(IStateMachine machine);
        
        /// <summary>
        /// 更新时调用
        /// </summary>
        /// <param name="machine"></param>
        public void OnUpdate(IStateMachine machine);
    }
    
    public abstract class StateBehaviour<T> : MonoBehaviour, IState where T : IStateMachine
    {
        int IState.Id { get; set; }

        void IState.OnInit(IStateMachine machine)
        {
            OnInit((T)machine);
        }

        void IState.OnEnter(IStateMachine machine)
        {
            OnEnter((T)machine);
        }

        void IState.OnExit(IStateMachine machine)
        {
            OnExit((T)machine);
        }

        void IState.OnUpdate(IStateMachine machine)
        {
            OnUpdate((T)machine);
        }
        
        protected virtual void OnInit(T machine)
        {
        }

        protected virtual void OnEnter(T machine)
        {
        }

        protected virtual void OnExit(T machine)
        {
        }

        protected virtual void OnUpdate(T machine)
        {
        }
    }

    /// <summary>
    /// 状态静态实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class State<T> where T : IState
    {
        private static readonly IState Instance = StateMachineSystem.Instance.CreateState<T>();
        public static int Id => Instance.Id;
        
        public static void OnInit(IStateMachine machine)
        {
            Instance.OnInit(machine);
        }

        public static void OnEnter(IStateMachine machine)
        {
            Instance.OnEnter(machine);
        }

        public static void OnExit(IStateMachine machine)
        {
            Instance.OnExit(machine);
        }

        public static void OnUpdate(IStateMachine machine)
        {
            Instance.OnUpdate(machine);
        }
    }
}