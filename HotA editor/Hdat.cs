﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace HotA_editor
{
    class Hdat
    {
        internal ObservableCollection<HdatEntry> ReadFile(string path, Encoding enc)
        {
            var entries = new ObservableCollection<HdatEntry>();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            using (var read = new BinaryReader(File.Open(path, FileMode.Open), enc))
            {
                var reader = read;

                var s = ReadString(ref reader, 4) + reader.ReadInt32();
                if (s != "HDAT2")
                    return null;

                var noOfFiles = reader.ReadInt32();

                for (var i = 0; i < noOfFiles; i++)
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var tmp = new HdatEntry(); 
                    tmp.Name = ReadString(ref reader, reader.ReadInt32());
                    tmp.FolderName = ReadString(ref reader, reader.ReadInt32());

                    tmp.Int1 = reader.ReadInt32();
                    tmp.Int2 = reader.ReadInt32();

                    if(tmp.Int2 > 0)
                    {
                        tmp.Data1 = ReadString(ref reader, tmp.Int2);
                    }
                    else
                    {
                        tmp.Data1 = ReadString(ref reader, reader.ReadInt32());
                    }

                    tmp.Data2 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data3 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data4 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data5 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data6 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data7 = ReadString(ref reader, reader.ReadInt32());
                    tmp.Data8 = ReadString(ref reader, reader.ReadInt32());

                    if (tmp.Int2 > 0)
                    {
                        tmp.NewData = ReadString(ref reader, reader.ReadInt32());
                    }

                    var exists = reader.ReadBoolean();

                    // chceck if extra data exist
                    if (exists)
                    {
                        tmp.Data9 = ReadExtraData(ref reader, reader.ReadInt32());
                    }

                    tmp.Data10 = ReadInts(ref reader, reader.ReadInt32());

                    entries.Add(tmp);
                }
            }
            return entries;
        }

        private static string ReadString(ref BinaryReader stream, int length)
        {
            return new string(stream.ReadChars(length));
        }

        private static Boolean ExtraDataExist(ref BinaryReader stream)
        {
            return stream.ReadBoolean();
        }

        private static byte[] ReadExtraData(ref BinaryReader stream, int length)
        {
            return stream.ReadBytes(length);
        }

        private static int[] ReadInts(ref BinaryReader stream, int length)
        {
            var ret = new int[length];
            for (var i = 0; i < length; i++)
            {
                ret[i] += stream.ReadInt32();
            }
            return ret;
        }

        internal void WriteFile(string path, ObservableCollection<HdatEntry> entries, Encoding enc)
        {
            using (var write = new BinaryWriter(File.Open(path, FileMode.Create), enc))
            {
                var writer = write;

                writer.Write(new []{'H', 'D', 'A', 'T'});
                writer.Write(2);

                writer.Write(entries.Count);

                foreach (var t in entries)
                {
                    WriteString(ref writer, t.Name);
                    WriteString(ref writer, t.FolderName);

                    writer.Write(t.Int1);

                    if (t.Int2 == 0)
                    {
                        writer.Write(t.Int2);
                    }

                    WriteString(ref writer, t.Data1);
                    WriteString(ref writer, t.Data2);
                    WriteString(ref writer, t.Data3);
                    WriteString(ref writer, t.Data4);
                    WriteString(ref writer, t.Data5);
                    WriteString(ref writer, t.Data6);
                    WriteString(ref writer, t.Data7);
                    WriteString(ref writer, t.Data8);


                    if (t.Int2 > 0)
                    {
                        WriteString(ref writer, t.NewData);
                    }

                    // chceck if extra data exist
                    if (t.Data9 != null)
                    {
                        writer.Write(true);
                        WriteExtraData(ref writer, t.Data9);
                    }
                    else
                    {
                        writer.Write(false);
                    }

                    WriteInts(ref writer, t.Data10, true);
                }
            }
        }

        private static void WriteString(ref BinaryWriter stream, string text)
        {
            if (text == null)
            {
                stream.Write(0);
                return;
            }

            stream.Write(text.Length);

            foreach (var t in text)
            {
                stream.Write(t);
            }
        }

        private static void WriteExtraData(ref BinaryWriter stream, byte[] bytes)
        {
            if (bytes == null)
            {
                stream.Write(0);
                return;
            }

            stream.Write(bytes.Length);
            stream.Write(bytes);
        }

        private static void WriteInts(ref BinaryWriter stream, int[] bytes, bool writeLength)
        {
            if (bytes == null)
            {
                stream.Write(0);
                return;
            }

            if (writeLength)
            {
                stream.Write(bytes.Length);
            }

            foreach (var t in bytes)
            {
                stream.Write(t);
            }
        }
    }
}
