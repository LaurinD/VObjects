﻿using System;
using System.Collections.Generic;
using System.IO;

namespace VVVV.Packs.VObjects
{
    public static class ColUtils
    {
        public static void Set<TKey, TVal>(this Dictionary<TKey, TVal> d, TKey k, TVal v)
        {
            if (d.ContainsKey(k))
            {
                d[k] = v;
            }
            else
            {
                d.Add(k, v);
            }
        }
    }
    public static class TypeUtils
    {
        public static IEnumerable<Type> GetTypes(this Type type)
        {
            // is there any base type?
            if ((type == null) || (type.BaseType == null))
            {
                yield break;
            }
            yield return type;
            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        public static string GetName(this Type T, bool full)
        {
            if (full) return T.FullName;
            else return T.Name;
        }
    }

    public static class ObjectHelper
    {
        public static void DisposeDisposable(object obj)
        {
            if (obj is IDisposable)
            {
                var t = obj as IDisposable;
                t.Dispose();
            }
        }

        public static Type ForceGetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }

    public static class StreamHelper
    {
        public static bool ReadBool(this Stream input)
        {
            byte[] tmp = new byte[1];
            input.Read(tmp, 0, 1);
            return BitConverter.ToBoolean(tmp, 0);
        }
        public static uint ReadUint(this Stream input)
        {
            byte[] tmp = new byte[4];
            input.Read(tmp, 0, 4);
            return BitConverter.ToUInt32(tmp, 0);
        }
        public static int ReadInt(this Stream input)
        {
            byte[] tmp = new byte[4];
            input.Read(tmp, 0, 1);
            return BitConverter.ToInt32(tmp, 0);
        }
        public static float ReadFloat(this Stream input)
        {
            byte[] tmp = new byte[4];
            input.Read(tmp, 0, 1);
            return BitConverter.ToSingle(tmp, 0);
        }
        public static double ReadDouble(this Stream input)
        {
            byte[] tmp = new byte[8];
            input.Read(tmp, 0, 1);
            return BitConverter.ToDouble(tmp, 0);
        }

        public static void WriteBool(this Stream input, bool data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteUint(this Stream input, uint data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteInt(this Stream input, int data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteFloat(this Stream input, float data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteDouble(this Stream input, double data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }


        public static string ReadASCII(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.ASCII.GetString(tmp);
        }
        public static string ReadUTF8(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.UTF8.GetString(tmp);
        }
        public static string ReadUnicode(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }

        public static void WriteASCII(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.ASCII.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteUTF8(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteUnicode(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.Unicode.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }

        public static uint ASCIILength(this string s)
        {
            byte[] tmp = System.Text.Encoding.ASCII.GetBytes(s);
            return (uint)tmp.Length;
        }
        public static uint UTF8Length(this string s)
        {
            byte[] tmp = System.Text.Encoding.UTF8.GetBytes(s);
            return (uint)tmp.Length;
        }
        public static uint UnicodeLength(this string s)
        {
            byte[] tmp = System.Text.Encoding.Unicode.GetBytes(s);
            return (uint)tmp.Length;
        }

        public static Stream ToStream(this byte[] b)
        {
            Stream s = new MemoryStream();
            s.Read(b,0,b.Length);
            return s;
        }
    }

    public class ObjectIsNotCloneableException : Exception
    {
        public ObjectIsNotCloneableException() { }

        public ObjectIsNotCloneableException(string message) : base(message) { }

        public ObjectIsNotCloneableException(string message, Exception inner) : base(message, inner) { }
    }
}
