﻿using System.Threading.Tasks;

namespace Zafiro.UI;

public interface INotificationService
{
    Task Show(string message);
}