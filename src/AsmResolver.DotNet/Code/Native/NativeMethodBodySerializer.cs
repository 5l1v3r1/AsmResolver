using AsmResolver.DotNet.Code.Cil;
using AsmResolver.PE;

namespace AsmResolver.DotNet.Code.Native
{
    /// <summary>
    /// Provides a default implementation of the <see cref="IMethodBodySerializer"/> interface, that serializes all
    /// native method bodies of type <see cref="NativeMethodBody"/> to raw method bodies of type <see cref="NativeCodeSegment"/>.
    /// </summary>
    public class NativeMethodBodySerializer : IMethodBodySerializer
    {
        /// <inheritdoc />
        public ISegmentReference SerializeMethodBody(
            INativeSymbolsProvider symbolsProvider, IMetadataTokenProvider provider, MethodDefinition method)
        {
            if (!(method.MethodBody is NativeMethodBody nativeMethodBody))
                return SegmentReference.Null;
            
            var segment = new NativeCodeSegment(symbolsProvider.ImageBase, nativeMethodBody.NativeCode);

            foreach (var fixup in nativeMethodBody.ImportAddressFixups)
            {
                var symbol = symbolsProvider.ImportSymbol(fixup.Symbol);
                segment.ImportAddressFixups.Add(new ImportAddressFixup(symbol, fixup.Offset));
            }

            return new SegmentReference(segment);
        }
    }
}