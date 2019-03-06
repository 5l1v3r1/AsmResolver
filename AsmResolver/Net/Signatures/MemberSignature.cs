﻿namespace AsmResolver.Net.Signatures
{
    public abstract class MemberSignature : CallingConventionSignature, IHasTypeSignature
    {
        protected abstract TypeSignature TypeSignature
        {
            get;
        }

        TypeSignature IHasTypeSignature.TypeSignature => TypeSignature;

        public override string ToString()
        {
            return (HasThis ? "instance " : string.Empty) + TypeSignature.FullName;
        }
    }
}
