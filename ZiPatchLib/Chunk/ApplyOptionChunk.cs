﻿using ZiPatchLib.Helpers;

namespace ZiPatchLib.Chunk
{
    public class ApplyOptionChunk : ZiPatchChunk
    {
        public new static string Type = "APLY";

        public enum ApplyOptionKind : uint
        {
            IgnoreMissing = 1,
            IgnoreOldMismatch = 2
        }

        // These are both false on all files seen
        public ApplyOptionKind OptionKind { get; protected set; }

        public bool OptionValue { get; protected set; }

        public ApplyOptionChunk(ChecksumBinaryReader reader, int size) : base(reader, size)
        {}

        protected override void ReadChunk()
        {
            var start = reader.BaseStream.Position;

            OptionKind = (ApplyOptionKind) reader.ReadUInt32BE();

            // Discarded padding, always 0x0000_0004 as far as observed
            reader.ReadBytes(4);

            var value = reader.ReadUInt32BE() != 0;

            if (OptionKind == ApplyOptionKind.IgnoreMissing ||
                OptionKind == ApplyOptionKind.IgnoreOldMismatch)
                OptionValue = value;
            else
                OptionValue = false; // defaults to false if OptionKind isn't valid

            reader.ReadBytes(Size - (int)(reader.BaseStream.Position - start));
        }

        public override void ApplyChunk(ZiPatchConfig config)
        {
            switch (OptionKind)
            {
                case ApplyOptionKind.IgnoreMissing:
                    config.IgnoreMissing = OptionValue;
                    break;
                case ApplyOptionKind.IgnoreOldMismatch:
                    config.IgnoreOldMismatch = OptionValue;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Type}:{OptionKind}:{OptionValue}";
        }
    }
}
