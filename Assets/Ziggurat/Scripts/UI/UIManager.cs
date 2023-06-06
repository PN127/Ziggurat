using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ziggurat
{
    class MovingPanel : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _panelMain;
        [SerializeField]
        protected GameObject _panelPointHide;
        [SerializeField]
        protected GameObject _panelPointShow;
        [SerializeField]
        protected Button _buttonSwitch;

        protected UIManager uIManager;

        protected virtual void Start()
        {
            uIManager = new UIManager();
            uIManager.ParamsTransfer(_panelMain, _buttonSwitch);
        }

        public void panelMovement()
        {
            StartCoroutine(uIManager.PanelMove(_panelPointHide, _panelPointShow));
            StopCoroutine(uIManager.PanelMove(_panelPointHide, _panelPointShow));
        }
    }

    public class UIManager : MonoBehaviour
    {        
        private Button _buttonSwitch;

        private Vector2 _pointHide;
        private Vector2 _pointShow;
        private Transform _rTransform;
        
        private bool _isShow;

        public void ParamsTransfer(GameObject PanelMain, Button ButtonSwitch)
        { 
        _isShow = false;
        _rTransform = PanelMain.GetComponent<RectTransform>();
        
        _buttonSwitch = ButtonSwitch;
        }

        public IEnumerator PanelMove(GameObject PanelPointHide, GameObject PanelPointShow)
        {
            _pointHide = PanelPointHide.GetComponent<RectTransform>().position;
            _pointShow = PanelPointShow.GetComponent<RectTransform>().position;
            _buttonSwitch.interactable = false;

            if (!_isShow)
            {                
                while (Vector2.Distance(_pointShow, _rTransform.position) > 0.01f)
                {
                    _rTransform.position = Vector2.Lerp(_rTransform.position, _pointShow, 0.1f);
                    yield return null;
                }                
            }

            if (_isShow)
            {
                while(Vector2.Distance(_pointHide, _rTransform.position) > 0.01f)
                {
                    _rTransform.position = Vector2.Lerp(_rTransform.position, _pointHide, 0.1f);
                    yield return null;
                }
            }
            _buttonSwitch.interactable = true;
            _isShow = !_isShow;
        }

        //public IEnumerator PanelMove()
        //{
        //    _buttonSwitch.interactable = false;
        //    if (!_isShow)
        //    {                
        //        while (_rTransform.position.y > _pointShow.y + 0.01)
        //        {
        //            _rTransform.position = Vector2.Lerp(_rTransform.position, _pointShow, 0.1f);

        //            yield return null;
        //        }
        //    }
        //    if (_isShow)
        //    {
        //        while (_rTransform.position.y < _pointHide.y - 0.01)
        //        {
        //            _rTransform.position = Vector2.Lerp(_rTransform.position, _pointHide, 0.1f);

        //            yield return null;
        //        }
        //    }
        //    _buttonSwitch.interactable = true;
        //    _isShow = !_isShow;
        //}

    }
}