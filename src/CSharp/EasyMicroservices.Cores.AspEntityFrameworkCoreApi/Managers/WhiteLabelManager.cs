using EasyMicroservices.Cores.AspEntityFrameworkCoreApi;
using EasyMicroservices.Cores.Database.Managers;
using EasyMicroservices.Cores.Models;
using EasyMicroservices.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WhiteLables.GeneratedServices;

namespace EasyMicroservices.Cores.AspCoreApi.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class WhiteLabelManager
    {
        internal WhiteLabelManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 
        /// </summary>
        public HttpClient HttpClient { get; set; } = new HttpClient();
        /// <summary>
        /// 
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return _CurrentWhiteLabel is not null;
            }
        }

        static WhiteLabelInfo _CurrentWhiteLabel;
        /// <summary>
        /// 
        /// </summary>
        public static WhiteLabelInfo CurrentWhiteLabel
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

        string GetDefaultUniqueIdentity(ICollection<WhiteLabelContract> whiteLables, long? parentId)
        {
            var found = whiteLables.FirstOrDefault(x => x.ParentId == parentId);
            if (found == null)
            {
                return "";
            }
            return $"{DefaultUniqueIdentityManager.GenerateUniqueIdentity(found.Id)}-{GetDefaultUniqueIdentity(whiteLables, found.Id)}".Trim('-');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="microserviceName"></param>
        /// <param name="whiteLableRoute"></param>
        /// <param name="dbContextTypes"></param>
        /// <returns></returns>
        public async Task<WhiteLabelInfo> Initialize(string microserviceName, string whiteLableRoute, params Type[] dbContextTypes)
        {
            if (IsInitialized)
                return CurrentWhiteLabel;
            microserviceName.ThrowIfNullOrEmpty(nameof(microserviceName));
            microserviceName.ThrowIfNullOrEmpty(nameof(whiteLableRoute));
            Console.WriteLine($"WhiteLabelManager Initialize! {microserviceName} {whiteLableRoute}");
            if (dbContextTypes.IsEmpty())
                return CurrentWhiteLabel;
            var whiteLabelClient = new WhiteLables.GeneratedServices.WhiteLabelClient(whiteLableRoute, HttpClient);
            var whiteLabels = await whiteLabelClient.GetAllAsync().AsCheckedResult(x => x.Result).ConfigureAwait(false);
            var defaultUniqueIdentity = GetDefaultUniqueIdentity(whiteLabels, null);

            var microserviceClient = new WhiteLables.GeneratedServices.MicroserviceClient(whiteLableRoute, HttpClient);
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

            var microserviceContextTableClient = new WhiteLables.GeneratedServices.MicroserviceContextTableClient(whiteLableRoute, HttpClient);
            var microserviceContextTables = await microserviceContextTableClient.GetAllAsync().ConfigureAwait(false);

            HashSet<string> addedInWhitLabels = new HashSet<string>();
            foreach (var contextTableContract in microserviceContextTables.Result)
            {
                uniqueIdentityManager.InitializeTables(contextTableContract.MicroserviceId, contextTableContract.ContextName, contextTableContract.TableName, contextTableContract.ContextTableId);
                addedInWhitLabels.Add(uniqueIdentityManager.GetContextTableName(contextTableContract.MicroserviceId, contextTableContract.ContextName, contextTableContract.TableName));
            }

            foreach (var contextType in dbContextTypes)
            {
                var contextTableClient = new WhiteLables.GeneratedServices.ContextTableClient(whiteLableRoute, HttpClient);
                var contextTables = await contextTableClient.GetAllAsync().ConfigureAwait(false);
                using var instanceOfContext = _serviceProvider.GetService(contextType) as DbContext;
                string contextName = uniqueIdentityManager.GetContextName(contextType);
                foreach (var entityType in instanceOfContext.Model.GetEntityTypes())
                {
                    string tableName = entityType.ServiceOnlyConstructorBinding.RuntimeType.Name;
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
            return CurrentWhiteLabel;
        }
    }
}
