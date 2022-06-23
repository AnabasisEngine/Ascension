namespace Anabasis.Ascension.Assimp;

public class AssimpException : Exception
{
    public AssimpException(string? message) : base(message) { }
    public AssimpException(string? message, Exception? innerException) : base(message, innerException) { }
}