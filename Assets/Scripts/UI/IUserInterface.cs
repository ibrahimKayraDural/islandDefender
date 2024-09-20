
public interface IUserInterface
{
    public bool IsOpen { get; set; }
    void SetEnablity(bool setTo)
    {
        IUserInterface ui = CanvasManager.CurrentInterface;

        if (IsOpen == setTo) return;
        if (ui != null && ui != this) return;

        IsOpen = setTo;
        CanvasManager.CurrentInterface = setTo ? this : null;
        OnEnablityChanged(setTo);
    }
    abstract void OnEnablityChanged(bool changedTo);
    void SetEnablityGetter(bool setTo);
    //public void SetEnablityGetter(bool setTo)
    //{
    //    UserInterface ui = this as UserInterface;
    //    ui.SetEnablity(setTo);
    //}
}
