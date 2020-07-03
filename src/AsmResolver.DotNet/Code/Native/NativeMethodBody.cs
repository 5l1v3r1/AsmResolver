using System.Collections.Generic;
using AsmResolver.PE.Imports;
using AsmResolver.PE.Relocations;

namespace AsmResolver.DotNet.Code.Native
{
    /// <summary>
    /// Represents a method body of a method defined in a .NET assembly, implemented using native assembler code.
    /// </summary>
    public class NativeMethodBody : MethodBody
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
            : this(new DataSegment(nativeCode))
        {
        }

        /// <summary>
        /// Creates a new native method body with the provided native code segment.
        /// </summary>
        /// <param name="nativeCode">The segment containing the native code stream.</param>
        public NativeMethodBody(ISegment nativeCode)
        {
            NativeCode = nativeCode;
        }
        
        /// <summary>
        /// Gets or sets the native code (or data) stream to be executed.
        /// </summary>
        public ISegment NativeCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of symbols that are required by the native code.
        /// </summary>
        public IList<ImportedSymbol> ImportedSymbols
        {
            get;
        } = new List<ImportedSymbol>();

        /// <summary>
        /// Gets a collection of relocations that the native code requires to be applied at runtime.
        /// </summary>
        public IList<BaseRelocation> Relocations
        {
            get;
        } = new List<BaseRelocation>();
    }
}