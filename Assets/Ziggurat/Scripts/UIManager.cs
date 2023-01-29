using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panelMain;
        [SerializeField]
        private GameObject _panelRed;
        [SerializeField]
        private GameObject _panelGreen;
        [SerializeField]
        private GameObject _panelBlue;
        [SerializeField]
        private Button _buttonParams;

        private Vector2 _pointUp;
        private Vector2 _pointDown;

        private RectTransform _rTransform;
        
        private bool _isShow;


        private void Awake()
        {
            RayCast.ShowEvent += (colour) => SelectedGate(colour);

        }


        private void Start()
        {
            _isShow = false;
            _rTransform = _panelMain.GetComponent<RectTransform>();
            _pointUp = _rTransform.position;
            _pointDown = _rTransform.position; _pointDown.y -= 540;
            _panelRed.SetActive(true);
            _panelGreen.SetActive(false);
            _panelBlue.SetActive(false);
        }

        private void Update()
        {
            
        }

        private void SelectedGate(Colour colour)
        {
            switch (colour)
            {
                case Colour.Red:
                    _panelRed.SetActive(true);
                    _panelGreen.SetActive(false);
                    _panelBlue.SetActive(false);
                    break;
                case Colour.Green:
                    _panelRed.SetActive(false);
                    _panelGreen.SetActive(true);
                    _panelBlue.SetActive(false);
                    break;
                case Colour.Blue:
                    _panelRed.SetActive(false);
                    _panelGreen.SetActive(false);
                    _panelBlue.SetActive(true);
                    break;                
            }
                
        }

        public void Move()
        {
            StartCoroutine(PanelMove());
        }

        IEnumerator PanelMove()
        {
            _buttonParams.interactable = false;
            if (!_isShow)
            {                
                while (_rTransform.position.y > _pointDown.y + 0.01)
                {
                    _rTransform.position = Vector2.Lerp(_rTransform.position, _pointDown, 0.1f);

                    yield return null;
                }
            }
            if (_isShow)
            {
                while (_rTransform.position.y < _pointUp.y - 0.01)
                {
                    _rTransform.position = Vector2.Lerp(_rTransform.position, _pointUp, 0.1f);

                    yield return null;
                }
            }
            StopCoroutine(PanelMove());
            _buttonParams.interactable = true;
            _isShow = !_isShow;
        }
    }
}