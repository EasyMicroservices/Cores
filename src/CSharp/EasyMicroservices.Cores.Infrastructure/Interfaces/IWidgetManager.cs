using System;
using System.Collections.Generic;

namespace EasyMicroservices.Cores.Interfaces;

public interface IWidgetManager
{
    void Register(IWidget widget);
    void UnRegister(IWidget widget);
    IEnumerable<IWidget> GetWidgetsByType(Type type);
    IEnumerable<T> GetWidgets<T>()
        where T : IWidget;
}
