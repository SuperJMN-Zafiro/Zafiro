using System;

namespace Zafiro.Core
{
    public class DialogButton 
    {
        public string Text { get; set; }
        public bool IsDefault { get; set; }
        public Action<IPopup> Handler { get; set; }
        public IObservable<bool> CanExecute { get; set; }
    }
}