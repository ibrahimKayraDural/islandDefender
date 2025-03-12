
using System.Collections.Generic;

public interface IUserInterface
{
    public bool IsOpen { get; set; }
    void SetEnablity(bool setTo, List<string> optionalParameters)
    {
        IUserInterface ui = CanvasManager.CurrentInterface;

        if (IsOpen == setTo) return;
        if (ui != null && ui != this) return;

        IsOpen = setTo;
        CanvasManager.CurrentInterface = setTo ? this : null;
        OnEnablityChanged(setTo, optionalParameters);
    }
    abstract void OnEnablityChanged(bool changedTo, List<string> optionalParameters);
    void SetEnablityGetter(bool setTo, List<string> optionalParameters);

    //public void SetEnablityGetter(bool setTo)
    //{
    //    UserInterface ui = this as UserInterface;
    //    ui.SetEnablity(setTo);
    //}
}
