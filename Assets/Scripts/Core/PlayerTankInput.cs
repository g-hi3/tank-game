﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Core
{
    public class PlayerTankInput : MonoBehaviour
    {
        private const string ActionNameMove = "Move";
        private const string ActionNameLook = "Look";
        private const string ActionNamePoint = "Point";
        private const string ActionNameShoot = "Shoot";
        private const string ActionNameBomb = "Bomb";
        private PlayerInput _playerInput;
        private Camera _mainCamera;

        [field: SerializeField] public TankController Tank { get; private set; }

        private void RaiseTankMovingStarted()
        {
            Tank.StartMoving();
        }

        private void RaiseTankMoved(InputAction.CallbackContext context)
        {
            var moveDirection = context.ReadValue<Vector2>();
            Tank.Move(moveDirection);
        }

        private void RaiseTankMovingCanceled()
        {
            Tank.StopMoving();
        }

        private void RaiseTankLooked(InputAction.CallbackContext context)
        {
            var lookDirection = context.ReadValue<Vector2>();
            Tank.Look(lookDirection);
        }

        private void RaiseTankLookedAt(InputAction.CallbackContext context)
        {
            var pointPosition = context.ReadValue<Vector2>();
            var lookPosition = _mainCamera.ScreenToWorldPoint(pointPosition);
            Tank.LookAt(lookPosition);
        }

        private void RaiseTankShot()
        {
            Tank.Shoot();
        }

        private void RaiseTankBombed() 
        {
            Tank.Bomb();
        }

        private void DelegateTriggeredAction(InputAction.CallbackContext context)
        {
            if (context.started)
                DelegateStartedAction(context);
            else if (context.performed)
                DelegatePerformedAction(context);
            else if (context.canceled)
                DelegateCanceledAction(context);
        }

        private void DelegateStartedAction(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case ActionNameMove:
                    RaiseTankMovingStarted();
                    break;
                case ActionNameBomb:
                    RaiseTankBombed();
                    break;
            }
        }

        private void DelegatePerformedAction(InputAction.CallbackContext context)
        {
            switch (context.action.name)
            {
                case ActionNameMove:
                    RaiseTankMoved(context);
                    break;
                case ActionNameLook:
                    RaiseTankLooked(context);
                    break;
                case ActionNamePoint:
                    RaiseTankLookedAt(context);
                    break;
                case ActionNameShoot:
                    RaiseTankShot();
                    break;
            }
        }

        private void DelegateCanceledAction(InputAction.CallbackContext context)
        {
            if (context.action.name == ActionNameMove)
                RaiseTankMovingCanceled();
        }

        private void Awake()
        {
            Tank = GetComponent<TankController>();
        }

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.onActionTriggered += DelegateTriggeredAction;
            _mainCamera = Camera.main;
        }

        private void OnDestroy()
        {
            _playerInput.onActionTriggered -= DelegateTriggeredAction;
        }
    }
}
