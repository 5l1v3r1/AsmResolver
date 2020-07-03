using AsmResolver.PE.Imports;

namespace AsmResolver.PE
{
    /// <summary>
    /// Represents a single import address fixup in a native code stream.
    /// </summary>
    public readonly struct ImportAddressFixup
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ImportAddressFixup"/> structure.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="offset">The relative offset to write the pointer at.</param>
        public ImportAddressFixup(ImportedSymbol symbol, uint offset)
        {
            Symbol = symbol;
            Offset = offset;
        }

        /// <summary>
        /// Gets the pointer that needs to be written.
        /// </summary>
        public ImportedSymbol Symbol
        {
            get;
        }

        /// <summary>
        /// Gets the relative offset to write the pointer at.
        /// </summary>
        public uint Offset
        {
            get;
        }
    }
}