
public interface UserInterface
{
    public bool IsOpen { get; set; }
    void SetEnablity(bool setTo)
    {
        UserInterface ui = CanvasManager.CurrentInterface;

        if (IsOpen == setTo) return;
        if (ui != null && ui != this) return;

        IsOpen = setTo;
        CanvasManager.CurrentInterface = setTo ? this : null;
        OnEnablityChanged(setTo);
    }
    void OnEnablityChanged(bool changedTo);
    void SetEnablityGetter(bool setTo);
    //public void SetEnablityGetter(bool setTo)
    //{
    //    UserInterface ui = this as UserInterface;
    //    ui.SetEnablity(setTo);
    //}
}
