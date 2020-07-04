using System.Collections.Generic;
using AsmResolver.DotNet.Code;
using AsmResolver.DotNet.Code.Native;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.Imports;
using AsmResolver.PE.Relocations;

namespace AsmResolver.DotNet.Builder
{
    /// <summary>
    /// Provides a prototype for a .NET PE image, containing a .NET data directory, as well as any extra information
    /// that needs to be added to the final PE image.
    /// </summary>
    public class DotNetImagePrototype : INativeSymbolsProvider
    {
        private readonly IDictionary<string, ImportedModule> _imports = new Dictionary<string, ImportedModule>();
        private readonly IList<BaseRelocation> _relocations = new List<BaseRelocation>();

        /// <summary>
        /// Creates a new instance of the <see cref="DotNetImagePrototype"/> class.
        /// </summary>
        /// <param name="isDll">Indicates whether the final PE image is supposed to be a dynamically linked library.</param>
        public DotNetImagePrototype(bool isDll)
        {
            ImageBase = isDll ? 0x10000000u : 0x00400000u;
        }

        /// <inheritdoc />
        public uint ImageBase
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the constructed .NET data directory.
        /// </summary>
        public IDotNetDirectory ConstructedDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of modules that were used by native method bodies referenced in the
        /// constructed .NET data directory.
        /// </summary>
        public IEnumerable<ImportedModule> GetNativeImports() => _imports.Values;

        /// <summary>
        /// Gets a collection of base relocations that need to be applied to native method bodies referenced in the
        /// constructed .NET data directory.
        /// </summary>
        public IEnumerable<BaseRelocation> GetNativeRelocations() => _relocations;

        /// <inheritdoc />
        public ImportedSymbol ImportSymbol(ImportedSymbol symbol)
        {
            if (!_imports.TryGetValue(symbol.DeclaringModule.Name, out var module))
            {
                module = new ImportedModule(symbol.DeclaringModule.Name);
                _imports.Add(module.Name, module);
            }

            var clonedSymbol = symbol.IsImportByName
                ? new ImportedSymbol(symbol.Hint, symbol.Name)
                : new ImportedSymbol(symbol.Ordinal);
            module.Symbols.Add(clonedSymbol);
            return clonedSymbol;
        }

        /// <inheritdoc />
        public void RegisterBaseRelocation(BaseRelocation relocation) => _relocations.Add(relocation);
    }
}