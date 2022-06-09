using System.Numerics;
using Anabasis.Core;

namespace Anabasis.Ascension;

public class Camera2D
{
    private readonly AnabasisGraphicsDevice _graphics;

    public Camera2D(AnabasisGraphicsDevice graphics) {
        _graphics = graphics;
    }

    public Vector2 Position {
        get => _position;
        set {
            _transform = null;
            _position = value;
        }
    }

    public float Zoom {
        get => _zoom;
        set {
            _transform = null;
            _zoom = value;
        }
    }

    public Matrix4x4 Transform =>
        _transform ??= Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0)
                       * Matrix4x4.CreateScale(Zoom)
                       * Matrix4x4.CreateTranslation(_graphics.ViewportSize.X / 2f, _graphics.ViewportSize.Y / 2f, 0);

    private float      _zoom;
    private Vector2    _position;
    private Matrix4x4? _transform;
}