//    ▄▀▀▀▄▄▄▄▄▄▄▀▀▀▄
//    █▒▒░░░░░░░░░▒▒█    ValueBar.cs
//     █░░█░░░░░█░░█     Created by WanNeng
//  ▄▄  █░░░▀█▀░░░█  ▄▄  Created   03/09/2024 12:56
// █░░█ ▀▄░░░░░░░▄▀ █░░█

using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WanFramework.Utils;

namespace WanFramework.UI.Components
{
    /// <summary>
    /// 数值条
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class ValueBar : MonoBehaviour
    {
        [SerializeField]
        private float max;

        [SerializeField]
        private float current;

        [SerializeField]
        private TMP_Text currentText;
        
        [SerializeField]
        private TMP_Text maxText;

        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void SetValue(float val)
        {
            current = val;
            if (!isActiveAndEnabled) return;
            _slider.value = val;
            using var str = ((int)current).ToStringNoGC();
            currentText.SetText(str);
        }
        public float GetValue() => current;
        public void SetMaxValue(float maxValue)
        {
            max = maxValue;
            if (!isActiveAndEnabled) return;
            using var str = ((int)max).ToStringNoGC();
            maxText.SetText(str);
        }
        private void OnEnable()
        {
            _slider.value = current;
            _slider.maxValue = max;
            using var curStr = ((int)current).ToStringNoGC();
            currentText.SetText(curStr);
            using var maxStr = ((int)max).ToStringNoGC();
            maxText.SetText(maxStr);
        }
    }
}