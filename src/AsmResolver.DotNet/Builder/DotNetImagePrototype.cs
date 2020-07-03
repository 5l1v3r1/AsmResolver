using System.Collections.Generic;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.Imports;
using AsmResolver.PE.Relocations;

namespace AsmResolver.DotNet.Builder
{
    /// <summary>
    /// Provides a prototype for a .NET PE image, containing a .NET data directory, as well as any extra information
    /// that needs to be added to the final PE image.
    /// </summary>
    public class DotNetImagePrototype
    {
        private readonly IDictionary<string, ImportedModule> _imports = new Dictionary<string, ImportedModule>();
        private readonly IList<BaseRelocation> _relocations = new List<BaseRelocation>();
        
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

        /// <summary>
        /// Adds a single symbol to the prototype.
        /// </summary>
        /// <param name="symbol">The symbol to import.</param>
        /// <returns>The imported symbol.</returns>
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

        /// <summary>
        /// Adds a base relocation to the prototype.
        /// </summary>
        /// <param name="relocation">The relocation.</param>
        public void AddBaseRelocation(BaseRelocation relocation) => _relocations.Add(relocation);
    }
}