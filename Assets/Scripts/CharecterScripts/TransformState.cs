using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CharecterScripts
{
    public class TransformState : INetworkSerializable
    {
        public int tick;
        public Vector3 position;
        public Quaternion rotation;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out tick);
                reader.ReadValueSafe(out position);
                reader.ReadValueSafe(out rotation);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(tick);
                writer.WriteValueSafe(position);
                writer.WriteValueSafe(rotation);
            }

        }
    }
}