using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CharecterScripts
{
    public class TransformState : INetworkSerializable
    {
        public int _tick;
        public Vector3 _position;
        public Quaternion _rotation;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out _tick);
                reader.ReadValueSafe(out _position);
                reader.ReadValueSafe(out _rotation);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(_tick);
                writer.WriteValueSafe(_position);
                writer.WriteValueSafe(_rotation);
            }

        }
    }
}