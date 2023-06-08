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
        [SerializeField]
        private Toggle _toggleShowHealth;
        private Button _buttonSwitch;

        private Vector2 _pointHide;
        private Vector2 _pointShow;
        private Transform _rTransform;

        private bool _isShow;

        public delegate void ShowHealthSliderDelegate(bool on);
        public static event ShowHealthSliderDelegate ShowHealth;


        private void Awake()
        {
            _toggleShowHealth.onValueChanged.AddListener(delegate (bool on) { ShowSlidersHealth(on); });
        }

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

        public void AllKill()
        {
            while (ConfigurationManager.unitsRed.Count > 0)
            {
                var unit = ConfigurationManager.unitsRed[0];
                unit.GetDamage(unit.Health);
            }
            while (ConfigurationManager.unitsGreen.Count > 0)
            {
                var unit = ConfigurationManager.unitsGreen[0];
                unit.GetDamage(unit.Health);
            }
            while (ConfigurationManager.unitsBlue.Count > 0)
            {
                var unit = ConfigurationManager.unitsBlue[0];
                unit.GetDamage(unit.Health);
            }            
        }
        public void ShowSlidersHealth(bool on)
        {
            ShowHealth?.Invoke(on);
        }

    }
}