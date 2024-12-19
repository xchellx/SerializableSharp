/*
 * MIT License
 * 
 * Copyright (c) 2024 Yonder
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Diagnostics.CodeAnalysis;
using System.IO;
using static SerializableSharp.ISerializable;

namespace SerializableSharp
{
    public abstract class Serializable : ISerializable
    {
        public Serializable(Serializable? parent = null)
        {
            Parent = parent;
        }

        private ISerializable? parent = null;

        [AllowNull]
        public ISerializable Parent
        {
            get => parent ?? this;
            set => parent = value != this ? value : null;
        }

        public virtual ISerializationRequirements Requirements => SerializationRequirements.Default;

        public virtual IExtraData ExtraData => NullExtraData.Instance;

        protected abstract ISerializable? ReadImpl(Stream stream, BinaryReader? reusableReader, bool unfixedLen);

        public ISerializable? Read(Stream stream, BinaryReader? reusableReader = null, bool unfixedLen = false)
        {
            Requirements.PreValidateRead(this, stream, reusableReader, unfixedLen);
            ISerializable? versionSpec = ReadImpl(stream, reusableReader, unfixedLen); ;
            Requirements.PostValidateRead(this, stream, reusableReader, unfixedLen, versionSpec);
            return versionSpec;
        }

        protected abstract void WriteImpl(Stream stream, BinaryWriter? reusableWriter, ISerializable? versionSpec,
            bool unfixedLen);

        public void Write(Stream stream, BinaryWriter? reusableWriter = null, ISerializable? versionSpec = null,
            bool unfixedLen = false)
        {
            Requirements.PreValidateWrite(this, stream, reusableWriter, versionSpec, unfixedLen);
            WriteImpl(stream, reusableWriter, versionSpec, unfixedLen);
            Requirements.PostValidateWrite(this, stream, reusableWriter, versionSpec, unfixedLen);
        }

        protected abstract object GetMagicImpl(Stream stream, BinaryReader? reusableReader, bool unfixedLen);

        public bool CheckMagic(CheckMagicDel checker, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen = false)
        {
            Requirements.PreValidateCheckMagic(this, stream, reusableReader, unfixedLen);
            bool success = checker(GetMagicImpl(stream, reusableReader, unfixedLen));
            Requirements.PostValidateCheckMagic(this, stream, reusableReader, unfixedLen);
            return success;
        }
    }
}
