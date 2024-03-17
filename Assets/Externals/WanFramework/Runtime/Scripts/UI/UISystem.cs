//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    UISystem.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/03/2024 19:28
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System.Collections.Generic;
using UnityEngine;
using WanFramework.Base;
using WanFramework.Resource;
using WanFramework.Sequence;

namespace WanFramework.UI
{
    [SystemPriority(SystemPriorities.UI)]
    public class UISystem : SystemBase<UISystem>
    {
        public const string DefaultUIViewCategory = "UIView";
        
        [Tooltip("UI根节点")]
        public Transform uiRoot;
        
        [SerializeField]
        [Tooltip("UI相机")]
        private Camera uiCamera;
        public Camera UICamera => uiCamera;
        
        private readonly Dictionary<string, UIRootView> _viewCache = new();
        
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="autoLoad"></param>
        /// <returns></returns>
        public UIRootView Get(string viewName, bool autoLoad = true)
        {
            if (_viewCache.TryGetValue(viewName, out var cached))
                return cached;
            if (autoLoad)
            {
                cached = Load(viewName);
                cached.gameObject.SetActive(false);
            }
            else
                Debug.LogError($"View {viewName} not found in opened view cache");
            return cached;
        }

        public T Get<T>(string viewName) where T : UIRootView
        {
            return Get(viewName) as T;
        }

        private UIRootView Load(string viewName)
        {
            Debug.Log($"Show {viewName}");
            if (_viewCache.TryGetValue(viewName, out var cached))
            {
                cached.gameObject.SetActive(true);
                cached.OnShow();
                return cached;
            }

            var prefab = ResourceSystem.Instance.Load<UIRootView>(viewName, DefaultUIViewCategory);
            var instance = Instantiate(prefab, uiRoot);
            instance.name = viewName;
            _viewCache[viewName] = instance;
            return instance;
        }
        
        /// <summary>
        /// 弹出UI
        /// </summary>
        public UIRootView Show(string viewName)
        {
            var instance = Load(viewName);
            instance.gameObject.SetActive(true);
            instance.OnShow();
            return instance;
        }

        public T Show<T>(string viewName) where T: UIRootView
        {
            return Show(viewName) as T;
        }
        
        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void Hide(UIRootView rootView)
        {
            rootView.OnHide();
            // TODO: 隐藏UI
            rootView.gameObject.SetActive(false);
            // 停止绑定在UI上的所有序列动画
            this.StopAllSequence();
        }
        
        public void Hide(string viewName)
        {
            if (_viewCache.TryGetValue(viewName, out var cached))
                Hide(cached);
            else
                Debug.LogWarning($"View {viewName} not found in cache while trying to fade");
        }
    }
}