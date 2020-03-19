﻿using System.Threading.Tasks;

namespace Zafiro.UI.Infrastructure.Uno.Navigation
{
    public static class NavigationMixin
    {
        public static Task Go<T>(this INavigation navigation, object parameter = null)
        {
            return navigation.Go(typeof(T), parameter);
        }
    }
}