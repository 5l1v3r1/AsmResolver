using System;
using System.Threading;
using AsmResolver.PE.DotNet.Metadata;
using AsmResolver.PE.DotNet.Metadata.Tables;
using AsmResolver.PE.DotNet.Metadata.Tables.Rows;

namespace AsmResolver.DotNet.Serialized
{
    internal class CachedSerializedMemberFactory
    {
        private readonly IMetadata _metadata;
        private readonly SerializedModuleDefinition _parentModule;

        private TypeReference[] _typeReferences;
        private TypeDefinition[] _typeDefinitions;
        private FieldDefinition[] _fieldDefinitions;
        private MethodDefinition[] _methodDefinitions;
        private ParameterDefinition[] _parameterDefinitions;
        private MemberReference[] _memberReferences;
        private StandAloneSignature[] _standAloneSignatures;
        private PropertyDefinition[] _propertyDefinitions;
        private EventDefinition[] _eventDefinition;
        private MethodSemantics[] _methodSemantics;
        private TypeSpecification[] _typeSpecifications;
        private CustomAttribute[] _customAttributes;
        private MethodSpecification[] _methodSpecifications;
        private GenericParameter[] _genericParameters;
        private GenericParameterConstraint[] _genericParameterConstraints;
        private ModuleReference[] _moduleReferences;
        private FileReference[] _fileReferences;
        private ManifestResource[] _resources;
        private ExportedType[] _exportedTypes;
        private Constant[] _constants;
        private ClassLayout[] _classLayouts;
        private ImplementationMap[] _implementationMaps;
        private InterfaceImplementation[] _interfaceImplementations;
        private SecurityDeclaration[] _securityDeclarations;

        internal CachedSerializedMemberFactory(IMetadata metadata, SerializedModuleDefinition parentModule)
        {
            _metadata = metadata;
            _parentModule = parentModule;
        }

        internal bool TryLookupMember(MetadataToken token, out IMetadataMember member)
        {
            member = token.Table switch
            {
                TableIndex.Module => LookupModuleDefinition(token),
                TableIndex.TypeRef => LookupTypeReference(token),
                TableIndex.TypeDef => LookupTypeDefinition(token),
                TableIndex.TypeSpec => LookupTypeSpecification(token),
                TableIndex.Assembly => LookupAssemblyDefinition(token),
                TableIndex.AssemblyRef => LookupAssemblyReference(token),
                TableIndex.Field => LookupFieldDefinition(token),
                TableIndex.Method => LookupMethodDefinition(token),
                TableIndex.Param => LookupParameterDefinition(token),
                TableIndex.MemberRef => LookupMemberReference(token),
                TableIndex.StandAloneSig => LookupStandAloneSignature(token),
                TableIndex.Property => LookupPropertyDefinition(token),
                TableIndex.Event => LookupEventDefinition(token),
                TableIndex.MethodSemantics => LookupMethodSemantics(token),
                TableIndex.CustomAttribute => LookupCustomAttribute(token),
                TableIndex.MethodSpec => LookupMethodSpecification(token),
                TableIndex.GenericParam => LookupGenericParameter(token),
                TableIndex.GenericParamConstraint => LookupGenericParameterConstraint(token),
                TableIndex.ModuleRef => LookupModuleReference(token),
                TableIndex.File => LookupFileReference(token),
                TableIndex.ManifestResource => LookupManifestResource(token),
                TableIndex.ExportedType => LookupExportedType(token),
                TableIndex.Constant => LookupConstant(token),
                TableIndex.ClassLayout => LookupClassLayout(token),
                TableIndex.ImplMap => LookupImplementationMap(token),
                TableIndex.InterfaceImpl => LookupInterfaceImplementation(token),
                TableIndex.DeclSecurity => LookupSecurityDeclaration(token), 
                _ => null
            };

            return member != null;
        }

        private IMetadataMember LookupModuleDefinition(in MetadataToken token)
        {
            return token.Rid == 1
                ? _parentModule
                : null; // TODO: handle spurious assembly definition rows.
        }

        internal TypeReference LookupTypeReference(MetadataToken token)
        {
            return LookupOrCreateMember<TypeReference, TypeReferenceRow>(ref _typeReferences, token,
                (m, t, r) => new SerializedTypeReference(m, t, r));
        }

        internal TypeDefinition LookupTypeDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<TypeDefinition, TypeDefinitionRow>(ref _typeDefinitions, token,
                (m, t, r) => new SerializedTypeDefinition(m, t, r));
        }

        internal TypeSpecification LookupTypeSpecification(MetadataToken token)
        {
            return LookupOrCreateMember<TypeSpecification, TypeSpecificationRow>(ref _typeSpecifications, token,
                (m, t, r) => new SerializedTypeSpecification(m, t, r));
        }

        private AssemblyDefinition LookupAssemblyDefinition(MetadataToken token)
        {
            return token.Rid == 1
                ? _parentModule.Assembly
                : null; // TODO: handle spurious assembly definition rows.
        }

        internal IMetadataMember LookupAssemblyReference(MetadataToken token)
        {
            return token.Rid != 0 && token.Rid <= _parentModule.AssemblyReferences.Count
                ? _parentModule.AssemblyReferences[(int) (token.Rid - 1)]                : null;
        }

        private FieldDefinition LookupFieldDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<FieldDefinition, FieldDefinitionRow>(ref _fieldDefinitions, token,
                (m, t, r) => new SerializedFieldDefinition(m, t, r));
        }

        private MethodDefinition LookupMethodDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<MethodDefinition, MethodDefinitionRow>(ref _methodDefinitions, token,
                (m, t, r) => new SerializedMethodDefinition(m, t, r));
        }

        private ParameterDefinition LookupParameterDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<ParameterDefinition, ParameterDefinitionRow>(ref _parameterDefinitions, token,
                (m, t, r) => new SerializedParameterDefinition(m, t, r));
        }

        private MemberReference LookupMemberReference(MetadataToken token)
        {
            return LookupOrCreateMember<MemberReference, MemberReferenceRow>(ref _memberReferences, token,
                (m, t, r) => new SerializedMemberReference(m, t, r));
        }

        private StandAloneSignature LookupStandAloneSignature(MetadataToken token)
        {
            return LookupOrCreateMember<StandAloneSignature, StandAloneSignatureRow>(ref _standAloneSignatures, token,
                (m, t, r) => new SerializedStandAloneSignature(m, t, r));
        }

        private PropertyDefinition LookupPropertyDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<PropertyDefinition, PropertyDefinitionRow>(ref _propertyDefinitions, token,
                (m, t, r) => new SerializedPropertyDefinition(m, t, r));
        }

        private EventDefinition LookupEventDefinition(MetadataToken token)
        {
            return LookupOrCreateMember<EventDefinition, EventDefinitionRow>(ref _eventDefinition, token,
                (m, t, r) => new SerializedEventDefinition(m, t, r));
        }

        private MethodSemantics LookupMethodSemantics(MetadataToken token)
        {
            return LookupOrCreateMember<MethodSemantics, MethodSemanticsRow>(ref _methodSemantics, token,
                (m, t, r) => new SerializedMethodSemantics(m, t, r));
        }

        private CustomAttribute LookupCustomAttribute(MetadataToken token)
        {
            return LookupOrCreateMember<CustomAttribute, CustomAttributeRow>(ref _customAttributes, token,
                (m, t, r) => new SerializedCustomAttribute(m, t, r));
        }

        private IMetadataMember LookupMethodSpecification(MetadataToken token)
        {
            return LookupOrCreateMember<MethodSpecification, MethodSpecificationRow>(ref _methodSpecifications, token,
                (m, t, r) => new SerializedMethodSpecification(m, t, r));
        }

        private GenericParameter LookupGenericParameter(MetadataToken token)
        {
            return LookupOrCreateMember<GenericParameter, GenericParameterRow>(ref _genericParameters, token,
                (m, t, r) => new SerializedGenericParameter(m, t, r));
        }

        private GenericParameterConstraint LookupGenericParameterConstraint(in MetadataToken token)
        {
            return LookupOrCreateMember<GenericParameterConstraint, GenericParameterConstraintRow>(ref _genericParameterConstraints, token,
                (m, t, r) => new SerializedGenericParameterConstraint(m, t, r));
        }

        private ModuleReference LookupModuleReference(MetadataToken token)
        {
            return LookupOrCreateMember<ModuleReference, ModuleReferenceRow>(ref _moduleReferences, token,
                (m, t, r) => new SerializedModuleReference(m, t, r));
        }

        private FileReference LookupFileReference(MetadataToken token)
        {
            return LookupOrCreateMember<FileReference, FileReferenceRow>(ref _fileReferences, token,
                (m, t, r) => new SerializedFileReference(m, t, r));
        }

        private ManifestResource LookupManifestResource(MetadataToken token)
        {
            return LookupOrCreateMember<ManifestResource, ManifestResourceRow>(ref _resources, token,
                (m, t, r) => new SerializedManifestResource(m, t, r));
        }

        private ExportedType LookupExportedType(MetadataToken token)
        {
            return LookupOrCreateMember<ExportedType, ExportedTypeRow>(ref _exportedTypes, token,
                (m, t, r) => new SerializedExportedType(m, t, r));
        }

        private Constant LookupConstant(MetadataToken token)
        {
            return LookupOrCreateMember<Constant, ConstantRow>(ref _constants, token,
                (m, t, r) => new SerializedConstant(m, t, r));
        }

        private ClassLayout LookupClassLayout(MetadataToken token)
        {
            return LookupOrCreateMember<ClassLayout, ClassLayoutRow>(ref _classLayouts, token,
                (m, t, r) => new SerializedClassLayout(m, t, r));
        }

        internal ImplementationMap LookupImplementationMap(MetadataToken token)
        {
            return LookupOrCreateMember<ImplementationMap, ImplementationMapRow>(ref _implementationMaps, token,
                (m, t, r) => new SerializedImplementationMap(m, t, r));
        }

        private InterfaceImplementation LookupInterfaceImplementation(MetadataToken token)
        {
            return LookupOrCreateMember<InterfaceImplementation, InterfaceImplementationRow>(ref _interfaceImplementations, token,
                (m, t, r) => new SerializedInterfaceImplementation(m, t, r));
        }

        private SecurityDeclaration LookupSecurityDeclaration(MetadataToken token)
        {
            return LookupOrCreateMember<SecurityDeclaration, SecurityDeclarationRow>(ref _securityDeclarations, token,
                (m, t, r) => new SerializedSecurityDeclaration(m, t, r));
        }

        internal TMember LookupOrCreateMember<TMember, TRow>(ref TMember[] cache, MetadataToken token,
            Func<SerializedModuleDefinition, MetadataToken, TRow, TMember> createMember)
            where TRow : struct, IMetadataRow
            where TMember : class, IMetadataMember
        {
            // Obtain table.
            var table = (MetadataTable<TRow>) _metadata
                .GetStream<TablesStream>()
                .GetTable(token.Table);

            // Check if within bounds.
            if (token.Rid == 0 || token.Rid > table.Count)
                return null;

            // Allocate cache if necessary.
            if (cache is null)
                Interlocked.CompareExchange(ref cache, new TMember[table.Count], null);

            // Get or create cached member.
            int index = (int) token.Rid - 1;
            var member = cache[index];
            if (member is null)
            {
                member = createMember(_parentModule, token, table[index]);
                member = Interlocked.CompareExchange(ref cache[index], member, null)
                         ?? member;
            }

            return member;
        }
    }
}