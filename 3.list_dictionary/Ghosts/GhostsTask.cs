using System;
using System.Text;

namespace hashes;

public class GhostsTask :
    IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>,
    IMagic
{
    private static readonly Encoding _encoding = Encoding.UTF8;
    private static byte[] _content = _encoding.GetBytes("Ghost");
    private Vector _vector = new Vector(0, 0);
    private Segment _segment = new Segment(new Vector(0, 0), new Vector(1, 1));
    private Cat _cat = new Cat("Ghost", "Siamese", new DateTime(2026, 3, 1));
    private Robot _robot = new Robot("Ghost", 1);

    public void DoMagic()
    {
        _content[0]++;
        _vector.Add(new Vector(1, 1));
        _segment.Start.Add(new Vector(1, 1));
        _cat.Rename("NotGhost");
        Robot.BatteryCapacity += 1;
    }

    Document IFactory<Document>.Create()
    {
        return new Document("Ghost", _encoding, _content);
    }

    Vector IFactory<Vector>.Create()
    {
        return _vector;
    }

    Segment IFactory<Segment>.Create()
    {
        return _segment;
    }

    Cat IFactory<Cat>.Create()
    {
        return _cat;
    }

    Robot IFactory<Robot>.Create()
    {
        return _robot;
    }
}
