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

namespace SerializableSharp
{
    public class SerializationException : Exception
    {
        public Type SerializerType { get; }

        public bool IsWrite { get; } = false;

        public SerializationException(Type serializerType) : base(GetMessage(serializerType))
        {
            SerializerType = serializerType;
        }

        public SerializationException(Type serializerType, bool isWrite)
            : base(GetMessage(serializerType, isWrite: isWrite))
        {
            SerializerType = serializerType;
            IsWrite = isWrite;
        }

        public SerializationException(Type serializerType, string? message)
            : base(GetMessage(serializerType, message: message))
        {
            SerializerType = serializerType;
        }

        public SerializationException(Type serializerType, string? message, bool isWrite)
            : base(GetMessage(serializerType, message: message))
        {
            SerializerType = serializerType;
            IsWrite = isWrite;
        }

        public SerializationException(Type serializerType, string? message, Exception? innerException)
            : base(GetMessage(serializerType, message: message), innerException)
        {
            SerializerType = serializerType;
        }

        public SerializationException(Type serializerType, string? message, Exception? innerException, bool isWrite)
            : base(GetMessage(serializerType, message: message, isWrite: isWrite), innerException)
        {
            SerializerType = serializerType;
            IsWrite = isWrite;
        }

        private static string GetMessage(Type serializerType, string? message = null, bool isWrite = false)
            => $"Failed to {(isWrite ? "serialize" : "deserialize")} type \"{serializerType.Name}\"{(message?.Length > 0 ? ": " : "")}{message ?? string.Empty}";
    }
}
