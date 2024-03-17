using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using WanFramework.SM;
using WanFramework.Utils;

namespace WanFramework.Base
{
    /// <summary>
    /// 游戏定义
    /// </summary>
    public sealed class GameManager : StateMachine<GameManager>
    {
        public static GameManager Current;

        [SerializeField]
        private Camera mainCamera;

        public Camera MainCamera => mainCamera;
        
        private ISystem[] _systems;
        
        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            Current = this;
            SetupSystem();
            RunGame();
        }
        
        /// <summary>
        /// 系统初始化
        /// </summary>
        private void SetupSystem()
        {
            LogCurrentState();
            
            // load all system
            _systems = GetComponentsInChildren<ISystem>();
            Array.Sort(_systems, (a, b) =>
            {
                var aAttr = a.GetType().GetCustomAttributes(typeof(SystemPriorityAttribute)).FirstOrDefault();
                var bAttr = b.GetType().GetCustomAttributes(typeof(SystemPriorityAttribute)).FirstOrDefault();
                var aPriority = (aAttr as SystemPriorityAttribute)?.priority ?? 0;
                var bPriority = (bAttr as SystemPriorityAttribute)?.priority ?? 0;
                return bPriority - aPriority;
            });
            // init all system
            foreach (var system in _systems)
                system.Init();
        }

        private void RunGame()
        {
            LogCurrentState();
            
            if (GameEntryPoint.Instance == null)
            {
                Debug.LogError("A game entry point is needed to start the game");
                return;
            }

            GameEntryPoint.Instance.Main();
        }

        private static void LogCurrentState([CallerMemberName] string state = "")
        {
            Debug.Log(state);
        }
    }
    
    /// <summary>
    /// 程序入口点
    /// </summary>
    public abstract class GameEntryPoint : SingletonBehaviour<GameEntryPoint>
    {
        public abstract void Main();
    }

    /// <summary>
    /// 游戏状态
    /// </summary>
    public class GameState : StateBehaviour<GameManager>
    {
    }
}
