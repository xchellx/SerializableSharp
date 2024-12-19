using System.IO;

namespace SerializableSharp
{
    public class NullSerializable : ISerializable
    {
        private NullSerializable()
        {
        }

        public static ISerializable Instance => EmptyInstance.Value;

        public ISerializable Parent => this;

        public ISerializationRequirements Requirements => NullSerializationRequirements.Instance;

        public IExtraData ExtraData => NullExtraData.Instance;

        public ISerializable? Read(Stream stream, BinaryReader? reusableReader, bool unfixedLen = false) => null;

        public void Write(Stream stream, BinaryWriter? reusableWriter, ISerializable? versionSpec = null,
            bool unfixedLen = false)
        {
        }

        public bool CheckMagic(ISerializable.CheckMagicDel checker, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen = false) => true;

        private static class EmptyInstance
        {
            internal static readonly NullSerializable Value = new();
        }
    }
}
