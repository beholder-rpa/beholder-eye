namespace beholder_eye_win.Direct3D
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct ShaderMacro : IEquatable<ShaderMacro>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderMacro"/> struct. 
        /// </summary>
        /// <param name="name">The macro name.</param>
        /// <param name="definition">The macro definition.</param>
        public ShaderMacro(string name, object definition)
        {
            Name = name;
            Definition = definition?.ToString();
        }

        public bool Equals(ShaderMacro other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Definition, other.Definition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ShaderMacro shaderMacro && Equals(shaderMacro);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 0;
                if (!string.IsNullOrEmpty(Name))
                {
                    hashCode = Name.GetHashCode() * 397;
                }

                if (!string.IsNullOrEmpty(Definition))
                {
                    hashCode ^= Definition.GetHashCode();
                }

                return hashCode;
            }
        }

        public static bool operator ==(ShaderMacro left, ShaderMacro right) => left.Equals(right);

        public static bool operator !=(ShaderMacro left, ShaderMacro right) => !left.Equals(right);

        #region Marshal
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct __Native
        {
            public IntPtr Name;

            public IntPtr Definition;
        }

        internal void __MarshalTo(ref __Native @ref)
        {
            @ref.Name = Marshal.StringToHGlobalAnsi(Name);
            @ref.Definition = Marshal.StringToHGlobalAnsi(Definition);
        }

        internal void __MarshalFree(ref __Native @ref)
        {
            Marshal.FreeHGlobal(@ref.Name);
            Marshal.FreeHGlobal(@ref.Definition);
        }
        #endregion Marshal
    }
}
