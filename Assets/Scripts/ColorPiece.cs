using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Match3
{
    public class ColorPiece : MonoBehaviour
    {
        [System.Serializable]
        public struct ColorSprite
        {
            public ColorType color;
            public Sprite sprite;
        }

        public ColorSprite[] colorSprites;

        private ColorType _color;

        public ColorType Color
        {
            get => _color;
            set => SetColor(value);
        }

        public int NumColors => colorSprites.Length;

        private SpriteRenderer _sprite;
        private Dictionary<ColorType, Sprite> _colorSpriteDict;

        // Addressable asset loading
        private List<string> m_AssetLabels = new List<string>() {"Fish_Icons"};
        private AsyncOperationHandle<IList<Sprite>> m_FishesLoadOpHandle;
        private Dictionary<string, ColorType> m_FishAssetNameColorPairs = new Dictionary<string, ColorType>
        {
            { Constants.Addressables.yellowFishAssetName, ColorType.Yellow },
            { Constants.Addressables.purpleFishAssetName, ColorType.Purple }
        };

        private void OnEnable()
        {
            LoadFishSprites();
        }

        private void OnDisable()
        {
            m_FishesLoadOpHandle.Completed -= OnFishSpritesLoadComplete;
        }

        private void Awake()
        {
            _sprite = transform.Find("piece").GetComponent<SpriteRenderer>();

            UpdateColorSpriteDict();
        }

        void UpdateColorSpriteDict()
        {
            // instantiating and populating a Dictionary of all Color Types / Sprites (for fast lookup)
            _colorSpriteDict = new Dictionary<ColorType, Sprite>();

            for (int i = 0; i < colorSprites.Length; i++)
            {
                if (!_colorSpriteDict.ContainsKey(colorSprites[i].color))
                {
                    _colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
                }
            }
            UpdateSpritewrtColor();
        }

        void UpdateSpritewrtColor()
        {
            if (_colorSpriteDict.ContainsKey(_color))
            {
                _sprite.sprite = _colorSpriteDict[_color];
            }
        }

        public void SetColor(ColorType newColor)
        {
            _color = newColor;

            UpdateSpritewrtColor();
        }

        void LoadFishSprites()
        {
            m_FishesLoadOpHandle = Addressables.LoadAssetsAsync<Sprite>(m_AssetLabels, null, Addressables.MergeMode.Union);
            m_FishesLoadOpHandle.Completed += OnFishSpritesLoadComplete;
        }

        void OnFishSpritesLoadComplete(AsyncOperationHandle<IList<Sprite>> asyncOperationHandle)
        {
            Debug.Log("OnFishSpritesLoad Status: " + asyncOperationHandle.Status);

            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<Sprite> results = asyncOperationHandle.Result;
                for (int i = 0; i < results.Count; i++)
                {
                    string loadedSpriteName = results[i].name;
                    ColorType respectiveColor = m_FishAssetNameColorPairs[loadedSpriteName];

                    if (respectiveColor == ColorType.Any || _colorSpriteDict == null)
                    {
                        continue;
                    }
                    if (!_colorSpriteDict.ContainsKey(respectiveColor))
                    {
                        _colorSpriteDict.Add(respectiveColor, results[i]);
                    }
                    else
                    {
                        _colorSpriteDict[respectiveColor] = results[i];
                    }
                }
            }

            UpdateSpritewrtColor();

        }
    }
}
