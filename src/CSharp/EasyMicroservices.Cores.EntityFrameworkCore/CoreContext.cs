using EasyMicroservices.Cores.Database.Entities;
using EasyMicroservices.Cores.EntityFrameworkCore.Builders;
using EasyMicroservices.Cores.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace EasyMicroservices.Cores.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class CoreContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new CoreModelBuilder().OnModelCreating(modelBuilder);
        }
    }
}
