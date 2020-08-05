﻿namespace beholder_eye_win.Direct3D11.Shader
{
    using beholder_eye_mathematics;

    public partial class ID3D11ShaderReflection
    {
        private ShaderParameterDescription[] _inputParameters;
        private ShaderParameterDescription[] _outputParameters;
        private ID3D11ShaderReflectionConstantBuffer[] _constantBuffers;
        private InputBindingDescription[] _resources;

        public Int3 ThreadGroupSize
        {
            get
            {
                GetThreadGroupSize(out var x, out var y, out var z);
                return new Int3(x, y, z);
            }
        }

        public ShaderParameterDescription[] InputParameters
        {
            get
            {
                if (_inputParameters == null)
                {
                    _inputParameters = new ShaderParameterDescription[Description.InputParameters];
                    for (var i = 0; i < Description.InputParameters; i++)
                    {
                        _inputParameters[i] = GetInputParameterDescription(i);
                    }
                }

                return _inputParameters;
            }
        }

        public ShaderParameterDescription[] OutputParameters
        {
            get
            {
                if (_outputParameters == null)
                {
                    _outputParameters = new ShaderParameterDescription[Description.OutputParameters];
                    for (var i = 0; i < Description.OutputParameters; i++)
                    {
                        _outputParameters[i] = GetOutputParameterDescription(i);
                    }
                }

                return _outputParameters;
            }
        }

        public ID3D11ShaderReflectionConstantBuffer[] ConstantBuffers
        {
            get
            {
                if (_constantBuffers == null)
                {
                    _constantBuffers = new ID3D11ShaderReflectionConstantBuffer[Description.ConstantBuffers];
                    for (var i = 0; i < Description.ConstantBuffers; i++)
                    {
                        _constantBuffers[i] = GetConstantBufferByIndex(i);
                    }
                }

                return _constantBuffers;
            }
        }

        public InputBindingDescription[] Resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = new InputBindingDescription[Description.BoundResources];
                    for (var i = 0; i < Description.BoundResources; i++)
                    {
                        _resources[i] = GetResourceBindingDescription(i);
                    }
                }

                return _resources;
            }
        }

        public int GetThreadGroupSize(out Int3 size)
        {
            var totalSize = GetThreadGroupSize(out var x, out var y, out var z);
            size = new Int3(x, y, z);
            return totalSize;
        }
    }
}
