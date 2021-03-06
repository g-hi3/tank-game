using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTankInput : MonoBehaviour
{
    private const string ActionNameMove = "Move";
    private const string ActionNameLook = "Look";
    private const string ActionNamePoint = "Point";
    private const string ActionNameShoot = "Shoot";
    private const string ActionNameBomb = "Bomb";
    private PlayerInput _playerInput;
    private Camera _mainCamera;

    private void RaiseTankMovingStarted()
    {
        SendMessage("StartMoving");
    }

    private void RaiseTankMoved(Vector2 moveDirection)
    {
        SendMessage("Move", (Vector3)moveDirection);
    }

    private void RaiseTankMovingCanceled()
    {
        SendMessage("StopMoving");
    }

    private void RaiseTankLooked(Vector2 lookDirection)
    {
        SendMessage("Look", lookDirection);
    }

    private void RaiseTankLookedAt(Vector2 lookPosition)
    {
        SendMessage("LookAt", (Vector3)lookPosition);
    }

    private void RaiseTankShot()
    {
        SendMessage("Shoot");
    }

    private void RaiseTankBombed() 
    {
        SendMessage("Bomb");
    }

    private void DelegateTriggeredAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DelegateStartedAction(context);
        }
        else if (context.performed)
        {
            DelegatePerformedAction(context);
        }
        else if (context.canceled)
        {
            DelegateCanceledAction(context);
        }
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
                var moveDirection = context.ReadValue<Vector2>();
                RaiseTankMoved(moveDirection);
                break;
            case ActionNameLook:
                var lookDirection = context.ReadValue<Vector2>();
                RaiseTankLooked(lookDirection);
                break;
            case ActionNamePoint:
                var pointPosition = context.ReadValue<Vector2>();
                var worldPoint = _mainCamera.ScreenToWorldPoint(pointPosition);
                RaiseTankLookedAt(worldPoint);
                break;
            case ActionNameShoot:
                RaiseTankShot();
                break;
        }
    }

    private void DelegateCanceledAction(InputAction.CallbackContext context)
    {
        if (context.action.name == ActionNameMove)
        {
            RaiseTankMovingCanceled();
        }
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
