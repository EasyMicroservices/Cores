using EasyMicroservices.Cores.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyMicroservices.Cores.Widgets;
public class WidgetManager : IWidgetManager
{
    public WidgetManager(IWidgetBuilder widgetBuilder)
    {
        widgetBuilder.Build(this);
    }

    readonly ConcurrentDictionary<Type, List<IWidget>> Widgets = new ConcurrentDictionary<Type, List<IWidget>>();


    public void Register(IWidget widget)
    {
        var type = widget.GetObjectType();
        if (Widgets.TryGetValue(type, out List<IWidget> widgets))
            widgets.Add(widget);
        else
        {
            Widgets.TryAdd(widget.GetObjectType(), new List<IWidget>()
            {
                widget
            });
        }
    }

    public void UnRegister(IWidget widget)
    {
        Widgets.TryRemove(widget.GetObjectType(), out _);
    }

    public IEnumerable<IWidget> GetWidgetsByType(Type type)
    {
        if (Widgets.TryGetValue(type, out List<IWidget> widgets))
            return widgets.ToList();
        return Enumerable.Empty<IWidget>();
    }

    public IEnumerable<T> GetWidgets<T>() where T : IWidget
    {
        if (Widgets.TryGetValue(typeof(T), out List<IWidget> widgets))
            return widgets.Cast<T>().ToList();
        return Enumerable.Empty<T>();
    }
}
