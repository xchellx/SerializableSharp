using System.Diagnostics;

namespace SerializableSharp
{
    [DebuggerDisplay("{Comment}")]
    public sealed class SerializationLabel(string comment)
    {
        public string Comment => comment;
    }
}
