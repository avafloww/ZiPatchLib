﻿using System.IO;
using ZiPatchLib.Helpers;
using ZiPatchLib.Util;

namespace ZiPatchLib.Chunk.SqpkCommand
{
    public class SqpkDeleteData : SqpkChunk
    {
        public new static string Command = "D";


        public SqpackDatFile TargetFile { get; protected set; }
        public int BlockOffset { get; protected set; }
        public int BlockNumber { get; protected set; }


        public SqpkDeleteData(ChecksumBinaryReader reader, int size) : base(reader, size) {}

        protected override void ReadChunk()
        {
            var start = reader.BaseStream.Position;

            reader.ReadBytes(3); // Alignment

            TargetFile = new SqpackDatFile(reader);

            BlockOffset = reader.ReadInt32BE() << 7;
            BlockNumber = reader.ReadInt32BE();

            reader.ReadUInt32(); // Reserved

            reader.ReadBytes(Size - (int)(reader.BaseStream.Position - start));
        }

        public override void ApplyChunk(ZiPatchConfig config)
        {
            TargetFile.ResolvePath(config.Platform);

            var file = config.Store == null ?
                TargetFile.OpenStream(config.GamePath, FileMode.OpenOrCreate) :
                TargetFile.OpenStream(config.Store, config.GamePath, FileMode.OpenOrCreate);

            SqpackDatFile.WriteEmptyFileBlockAt(file, BlockOffset, BlockNumber);
        }

        public override string ToString()
        {
            return $"{Type}:{Command}:{TargetFile}:{BlockOffset}:{BlockNumber}";
        }
    }
}
