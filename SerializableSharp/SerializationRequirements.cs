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

using System;
using System.IO;
using UtilSharp.Extensions;

namespace SerializableSharp
{
    public class SerializationRequirements(
        bool readNeedsSeek = false,
        bool writeNeedsSeek = false,
        Func<BinaryReader?, bool>? validateReusableReader = null,
        Func<BinaryWriter?, bool>? validateReusableWriter = null,
        bool readSupportsUnfixedLen = false,
        bool writeSupportsUnfixedLen = false,
        Func<ISerializable?, bool>? validateVersionSpec = null,
        Func<ISerializable, bool>? validateReadParent = null,
        Func<ISerializable, bool>? validateWriteParent = null,
        Func<IExtraData, bool>? preValidateReadExtraData = null,
        Func<IExtraData, bool>? postValidateReadExtraData = null,
        Func<IExtraData, bool>? preValidateWriteExtraData = null,
        Func<IExtraData, bool>? postValidateWriteExtraData = null,
        Func<IExtraData, bool>? preValidateReadParentExtraData = null,
        Func<IExtraData, bool>? postValidateReadParentExtraData = null,
        Func<IExtraData, bool>? preValidateWriteParentExtraData = null,
        Func<IExtraData, bool>? postValidateWriteParentExtraData = null
    ) : ISerializationRequirements
    {
        public bool ReadNeedsSeek => readNeedsSeek;

        public bool WriteNeedsSeek => writeNeedsSeek;

        public bool ReadSupportsUnfixedLen => readSupportsUnfixedLen;

        public bool WriteSupportsUnfixedLen => writeSupportsUnfixedLen;

        public Func<ISerializable, bool>? ValidateReadParent => validateReadParent;

        public Func<ISerializable, bool>? ValidateWriteParent => validateWriteParent;

        public Func<BinaryReader?, bool>? ValidateReusableReader => validateReusableReader;

        public Func<BinaryWriter?, bool>? ValidateReusableWriter => validateReusableWriter;

        public Func<ISerializable?, bool>? ValidateVersionSpec => validateVersionSpec;

        public Func<IExtraData, bool>? PreValidateReadExtraData = preValidateReadExtraData;

        public Func<IExtraData, bool>? PostValidateReadExtraData = postValidateReadExtraData;

        public Func<IExtraData, bool>? PreValidateWriteExtraData = preValidateWriteExtraData;

        public Func<IExtraData, bool>? PostValidateWriteExtraData = postValidateWriteExtraData;

        public Func<IExtraData, bool>? PreValidateReadParentExtraData = preValidateReadParentExtraData;

        public Func<IExtraData, bool>? PostValidateReadParentExtraData = postValidateReadParentExtraData;

        public Func<IExtraData, bool>? PreValidateWriteParentExtraData = preValidateWriteParentExtraData;

        public Func<IExtraData, bool>? PostValidateWriteParentExtraData = postValidateWriteParentExtraData;

        public static ISerializationRequirements Default => DefaultInstance.Value;

        public virtual void PreValidateRead(ISerializable serializer, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen)
        {
            if (stream == null)
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentNullException(nameof(stream)).PopulateException());
            else if (!stream.CanRead)
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input reader cannot read", nameof(stream)).PopulateException());
            else if (ReadNeedsSeek && !stream.CanSeek)
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input reader cannot seek", nameof(stream)).PopulateException());
            else if (reusableReader != null && reusableReader.BaseStream != stream)
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input reusable reader does not wrap the input stream.", nameof(serializer)).PopulateException());
            else if (!(ValidateReusableReader?.Invoke(reusableReader) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input serializer failed to validate its ruesable reader", nameof(serializer)).PopulateException());
            else if (!ReadSupportsUnfixedLen && unfixedLen)
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input serializer does not support indeterminate positional binary data", nameof(serializer)).PopulateException());
            else if (!(ValidateReadParent?.Invoke(serializer.Parent) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input serializer failed to validate its parent", nameof(serializer)).PopulateException());
            else if (!(PreValidateReadExtraData?.Invoke(serializer.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input serializer failed to validate its extra data", nameof(serializer)).PopulateException());
            else if (!(PreValidateReadParentExtraData?.Invoke(serializer.Parent.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre read validation failed", new ArgumentException("The input serializer failed to validate its parent's extra data", nameof(serializer)).PopulateException());
        }

        public virtual void PostValidateRead(ISerializable serializer, Stream stream, BinaryReader? reusableReader,
            bool unfixedLen, ISerializable? versionSpec)
        {
            if (!(ValidateVersionSpec?.Invoke(versionSpec) ?? true))
                throw new SerializationException(serializer.GetType(), "Post read validation failed", new ArgumentException("The input serializer failed to validate its version specific serializer", nameof(serializer)).PopulateException());
            else if (!(PostValidateReadExtraData?.Invoke(serializer.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Post read validation failed", new ArgumentException("The input serializer failed to validate its extra data", nameof(serializer)).PopulateException());
            else if (!(PostValidateReadParentExtraData?.Invoke(serializer.Parent.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Post read validation failed", new ArgumentException("The input serializer failed to validate its parent's extra data", nameof(serializer)).PopulateException());
        }

        public virtual void PreValidateWrite(ISerializable serializer, Stream stream, BinaryWriter? reusableWriter,
            ISerializable? versionSpec, bool unfixedLen)
        {
            if (stream == null)
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentNullException(nameof(stream)).PopulateException(), true);
            else if (!stream.CanWrite)
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input writer cannot write", nameof(stream)).PopulateException(), true);
            else if (WriteNeedsSeek && !stream.CanSeek)
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input writer cannot seek", nameof(stream)).PopulateException(), true);
            else if (reusableWriter != null && reusableWriter.BaseStream != stream)
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input reusable writer does not wrap the input stream.", nameof(serializer)).PopulateException(), true);
            else if (!(ValidateReusableWriter?.Invoke(reusableWriter) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its ruesable writer", nameof(serializer)).PopulateException(), true);
            else if (!WriteSupportsUnfixedLen && unfixedLen)
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer does not support indeterminate positional binary data", nameof(serializer)).PopulateException(), true);
            else if (!(ValidateVersionSpec?.Invoke(versionSpec) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its version specific serializer", nameof(serializer)).PopulateException(), true);
            else if (!(ValidateWriteParent?.Invoke(serializer.Parent) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its parent", nameof(serializer)).PopulateException(), true);
            else if (!(PreValidateWriteExtraData?.Invoke(serializer.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its extra data", nameof(serializer)).PopulateException(), true);
            else if (!(PreValidateWriteParentExtraData?.Invoke(serializer.Parent.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its parent's extra data", nameof(serializer)).PopulateException(), true);
        }

        public virtual void PostValidateWrite(ISerializable serializer, Stream stream, BinaryWriter? reusableWriter,
            ISerializable? versionSpec, bool unfixedLen)
        {
            if (!(PostValidateWriteExtraData?.Invoke(serializer.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its extra data", nameof(serializer)).PopulateException(), true);
            else if (!(PostValidateWriteParentExtraData?.Invoke(serializer.Parent.ExtraData) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre write validation failed", new ArgumentException("The input serializer failed to validate its parent's extra data", nameof(serializer)).PopulateException(), true);
        }

        public virtual void PreValidateCheckMagic(ISerializable serializer, Stream stream,
            BinaryReader? reusableReader, bool unfixedLen)
        {
            if (stream == null)
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentNullException(nameof(stream)).PopulateException());
            else if (!stream.CanRead)
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentException("The input reader cannot read", nameof(stream)).PopulateException());
            else if (ReadNeedsSeek && !stream.CanSeek)
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentException("The input reader cannot seek", nameof(stream)).PopulateException());
            else if (reusableReader != null && reusableReader.BaseStream != stream)
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentException("The input reusable reader does not wrap the input stream.", nameof(serializer)).PopulateException());
            else if (!(ValidateReusableReader?.Invoke(reusableReader) ?? true))
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentException("The input serializer failed to validate its ruesable reader", nameof(serializer)).PopulateException());
            else if (!ReadSupportsUnfixedLen && unfixedLen)
                throw new SerializationException(serializer.GetType(), "Pre check magic validation failed", new ArgumentException("The input serializer does not support indeterminate positional binary data", nameof(serializer)).PopulateException());
        }

        public virtual void PostValidateCheckMagic(ISerializable serializer, Stream stream,
            BinaryReader? reusableReader, bool unfixedLen)
        {
        }

        private static class DefaultInstance
        {
            internal static readonly SerializationRequirements Value = new();
        }
    }
}
