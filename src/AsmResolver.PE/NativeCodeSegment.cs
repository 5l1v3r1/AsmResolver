using System.Collections.Generic;

namespace AsmResolver.PE
{
    /// <summary>
    /// Represents a chunk of native code.
    /// </summary>
    public class NativeCodeSegment : SegmentBase
    {
        /// <summary>
        /// Creates a new native code segment, that is relative to a certain image base.
        /// </summary>
        /// <param name="imageBase">The image base address this native code targets.</param>
        /// <param name="nativeCode">The raw native code stream.</param>
        public NativeCodeSegment(uint imageBase, byte[] nativeCode)
        {
            NativeCode = nativeCode;
            ImageBase = imageBase;
        }

        /// <summary>
        /// Gets or sets the raw native code stream. 
        /// </summary>
        public byte[] NativeCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the image base.
        /// </summary>
        public uint ImageBase
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of pointer fixups that need to be applied to <see cref="NativeCode"/>.
        /// </summary>
        public IList<ImportAddressFixup> ImportAddressFixups
        {
            get;
        } = new List<ImportAddressFixup>();

        /// <inheritdoc />
        public override uint GetPhysicalSize() => (uint) NativeCode.Length;

        /// <inheritdoc />
        public override void Write(IBinaryStreamWriter writer)
        {
            uint startOffset = writer.FileOffset;
            writer.WriteBytes(NativeCode);
            uint endOffset = writer.FileOffset;
            
            for (int i = 0; i < ImportAddressFixups.Count; i++)
            {
                var fixup = ImportAddressFixups[i];
                writer.FileOffset = startOffset + fixup.Offset;
                writer.WriteUInt32(ImageBase + fixup.Symbol.AddressTableEntry.Rva);
            }

            writer.FileOffset = endOffset;
        }
    }
}