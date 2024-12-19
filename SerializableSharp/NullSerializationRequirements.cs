using System.IO;

namespace SerializableSharp
{
    public class NullSerializationRequirements : ISerializationRequirements
    {
        private NullSerializationRequirements()
        {
        }

        public static ISerializationRequirements Instance => EmptyInstance.Value;
        
        public void PreValidateRead(ISerializable serializer, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen)
        {
        }

        public void PostValidateRead(ISerializable serializer, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen, ISerializable? versionSpec)
        {
        }

        public void PreValidateWrite(ISerializable serializer, Stream stream, BinaryWriter? reusableWriter,
            ISerializable? versionSpec, bool unfixedLen)
        {
        }

        public void PostValidateWrite(ISerializable serializer, Stream stream, BinaryWriter? reusableWriter,
            ISerializable? versionSpec, bool unfixedLen)
        {
        }

        public void PreValidateCheckMagic(ISerializable serializer, Stream stream, BinaryReader? reusableReader, bool unfixedLen)
        {
        }

        public void PostValidateCheckMagic(ISerializable serializer, Stream stream, BinaryReader? reusableReader, bool unfixedLen)
        {
        }

        private static class EmptyInstance
        {
            internal static readonly NullSerializationRequirements Value = new();
        }
    }
}
