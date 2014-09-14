﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace VVVV.Packs.VObject
{
    // class for high level and simple object emulation in VVVV
    public class VObjectCollection
    {
        public Dictionary<string, VObject> Children = new Dictionary<string, VObject>();
        public string Name = "";
        public Stopwatch Age = new Stopwatch();
        public string Debug = "";
        public bool Remove = false;
        public VObjectCollection()
        {
            this.Age.Reset();
            this.Age.Start();
        }

        public VObjectCollection DeepCopy()
        {
            VObjectCollection NewObject = new VObjectCollection();
            NewObject.Name = this.Name;
            NewObject.Debug = this.Debug;

            foreach (KeyValuePair<string, VObject> kvp in this.Children)
            {
                NewObject.Children.Add(kvp.Key, kvp.Value.DeepCopy());
            }
            return NewObject;
        }
        public VObjectCollection DeepCopy(string name)
        {
            VObjectCollection NewObject = new VObjectCollection();
            NewObject.Name = name;
            NewObject.Debug = this.Debug;

            foreach (KeyValuePair<string, VObject> kvp in this.Children)
            {
                NewObject.Children.Add(kvp.Key, kvp.Value.DeepCopy());
            }
            return NewObject;
        }
        public void Dispose()
        {
            foreach (KeyValuePair<string, VObject> kvp in this.Children)
            {
                kvp.Value.Dispose();
            }
        }

        public VObject CreateVObject()
        {
            return new VObjectCollectionWrap(this);
        }
    }
    // Wrap in VObject
    public class VObjectCollectionWrap : VObject
    {
        public VObjectCollectionWrap(VObjectCollection o) : base(o.GetType(), o) { }
        public VObjectCollectionWrap(Stream s) : base(typeof(VObjectCollection), s) { }
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
                VObjectCollection ThisContent = this.Content as VObjectCollection;
                ThisContent.Dispose();
                this.Serialized.Dispose();
            }
            disposed = true;
        }
        public override Stream Serialize()
        {
            VObjectCollection ThisContent = this.Content as VObjectCollection;
            Stream dest = this.Serialized;
            dest.SetLength(0);
            dest.Position = 0;

            dest.WriteUint((uint)ThisContent.Children.Count); // 0 | 4
            dest.WriteUint(ThisContent.Name.UnicodeLength()); // 4 | 4
            dest.WriteUint(ThisContent.Debug.UnicodeLength()); // 8 | 4

            foreach (KeyValuePair<string, VObject> kvp in ThisContent.Children) // 12 | CC*4
            {
                uint l = (uint)kvp.Value.Serialize().Length; // serialized here
                l += kvp.Key.UnicodeLength();
                l += kvp.Value.GetType().ToString().UnicodeLength() + 8;
                dest.WriteUint(l);
            }

            dest.WriteUnicode(ThisContent.Name); // 12 + CC*4 | NL
            dest.WriteUnicode(ThisContent.Debug); // 12 + CC*4 + NL | DL

            foreach (KeyValuePair<string, VObject> kvp in ThisContent.Children) // 12 + CC*4 + NL + DL
            {
                dest.WriteUint(kvp.Key.UnicodeLength()); // 0 | 4
                dest.WriteUint(kvp.Value.GetType().ToString().UnicodeLength()); // 0 | 4
                dest.WriteUnicode(kvp.Key); // 4 | KL
                dest.WriteUnicode(kvp.Value.GetType().ToString());

                kvp.Value.Serialized.CopyTo(dest); // 4 + KL | CL // using the stream created above
            }
            return dest;
        }
        protected override void DeSerialize(Stream Input)
        {
            base.DeSerialize(Input);
            VObjectCollection ThisContent = new VObjectCollection();

            uint Count = this.Serialized.ReadUint();
            uint NameL = this.Serialized.ReadUint();
            uint DebugL = this.Serialized.ReadUint();

            List<uint> ChildrenLengths = new List<uint>();
            for (int i = 0; i < Count; i++)
            {
                ChildrenLengths.Add(this.Serialized.ReadUint());
            }

            ThisContent.Name = this.Serialized.ReadUnicode((int)NameL);
            ThisContent.Debug = this.Serialized.ReadUnicode((int)DebugL);

            for (int i = 0; i < Count; i++)
            {
                uint keylength = this.Serialized.ReadUint();
                uint typelength = this.Serialized.ReadUint();
                string keyname = this.Serialized.ReadUnicode((int)keylength);
                string typename = this.Serialized.ReadUnicode((int)typelength);

                uint l = ChildrenLengths[i] - keylength - typelength - 8;
                Stream child = new MemoryStream();
                Type childtype = Type.GetType(typename);
                this.Serialized.CopyTo(child, (int)l);

                Object[] ConstrArgs = new Object[] {new MemoryStream()};
                ThisContent.Children.Add(
                    keyname,
                    Activator.CreateInstance(childtype, ConstrArgs) as VObject
                );
            }
            this.Content = ThisContent;
        }

        public VObjectCollection Cast()
        {
            return this.Content as VObjectCollection;
        }
    }
}
