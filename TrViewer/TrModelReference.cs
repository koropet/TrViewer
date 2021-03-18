using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrViewer
{
    public class TRmodelreference
    {
        public string name;
        public int position = 0;
        public TRmodelreference()
        {
            name = "";
        }
        public TRmodelreference(BinaryReader br)
        {
            position = br.ReadInt32();
            byte count = br.ReadByte();
            name = System.Text.Encoding.Default.GetString(br.ReadBytes(count));
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(position);

            if (name.Length > 255) throw new ArgumentException("too long name of model: " + name);
            bw.Write(Convert.ToByte(name.Length));

            bw.Write(System.Text.Encoding.Default.GetBytes(name));
        }
        public override string ToString()
        {
            return string.Format("[TRmodelreference Name={0}, Position={1}]", name, position);
        }
    }
}
