﻿using System;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Interfaces;
public interface IWidget
{
    Type GetObjectType();
}

public interface IWidget<T>: IWidget
{
    Task Initialize(params T[] parameters);
}