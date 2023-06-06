using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ziggurat
{
    class Panel_Settings : MovingPanel
    {
        [SerializeField]
        private GameObject _panelRed;
        [SerializeField]
        private GameObject _panelGreen;
        [SerializeField]
        private GameObject _panelBlue;       

        private void Awake()
        {
            RayCast.ShowEvent += (colour) => SelectedGate(colour);
        }

        protected override void Start()
        {
            uIManager = new UIManager();
            uIManager.ParamsTransfer(_panelMain, _buttonSwitch); 

            SelectedGate(Colour.Red);
        }

        private void SelectedGate(Colour colour)
        {
            _panelRed.SetActive(false);
            _panelGreen.SetActive(false);
            _panelBlue.SetActive(false);

            switch (colour)
            {
                case Colour.Red:
                    _panelRed.SetActive(true);
                    break;
                case Colour.Green:
                    _panelGreen.SetActive(true);
                    break;
                case Colour.Blue:
                    _panelBlue.SetActive(true);
                    break;
            }

        }
    }
}
