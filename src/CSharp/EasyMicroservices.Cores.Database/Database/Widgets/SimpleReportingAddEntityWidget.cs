﻿using EasyMicroservices.Cores.Database.Helpers;
using EasyMicroservices.Cores.Database.Interfaces;
using EasyMicroservices.Cores.Interfaces;
using EasyMicroservices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMicroservices.Cores.Database.Widgets;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TReportEntity"></typeparam>
/// <typeparam name="TObjectContract"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class SimpleReportingAddEntityWidget<TEntity, TReportEntity, TObjectContract> : SimpleReportingBaseEntityWidget, IDatabaseWidget<TEntity, TObjectContract>
    where TReportEntity : class
    where TEntity : class
    where TObjectContract : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Type GetObjectType()
    {
        return typeof(TObjectContract);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Initialize(params TObjectContract[] parameters)
    {
        return TaskHelper.GetCompletedTask();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="entity"></param>
    /// <param name="contract"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task AddProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, TEntity entity, TObjectContract contract, CancellationToken cancellationToken = default)
    {
        var reportEntity = await baseUnitOfWork
            .GetMapper()
            .MapAsync<TReportEntity>(contract);
        var logic = baseUnitOfWork.GetLogic<TReportEntity>(new Models.LogicOptions()
        {
            DoStopReporting = true
        });
        DatabaseExtensions.SetIdToRecordId(logic.GetReadableContext(), entity, reportEntity);
        if (reportEntity is IUniqueIdentitySchema reportUniqueIdentity && entity is IUniqueIdentitySchema entityUniqueIdentity)
            reportUniqueIdentity.UniqueIdentity = entityUniqueIdentity.UniqueIdentity;
        await logic
            .Add(reportEntity, cancellationToken)
            .AsCheckedResult();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseWidgetManager"></param>
    /// <param name="baseUnitOfWork"></param>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddBulkProcess(IDatabaseWidgetManager databaseWidgetManager, IBaseUnitOfWork baseUnitOfWork, Dictionary<TObjectContract, TEntity> items, CancellationToken cancellationToken = default)
    {
        var reportEntities = await baseUnitOfWork
            .GetMapper()
            .MapToDictionaryAsync<TObjectContract, TReportEntity>(items.Keys);
        var logic = baseUnitOfWork.GetLogic<TReportEntity>(new Models.LogicOptions()
        {
            DoStopReporting = true
        });
        DatabaseExtensions.SetIdToRecordId(logic.GetReadableContext(), reportEntities);
        foreach (var item in reportEntities)
        {
            DatabaseExtensions.SetIdToRecordId(logic.GetReadableContext(), item.Key, item.Value);
            if (item.Key is IUniqueIdentitySchema reportUniqueIdentity && item.Value is IUniqueIdentitySchema entityUniqueIdentity)
                reportUniqueIdentity.UniqueIdentity = entityUniqueIdentity.UniqueIdentity;
        }

        await logic
            .AddBulk(reportEntities.Values.ToList(), cancellationToken)
            .AsCheckedResult();
    }
}