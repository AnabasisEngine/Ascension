namespace Anabasis.Ascension;

public sealed class BindUnitPool
{
    private ulong _mask;
    private int  _next;

    public bool TryTake(out int unit) {
        if (_next > 63) {
            unit = 0;
            return false;
        }

        unit = _next;
        _mask |= 1u << _next;
        
        while ((_mask & (1u << ++_next)) != 0) { }

        return true;
    }

    public void Return(int unit) {
        _mask ^= 1u << unit;
        if (_next > unit)
            _next = unit;
    }
}