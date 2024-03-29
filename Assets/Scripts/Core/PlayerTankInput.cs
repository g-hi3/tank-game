﻿using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Core
{
    /// <summary>
    /// This component delegates the input from the player to their tank.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(TankController))]
    public class PlayerTankInput : MonoBehaviour
    {
        private const string ActionNameMove = "Move";
        private const string ActionNameLook = "Look";
        private const string ActionNamePoint = "Point";
        private const string ActionNameShoot = "Shoot";
        private const string ActionNameBomb = "Bomb";
        private const string ActionNameTogglePause = "Toggle Pause";
        [NotNull] private PlayerInput _playerInput = default!;
        private bool _paused;

        /// <summary>
        /// Represents the player's tank.
        /// </summary>
        [NotNull]
        [field: SerializeField]
        public TankController Tank { get; private set; } = default!;

        private void RaiseTankMovingStarted()
        {
            if (_paused)
                return;

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
            var camera = FindObjectOfType<Camera>();
            if (camera == null)
                return;

            var pointPosition = context.ReadValue<Vector2>();
            var lookPosition = camera.ScreenToWorldPoint(pointPosition);
            Tank.LookAt(lookPosition);
        }

        private void RaiseTankShot()
        {
            Tank.Shoot();
        }

        private void RaiseTankBombed()
        {
            if (_paused)
                return;

            Tank.Bomb();
        }

        private static void RaisePauseToggled()
        {
            if (GameManager.Instance.Paused)
                GameManager.Instance.ResumeGame();
            else
                GameManager.Instance.PauseGame();
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
                case ActionNameTogglePause:
                    RaisePauseToggled();
                    break;
            }
        }

        private void DelegatePerformedAction(InputAction.CallbackContext context)
        {
            if (_paused)
                return;

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
            if (_paused)
                return;

            if (context.action.name == ActionNameMove)
                RaiseTankMovingCanceled();
        }

        private void Awake()
        {
            Tank ??= GetComponent<TankController>()
                     ?? throw new InvalidOperationException($"Missing {nameof(TankController)} component!");
        }

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>()
                           ?? throw new InvalidOperationException($"Missing {nameof(PlayerInput)} component!");
            _playerInput.onActionTriggered += DelegateTriggeredAction;
        }

        [UsedImplicitly]
        private void OnPause()
        {
            _paused = true;
        }

        [UsedImplicitly]
        private void OnResume()
        {
            _paused = false;
        }

        private void OnDestroy()
        {
            _playerInput.onActionTriggered -= DelegateTriggeredAction;
        }
    }
}
