using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ziggurat
{
    public class RayCast : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField]
        private InputAction _inputAction;

        [SerializeField]
        private LayerMask _mask;

        public delegate void ShowGateParamsDelegate(Colour colour);
        public static event ShowGateParamsDelegate ShowEvent;

        private void Awake()
        {
            _inputAction.Enable();
        }

        private void OnEnable()
        {
            _inputAction.performed += _ => SelectGate();            
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        //отлавливает выбор зиккурата для отображения статистики и настройки зиккурата
        private void SelectGate()
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var Gate, 1000000f, 1<<(int)Masks.Gate))
            {
                ShowEvent?.Invoke(Gate.transform.parent.transform.GetComponent<GateAssistant>().Get_gateColour);
            }
        }
    }
}