using System.Numerics;
using Anabasis.Core;
using Anabasis.Core.Graphics;

namespace Anabasis.Ascension;

public class Camera2D
{
    private readonly AnabasisGraphicsDevice _graphics;

    public Camera2D(AnabasisGraphicsDevice graphics) {
        _graphics = graphics;
        _graphics.ViewportChanged += () => _proj = null;
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

    public Matrix4x4 View =>
        _transform ??= Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0) * Matrix4x4.CreateScale(Zoom);

    public Matrix4x4 Projection =>
        _proj ??= Matrix4x4.CreateOrthographic(_graphics.ViewportSize.X, _graphics.ViewportSize.Y, -1, 1);

    private float      _zoom;
    private Vector2    _position;
    private Matrix4x4? _transform;
    private Matrix4x4? _proj;
}