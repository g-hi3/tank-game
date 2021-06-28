using UnityEngine;

public class TankMovingStartedEventArgs {
}

public class TankMovedEventArgs {
  
  public Vector2 MoveDirection { get; set; }
  
}

public class TankMovingCanceledEventArgs {
}

public class TankLookedEventArgs {
  
  public Vector2 LookDirection { get; set; }
  
}

public class TankLookedAtEventArgs {
  
  public Vector2 LookPosition { get; set; }

}

public class TankShotEventArgs { 
}

public class TankBombedEventArgs {
}

public delegate void OnTankMovingStarted(object sender, TankMovingStartedEventArgs eventArgs);

public delegate void OnTankMoved(object sender, TankMovedEventArgs eventArgs);

public delegate void OnTankMovingCanceled(object sender, TankMovingCanceledEventArgs eventArgs);

public delegate void OnTankLooked(object sender, TankLookedEventArgs eventArgs);

public delegate void OnTankLookedAt(object sender, TankLookedAtEventArgs eventArgs);

public delegate void OnTankShot(object sender, TankShotEventArgs eventArgs);

public delegate void OnTankBombed(object sender, TankBombedEventArgs eventArgs);

public interface ITankInput {

  event OnTankMovingStarted TankMovingStarted;
  event OnTankMoved TankMoved;
  event OnTankMovingCanceled TankMovingCanceled;
  event OnTankLooked TankLooked;
  event OnTankLookedAt TankLookedAt;
  event OnTankShot TankShot;
  event OnTankBombed TankBombed;
  
}