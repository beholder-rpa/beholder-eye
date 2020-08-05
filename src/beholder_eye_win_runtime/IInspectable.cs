namespace SharpGen.Runtime
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// IInspectable used for a C# callback object exposed as WinRT Component.
    /// </summary>
    /// <msdn-id>br205821</msdn-id>
    /// <unamanaged>IInspectable</unamanaged>
    /// <unmanaged-short>IInspectable</unmanaged-short>	
    [Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90")]
    [ShadowAttribute(typeof(InspectableShadow))]
    [ExcludeFromTypeList]
    public interface IInspectable : ICallbackable
    {
    };
}
