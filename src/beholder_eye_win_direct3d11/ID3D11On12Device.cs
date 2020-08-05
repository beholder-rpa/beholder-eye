﻿namespace beholder_eye_win.Direct3D11
{
    using System;
    using SharpGen.Runtime;

    public partial class ID3D11On12Device
    {
        public ID3D11Resource CreateWrappedResource(IUnknown d3d12Resource, ResourceFlags flags, int inState, int outState)
        {
            return CreateWrappedResource(d3d12Resource, flags, inState, outState, typeof(ID3D11Resource).GUID);
        }

        public void AcquireWrappedResources(params ID3D11Resource[] resources)
        {
            AcquireWrappedResources_(resources, resources.Length);
        }

        public void AcquireWrappedResources(ID3D11Resource[] resources, int count)
        {
            AcquireWrappedResources_(resources, count);
        }

        public void ReleaseWrappedResources(params ID3D11Resource[] resources)
        {
            ReleaseWrappedResources_(resources, resources.Length);
        }

        public void ReleaseWrappedResources(ID3D11Resource[] resources, int count)
        {
            ReleaseWrappedResources_(resources, count);
        }
    }
}
