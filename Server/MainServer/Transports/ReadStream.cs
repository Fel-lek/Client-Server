using System.Buffers.Binary;
using System.Text;

public class PacketReader
{
    private byte[] _buffer;
    public int _position;

    public PacketReader(byte[] buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public int ReadInt()
    {
        int value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(_position));
        _position += sizeof(int);
        return value;
    }

    public short ReadShort()
    {
        short value = BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(_position));
        _position += sizeof(short);
        return value;
    }
    public byte ReadByte()
    {
        _position += sizeof(byte);
        return _buffer[_position++];
    }

    public float ReadFloat()
    {
        return BitConverter.Int32BitsToSingle(ReadInt());
    }

    public string ReadString()
    {
        int length = ReadInt();
        string value = Encoding.UTF8.GetString(_buffer, _position, length);
        _position += length;
        return value;
    }
}

public class ResizableWriter
{
    public int _position { get; set; }
    public byte[] _buffer { get; private set; }

    public ResizableWriter(int startSize = 16)
    {
        _buffer = new byte[startSize];
        _position = 0;
    }

    private void Resize(int size)
    {
        // create new buffer that can for sure hold the data
        var newBuffer = new byte[Math.Max(_buffer.Length * 2, _buffer.Length + size)];
        // replace the old buffer
        _buffer.AsSpan().CopyTo(newBuffer);
        _buffer = newBuffer;
    }

    public void Write(int value)
    {
        // check if it fits and resize if needed
        if (_position + sizeof(int) >= _buffer.Length)
        {
            Resize(sizeof(int));
        }

        BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan(_position), value);
        _position += sizeof(int);
    }
    public void Write(string value)
    {
        int count = Encoding.UTF8.GetMaxByteCount(value.Length) + sizeof(int);

        if (_position + count >= _buffer.Length)
        {
            Resize(count);
        }

        int length = Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_position + sizeof(int)));
        Write(length);
        _position += length;
    }

    public void Write(bool value)
    {
        if (_position + sizeof(bool) >= _buffer.Length)
        {
            Resize(sizeof(bool));
        }

        //BitConverter.GetBytes(value).CopyTo(_buffer, _position);
        Write(value ? 1 : 0);
        //_position += sizeof(bool);
    }

    public void Write(byte value)
    {
        if (_position + sizeof(byte) >= _buffer.Length)
        {
            Resize(sizeof(byte));
        }
        _buffer[_position++] = value;
    }

    //public int ReadInt()
    //{
    //    int value = BinaryPrimitives.ReadInt32LittleEndian(_buffer.AsSpan(_position));
    //    _position += sizeof(int);
    //    return value;
    //}

    //public short ReadShort()
    //{
    //    short value = BinaryPrimitives.ReadInt16LittleEndian(_buffer.AsSpan(_position));
    //    _position += sizeof(short);
    //    return value;
    //}
    //public byte ReadByte()
    //{
    //    return _buffer[_position++];
    //}

    //public float ReadFloat()
    //{
    //    return BitConverter.Int32BitsToSingle(ReadInt());
    //}

    //public string ReadString()
    //{
    //    int length = ReadInt();
    //    string value = Encoding.UTF8.GetString(_buffer, _position, length);
    //    _position += length;
    //    return value;
    //}

    //public void Write(string value)
    //{
    //    int length = Encoding.UTF8.GetBytes(value, _buffer.AsSpan(_position + sizeof(int)));
    //    Write(length);
    //    _position += length;
    //}
}