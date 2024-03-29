﻿using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.AspEntityFrameworkCoreApi.Interfaces;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhiteLables.GeneratedServices;

namespace EasyMicroservices.Cores.AspCoreApi.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class WhiteLabelManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public WhiteLabelManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 
        /// </summary>
        public bool IsInitialized { get; set; }

        WhiteLabelInfo _CurrentWhiteLabel;
        /// <summary>
        /// 
        /// </summary>
        public WhiteLabelInfo CurrentWhiteLabel
        {
            get
            {
                if (_CurrentWhiteLabel is null)
                    throw new Exception("Whitelabel is not intialized!");
                return _CurrentWhiteLabel;
            }
            private set
            {
                _CurrentWhiteLabel = value;
            }
        }

        string GetDefaultUniqueIdentity(ICollection<WhiteLabelContract> whiteLabels, long? parentId)
        {
            var found = whiteLabels.FirstOrDefault(x => x.ParentId == parentId);
            if (found == null)
            {
                return "";
            }
            return $"{DefaultUniqueIdentityManager.GenerateUniqueIdentity(found.Id)}-{GetDefaultUniqueIdentity(whiteLabels, found.Id)}".Trim('-');
        }

        static SemaphoreSlim SemaphoreSlim { get; set; } = new SemaphoreSlim(1);
        static HttpClient WhiteLabelHttpClient = new HttpClient();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="microserviceName"></param>
        /// <param name="whiteLableRoute"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        public async Task<WhiteLabelInfo> Initialize(IHttpContextAccessor httpContext, string microserviceName, string whiteLableRoute, params Type[] dbContextTypes)
        {
            try
            {
                await SemaphoreSlim.WaitAsync();
                if (IsInitialized)
                    return CurrentWhiteLabel;
                microserviceName.ThrowIfNullOrEmpty(nameof(microserviceName));
                microserviceName.ThrowIfNullOrEmpty(nameof(whiteLableRoute));
                Console.WriteLine($"WhiteLabelManager Initialize! {microserviceName} {whiteLableRoute}");
                if (dbContextTypes.IsEmpty())
                    return CurrentWhiteLabel;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                    var ownerPat = unitOfWork.GetFullAccessPersonalAccessToken();
                    if (ownerPat.HasValue())
                        WhiteLabelHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ownerPat);
                    else if (httpContext?.HttpContext != null && httpContext.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                        WhiteLabelHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationHeader.ToString().Replace("Bearer ", ""));
                }

                var whiteLabelClient = new WhiteLables.GeneratedServices.WhiteLabelClient(whiteLableRoute, WhiteLabelHttpClient);
                var whiteLabels = await whiteLabelClient.GetAllAsync().AsCheckedResult(x => x.Result).ConfigureAwait(false);
                var findParentBussinessId = whiteLabels.FirstOrDefault(x => x.Name.Equals(microserviceName, StringComparison.OrdinalIgnoreCase))?.Id;
                string defaultUniqueIdentity;
                if (findParentBussinessId.HasValue)
                    defaultUniqueIdentity = $"{DefaultUniqueIdentityManager.GenerateUniqueIdentity(findParentBussinessId.Value)}-{GetDefaultUniqueIdentity(whiteLabels, findParentBussinessId)}".Trim('-');
                else
                    defaultUniqueIdentity = GetDefaultUniqueIdentity(whiteLabels, findParentBussinessId);

                var microserviceClient = new WhiteLables.GeneratedServices.MicroserviceClient(whiteLableRoute, WhiteLabelHttpClient);
                var microservices = await microserviceClient.GetAllAsync().ConfigureAwait(false);
                var foundMicroservice = microservices.Result.FirstOrDefault(x => x.Name.Equals(microserviceName, StringComparison.OrdinalIgnoreCase));
                if (foundMicroservice == null)
                {
                    foundMicroservice = new WhiteLables.GeneratedServices.MicroserviceContract()
                    {
                        InstanceIndex = 1,
                        Name = microserviceName,
                        Description = "Automatically added"
                    };
                    var addMicroservice = await microserviceClient.AddAsync(foundMicroservice).ConfigureAwait(false);
                    foundMicroservice.Id = addMicroservice.Result;
                }

                CurrentWhiteLabel = new WhiteLabelInfo()
                {
                    MicroserviceId = foundMicroservice.Id,
                    MicroserviceName = microserviceName,
                    StartUniqueIdentity = defaultUniqueIdentity
                };

                var uniqueIdentityManager = new UnitOfWork(_serviceProvider).GetUniqueIdentityManager() as DefaultUniqueIdentityManager;

                var microserviceContextTableClient = new WhiteLables.GeneratedServices.MicroserviceContextTableClient(whiteLableRoute, WhiteLabelHttpClient);
                var microserviceContextTables = await microserviceContextTableClient.GetAllAsync().ConfigureAwait(false);

                HashSet<string> addedInWhitLabels = new HashSet<string>();
                foreach (var contextTableContract in microserviceContextTables.Result)
                {
                    uniqueIdentityManager.InitializeTables(contextTableContract.MicroserviceId, contextTableContract.ContextName, contextTableContract.TableName, contextTableContract.ContextTableId);
                    addedInWhitLabels.Add(uniqueIdentityManager.GetContextTableName(contextTableContract.MicroserviceId, contextTableContract.ContextName, contextTableContract.TableName));
                }

                foreach (var contextType in dbContextTypes)
                {
                    var contextTableClient = new WhiteLables.GeneratedServices.ContextTableClient(whiteLableRoute, WhiteLabelHttpClient);
                    var contextTables = await contextTableClient.GetAllAsync().ConfigureAwait(false);
                    using var instanceOfContext = _serviceProvider.GetService(contextType) as DbContext;
                    string contextName = uniqueIdentityManager.GetContextName(contextType);
                    foreach (var entityType in instanceOfContext.Model.GetEntityTypes())
                    {
                        string tableName = entityType.GetTableName();
                        uniqueIdentityManager.AddTableName(entityType.ClrType, tableName);
                        var tableFullName = uniqueIdentityManager.GetContextTableName(foundMicroservice.Id, contextType.Name, tableName);
                        if (!addedInWhitLabels.Contains(tableFullName))
                        {
                            if (microserviceContextTables.Result.Any(x => x.ContextName == contextName && x.TableName == tableName && x.MicroserviceId == foundMicroservice.Id))
                                continue;
                            var contextTable = contextTables.Result.FirstOrDefault(x => x.ContextName == contextName && x.TableName == tableName);
                            if (contextTable == null)
                            {
                                contextTable = new WhiteLables.GeneratedServices.ContextTableContract()
                                {
                                    ContextName = contextName,
                                    TableName = tableName,
                                };
                                var contextTableResult = await contextTableClient.AddAsync(contextTable).ConfigureAwait(false);
                                contextTable.Id = contextTableResult.Result;
                            }
                            var addedMicroserviceContextTable = await microserviceContextTableClient.AddAsync(new WhiteLables.GeneratedServices.MicroserviceContextTableContract()
                            {
                                ContextName = contextName,
                                TableName = tableName,
                                MicroserviceName = microserviceName,
                                MicroserviceId = foundMicroservice.Id,
                                ContextTableId = contextTable.Id
                            }).ConfigureAwait(false);
                            uniqueIdentityManager.InitializeTables(foundMicroservice.Id, contextName, tableName, contextTable.Id);
                        }
                    }
                }
                IsInitialized = true;
                return CurrentWhiteLabel;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
