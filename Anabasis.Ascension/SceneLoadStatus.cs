using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Anabasis.Ascension;

public class SceneLoadStatus : INotifyPropertyChanged
{
    private float _progress;

    public float Progress {
        get => _progress;
        set => SetField(ref _progress, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (!EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}