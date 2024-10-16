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

        [SerializeField]
        string m_yellowFishSpriteAddress;
        private AsyncOperationHandle<Sprite> m_FishLoadOpHandle;

        private void OnEnable()
        {
            m_FishLoadOpHandle = Addressables.LoadAssetAsync<Sprite>(m_yellowFishSpriteAddress);
            m_FishLoadOpHandle.Completed += OnFishSpriteLoadComplete;
        }

        private void OnDisable()
        {
            m_FishLoadOpHandle.Completed -= OnFishSpriteLoadComplete;
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

            if (_colorSpriteDict.ContainsKey(newColor))
            {
                _sprite.sprite = _colorSpriteDict[newColor];
            }
        }

        void OnFishSpriteLoadComplete(AsyncOperationHandle<Sprite> asyncOperationHandle)
        {
            for(int i = 0; i < NumColors; i++)
            {
                if(colorSprites[i].color == ColorType.Yellow) {
                    colorSprites[i].sprite = asyncOperationHandle.Result;
                }
            }
            UpdateColorSpriteDict();
        }
    }
}
