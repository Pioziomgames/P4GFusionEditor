using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4GFusionEditor
{
    public class BinChunk
    {
        public byte[] Data;
        public string Name;
        public BinChunk()
        {

        }
        public BinChunk(BinaryReader reader)
        {
            string name = new string(reader.ReadChars(32));
            Name = name.Replace("\0", "");
            int size = reader.ReadInt32();
            Data = reader.ReadBytes(size);
        }
        public BinChunk(byte [] data,string name)
        {
            Data = data;
            Name = name;
        }
        public void Save(BinaryWriter writer)
        {
            Write(writer);
        }
        void Write(BinaryWriter writer)
        {
            writer.Write(Name.ToCharArray());
            for (int i = 0; i < 36 - Name.Length; i++)
            {
                writer.Write((byte)0);
            }
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
    public class Bin
    {
        public List<BinChunk> Files;
        public Bin(string Path)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Path)))
                ReadBin(br);
        }
        public Bin(BinaryReader br)
        {
            ReadBin(br);
        }
        public Bin(byte[] byteArray)
        {
            MemoryStream stream = new MemoryStream(byteArray);
            ReadBin(new BinaryReader(stream));
        }
        public Bin()
        {
            Files = new List<BinChunk>();
        }
        public void Save(string Path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(Path, FileMode.Create)))
                WriteBin(writer);
        }
        public void Save(BinaryWriter writer)
        {
            WriteBin(writer);
        }
        void WriteBin(BinaryWriter writer)
        {
            writer.Write(Files.Count);
            foreach (BinChunk binChunk in Files)
                binChunk.Save(writer);
        }
        void ReadBin(BinaryReader reader)
        {
            uint FileCount = reader.ReadUInt32();
            Files = new List<BinChunk>();

            for (int i = 0; i < FileCount; i++)
                Files.Add(new BinChunk(reader));
        }

        public int FindBinIndex(string BinName)
        {
            int result = -1;
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Name == BinName)
                {
                    result = i;
                }
            }
            return result;
        }


    }

    public class Fcl
    {
        public List<FclEntry> Entries = new List<FclEntry>();

        public Fcl()
        {

        }
        public Fcl(byte[] byteArray)
        {
            MemoryStream stream = new MemoryStream(byteArray);
            Read(new BinaryReader(stream));
        }

        public Fcl(string Path)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Path)))
                Read(br);
        }
        public Fcl(BinaryReader br)
        {
            Read(br);
        }

        void Read(BinaryReader reader)
        {
            uint Size = reader.ReadUInt32();
            uint EntryCount = reader.ReadUInt32();
            reader.BaseStream.Seek(8, SeekOrigin.Current);
            for (int i = 0; i < EntryCount; i++)
                Entries.Add(new FclEntry(reader));
        }

        void Write(BinaryWriter writer)
        {
            writer.Write(Entries.Count * 44);
            writer.Write(Entries.Count);
            for (int i = 0; i < 8; i++)
                writer.Write((byte)0);
            for (int i = 0; i < Entries.Count;i++)
                Entries[i].Save(writer);
        }

        public void Save(BinaryWriter writer)
        {
            Write(writer);
        }

    }
    public class FclEntry
    {
        public ushort ResultId { get; set; }
        public List<ushort> MaterialIds { get; set; }
        public ushort Material1 { get; set; }
        public ushort Material2 { get; set; }
        public ushort Material3 { get; set; }
        public ushort Material4 { get; set; }
        public ushort Material5 { get; set; }
        public ushort Material6 { get; set; }
        public ushort Material7 { get; set; }
        public ushort Material8 { get; set; }
        public ushort Material9 { get; set; }
        public ushort Material10 { get; set; }
        public ushort Material11 { get; set; }
        public ushort Material12 { get; set; }


        public FclEntry()
        {
            MaterialIds = new List<ushort>();
        }

        public FclEntry(string Path)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Path)))
                Read(br);
        }
        public FclEntry(BinaryReader reader)
        {
            Read(reader);
        }
        void Read(BinaryReader reader)
        {
            
            ResultId = reader.ReadUInt16();
            reader.BaseStream.Seek(10, SeekOrigin.Current);
            Material1 = reader.ReadUInt16();
            Material2 = reader.ReadUInt16();
            Material3 = reader.ReadUInt16();
            Material4 = reader.ReadUInt16();
            Material5 = reader.ReadUInt16();
            Material6 = reader.ReadUInt16();
            Material7 = reader.ReadUInt16();
            Material8 = reader.ReadUInt16();
            Material9 = reader.ReadUInt16();
            Material10 = reader.ReadUInt16();
            Material11 = reader.ReadUInt16();
            Material12 = reader.ReadUInt16();
            reader.BaseStream.Seek(8, SeekOrigin.Current);

            MaterialIds = new List<ushort>();
            MaterialIds.Add(Material1);
            MaterialIds.Add(Material2);
            MaterialIds.Add(Material3);
            MaterialIds.Add(Material4);
            MaterialIds.Add(Material5);
            MaterialIds.Add(Material6);
            MaterialIds.Add(Material7);
            MaterialIds.Add(Material8);
            MaterialIds.Add(Material9);
            MaterialIds.Add(Material10);
            MaterialIds.Add(Material11);
            MaterialIds.Add(Material12);
        }

        void Write(BinaryWriter writer)
        {
            writer.Write(ResultId);
            for (int i = 0; i < 10; i++)
                writer.Write((byte)0);
            for (int i = 0; i < MaterialIds.Count; i++)
                writer.Write(MaterialIds[i]);
            for (int i = 0; i < (12 - MaterialIds.Count); i++)
                writer.Write((ushort)0);
            for (int i = 0; i < 8; i++)
                writer.Write((byte)0);
        }
        public void Save(BinaryWriter writer)
        {
            Write(writer);
        }

        
    }
    
}
