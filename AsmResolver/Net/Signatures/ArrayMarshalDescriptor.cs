﻿using AsmResolver.Net.Emit;

namespace AsmResolver.Net.Signatures
{
    public class ArrayMarshalDescriptor : MarshalDescriptor
    {
        public static ArrayMarshalDescriptor FromReader(IBinaryStreamReader reader)
        {
            var descriptor = new ArrayMarshalDescriptor((NativeType) reader.ReadByte());

            if (!reader.TryReadCompressedUInt32(out uint value))
                return descriptor;
            descriptor.ParameterIndex = (int) value;
            
            if (!reader.TryReadCompressedUInt32(out value))
                return descriptor;
            descriptor.NumberOfElements = (int) value;
            
            return descriptor;
        }

        public ArrayMarshalDescriptor(NativeType elementType)
        {
            ElementType = elementType;
        }

        public override NativeType NativeType => NativeType.Array;

        public NativeType ElementType
        {
            get;
            set;
        }

        public int? ParameterIndex
        {
            get;
            set;
        }

        public int? NumberOfElements
        {
            get;
            set;
        }

        public override uint GetPhysicalLength(MetadataBuffer buffer)
        {
            return 2 * sizeof (byte) +
                   (ParameterIndex.HasValue
                       ? ParameterIndex.Value.GetCompressedSize() +
                         (NumberOfElements.HasValue ? NumberOfElements.Value.GetCompressedSize() : 0)
                       : 0)
                + base.GetPhysicalLength(buffer);
        }

        public override void Prepare(MetadataBuffer buffer)
        {
        }

        public override void Write(MetadataBuffer buffer, IBinaryStreamWriter writer)
        {
            writer.WriteByte((byte)NativeType);
            writer.WriteByte((byte)ElementType);
            if (ParameterIndex.HasValue)
            {
                writer.WriteCompressedUInt32((uint)ParameterIndex.Value);
                if (NumberOfElements.HasValue)
                    writer.WriteCompressedUInt32((uint)NumberOfElements.Value);
            }

            base.Write(buffer, writer);
        }
    }
}