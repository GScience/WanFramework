//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    UIView.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   01/06/2024 14:21
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using WanFramework.UI.DataComponent;

namespace WanFramework.UI
{
    /// <summary>
    /// 视图基类
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        [Tooltip("当切换绑定的数据模型")]
        public UnityEvent<DataModelBase> onDataModelChanged;
        
        private readonly DataBindingWorker _worker = new();
        private DataModelBase _dataModel;
        public DataModelBase DataModel
        {
            get => _dataModel;
            set
            {
                _dataModel = value;
                onDataModelChanged?.Invoke(_dataModel);
                if (enabled)
                    _worker.SetupDataBinding(_dataModel);
            }
        }
        
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="propertyName">监听的属性</param>
        /// <param name="toTarget">源发生变化后执行的绑定函数</param>
        /// <returns>绑定路径</returns>
        public BindingPath Bind(string propertyName, BindFunc toTarget)
        {
            return _worker.Bind(propertyName, toTarget);
        }

        public void Unbind(ref BindingPath path)
        {
            _worker.Unbind(path);
            path.ToTarget = null;
        }
        
        /// <summary>
        /// 初始化所有组件，在Awake时调用，仅初始化1次
        /// </summary>
        protected virtual void InitComponents()
        {
            
        }
        
        protected void Awake()
        {
            InitComponents();
        }

        protected virtual void OnDisable()
        {
            _worker.SetupDataBinding(null);
        }
        
        protected virtual void OnEnable()
        {
            _worker.SetupDataBinding(_dataModel);
        }
    }

    /// <summary>
    /// 顶层视图
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIRootView : ViewBase
    {
        public virtual void OnShow()
        {
        }
        
        public virtual void OnHide()
        {
        }
    }

    /// <summary>
    /// 子视图
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class UISubView : ViewBase
    {
        public virtual void OnShow()
        {
        }
        
        public virtual void OnHide()
        {
        }
    }
    
    /// <summary>
    /// 集合UI视图，负责自动根据集合创建子对象
    /// </summary>
    public abstract class UICollectionView<T> : UISubView where T : DataModelBase
    {
        [SerializeField]
        [Tooltip("子视图根节点")]
        private Transform root;
        
        private readonly CollectionBindingWorker<T> _worker;
        private DataCollection<T> _itemSource;
        
        /// <summary>
        /// 数据源
        /// </summary>
        public DataCollection<T> ItemSource
        {
            get => _itemSource;
            set
            {
                _itemSource = value;
                if (enabled)
                    _worker.SetupCollectionBinding(_itemSource);
            }
        }

        private ObjectPool<UISubView> _subViewPool;
        /// <summary>
        /// 视图实例
        /// </summary>
        private readonly List<UISubView> _subViews = new();
        
        [Tooltip("集合模板")]
        [SerializeField]
        private UISubView template;
        public UISubView Template => template;

        protected UICollectionView()
        {
            _worker = new(OnCollectionItemRemove, OnCollectionItemInsert, OnCollectionReset);
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            _subViewPool = new ObjectPool<UISubView>(Create, OnGet, OnRelease, OnViewDestroy);

            UISubView Create()
            {
                return Instantiate(Template, root);
            }

            void OnGet(UISubView view)
            {
                view.gameObject.SetActive(true);
            }
            
            void OnRelease(UISubView view)
            {
                view.DataModel = null;
                view.gameObject.SetActive(false);
            }
            
            void OnViewDestroy(UISubView view)
            {
                Destroy(view.gameObject);
            }
        }

        private void OnCollectionReset()
        {
            foreach (var subView in _subViews)
                _subViewPool.Release(subView);
            _subViews.Clear();
        }
        
        private void OnCollectionItemRemove(DataCollection<T> collection, int oldIndex)
        {
            var instance = _subViews[oldIndex];
            _subViews.RemoveAt(oldIndex);
            _subViewPool.Release(instance);
            instance.OnHide();
            OnElementRemoving(instance);
        }

        protected virtual void OnElementRemoving(UISubView subView)
        {
        }
        
        private void OnCollectionItemInsert(DataCollection<T> collection, int newIndex, T newElement)
        {
            var instance = _subViewPool.Get();
            instance.transform.SetSiblingIndex(newIndex);
            instance.DataModel = newElement;
            _subViews.Insert(newIndex, instance);
            instance.OnShow();
            OnElementAdding(instance);
        }

        protected virtual void OnElementAdding(UISubView subView)
        {
        }

        public int GetSubViewCount()
        {
            return _subViews.Count;
        }
        
        public UISubView GetSubView(int index)
        {
            return _subViews[index];
        }

        public int IndexOf(UISubView subView)
        {
            return _subViews.IndexOf(subView);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _worker.SetupCollectionBinding(null);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _worker.SetupCollectionBinding(_itemSource);
        }

        protected virtual void OnDestroy()
        {
            _subViewPool.Dispose();
        }

        private void OnValidate()
        {
            if (root == null)
                root = transform;
        }
    }
}