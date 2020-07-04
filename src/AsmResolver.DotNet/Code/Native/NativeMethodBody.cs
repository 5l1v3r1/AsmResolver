using System.Collections.Generic;
using AsmResolver.PE;

namespace AsmResolver.DotNet.Code.Native
{
    /// <summary>
    /// Represents a method body of a method defined in a .NET assembly, implemented using native assembler code.
    /// </summary>
    public class NativeMethodBody :  IMethodBody
    {
        /// <summary>
        /// Creates a new empty native method body.
        /// </summary>
        public NativeMethodBody()
        {
        }

        /// <summary>
        /// Creates a new native method body with the provided raw native code stream.
        /// </summary>
        /// <param name="nativeCode">The raw native code stream.</param>
        public NativeMethodBody(byte[] nativeCode)
        {
            NativeCode = nativeCode;
        }

        /// <summary>
        /// Gets or sets the native code (or data) stream to be executed.
        /// </summary>
        public byte[] NativeCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of symbols that are required by the native code.
        /// </summary>
        public IList<ImportAddressFixup> ImportAddressFixups
        {
            get;
        } = new List<ImportAddressFixup>();
    }
}