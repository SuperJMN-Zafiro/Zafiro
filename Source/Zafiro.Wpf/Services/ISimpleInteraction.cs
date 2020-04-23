using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zafiro.Core;

namespace Zafiro.Wpf.Services
{
    public interface ISimpleInteraction
    {
        void Register(string token, Type viewType);
        Task Interact(string key, string title, object vm, ICollection<DialogButton> buttons);
    }
}