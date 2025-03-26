
public class SwitchModeCustodian
{
   public ArrowSwitchMode Mode { get; private set; }
   public bool IsStarted { get; private set; }

   public SwitchModeCustodian(ArrowSwitchMode mode)
   {
      IsStarted = true;
      Mode = mode;
   }

   public void SetMode(ArrowSwitchMode mode)
   {
      IsStarted = false;
      Mode = mode;
   }
}