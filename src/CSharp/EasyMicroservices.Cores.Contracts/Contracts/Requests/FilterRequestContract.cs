﻿using EasyMicroservices.Cores.DataTypes;
using System;

namespace EasyMicroservices.Cores.Contracts.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterRequestContract
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? IsDeleted { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FromDeletedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ToDeletedDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? FromCreationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ToCreationDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? FromModificationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ToModificationDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UniqueIdentity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public GetUniqueIdentityType? UniqueIdentityType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SortColumnNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDescending { get; set; }
        /// <summary>
        /// text to search
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// en-US, fa-IR
        /// </summary>
        public string Language { get; set; }
    }
}
