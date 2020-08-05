namespace SharpGen.Runtime
{
    /// <summary>
    /// Base interface for Component Object Model (COM).
    /// </summary>
    [Shadow(typeof(ComObjectShadow))]
    [ExcludeFromTypeList]
    public partial interface IUnknown
    {
        /// <summary>
        /// Increments the reference count for an interface on this instance.
        /// </summary>
        /// <returns>The method returns the new reference count.</returns>
        uint AddRef();

        /// <summary>
        /// Decrements the reference count for an interface on this instance.
        /// </summary>
        /// <returns>The method returns the new reference count.</returns>
        uint Release();
    }
}
