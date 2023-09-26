namespace EasyMicroservices.Cores.DataTypes
{
    /// <summary>
    /// 
    /// </summary>
    public enum GetUniqueIdentityType : byte
    {
        /// <summary>
        /// value is none, Never use the None to return values
        /// </summary>
        None = 0,
        /// <summary>
        /// error value is default
        /// </summary>
        Default = 1,
        /// <summary>
        /// for the filter values from web admin panel you can sent all for types
        /// </summary>
        All = 2,
        /// <summary>
        /// there is other error that is not in the types
        /// </summary>
        Other = 3,
        /// <summary>
        /// the error type is uknown to us
        /// </summary>
        Unknown = 4,
        /// <summary>
        /// there is nothing to show or validate error
        /// </summary>
        Nothing = 5,
        /// <summary>
        /// 
        /// </summary>
        OnlyParent = 6,
        /// <summary>
        /// 
        /// </summary>
        OnlyChilren = 7
    }
}
