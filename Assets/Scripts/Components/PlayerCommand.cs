using System;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct PlayerCommand : IComponentData
    {
        public enum Button : uint
        {
            None = 0,
            Move = 1 << 0,
            Boost = 1 << 1,
            PrimaryFire = 1 << 2,
            SecondaryFire = 1 << 3,
            Reload = 1 << 4,
            Melee = 1 << 5,
            Use = 1 << 6,
            Ability1 = 1 << 7,
            Ability2 = 1 << 8,
            Ability3 = 1 << 9,
        }

        public struct ButtonBitField
        {
            public uint flags;

            public bool IsSet(Button button)
            {
                return (flags & (uint)button) > 0;
            }

            public void Or(Button button, bool val)
            {
                if (val)
                    flags = flags | (uint)button;
            }


            public void Set(Button button, bool val)
            {
                if (val)
                    flags = flags | (uint)button;
                else
                {
                    flags = flags & ~(uint)button;
                }
            }
        }

        public int renderTick;
        public ButtonBitField buttons;
        public Vector3 targetPos;
        public bool isBack;
     


        public void Reset()
        {
            renderTick = 0;
            buttons.flags = 0;
            targetPos = Vector3.zero;
            isBack = false;
        }

        public byte[] ToData()
        {
            MemoryStream memStream = new MemoryStream(100);
            BinaryWriter writer = new BinaryWriter(memStream);

            writer.Write(renderTick);
            writer.Write(buttons.flags);
            writer.Write(targetPos.x);
            writer.Write(targetPos.y);
            writer.Write(targetPos.z);

            return memStream.ToArray();
        }

        public void FromData(byte[] data)
        {
            var memStream = new MemoryStream(data);
            var reader = new BinaryReader(memStream);

            renderTick = reader.ReadInt32();
            buttons.flags = reader.ReadUInt32();
            targetPos.x = reader.ReadSingle();
            targetPos.y = reader.ReadSingle();
            targetPos.z = reader.ReadSingle();           
        }
    }

}