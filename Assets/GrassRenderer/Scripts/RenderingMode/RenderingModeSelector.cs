using System;
using GrassRenderer.DataProxies;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace GrassRenderer.RenderingMode
{
    public class RenderingModeSelector : MonoBehaviour
    {

        [SerializeField] private FieldSelectorButton[] _fieldSelectorButtons;
        [SerializeField] private HeightMapToggle[] _heightMapToggle;

        private RenderingModeDataProxy _renderingModeDataProxy;

        [Inject]
        public void Construct(RenderingModeDataProxy renderingModeDataProxy)
        {
            _renderingModeDataProxy = renderingModeDataProxy;
        }

        private void Awake()
        {
            foreach (FieldSelectorButton fieldSelectorButton in _fieldSelectorButtons)
            {
                fieldSelectorButton.Button.OnClickAsObservable().Subscribe(delegate
                {
                    _renderingModeDataProxy.SetTerrainSize(fieldSelectorButton.Size);
                }).AddTo(this);
            }

            foreach (HeightMapToggle heightMapToggle in _heightMapToggle)
            {
                heightMapToggle.Button.OnClickAsObservable().Subscribe(delegate
                {
                    _renderingModeDataProxy.SetUseHeightMap(heightMapToggle.UseHeightMap);
                }).AddTo(this);
            }
        }

        [Serializable]
        public class FieldSelectorButton
        {

            [field: SerializeField]
            public Button Button { get; private set; }

            [field: SerializeField]
            public int Size { get; private set; }

        }

        [Serializable]
        public class HeightMapToggle
        {

            [field: SerializeField]
            public Button Button { get; private set; }

            [field: SerializeField]
            public bool UseHeightMap { get; private set; }

        }

    }
}
