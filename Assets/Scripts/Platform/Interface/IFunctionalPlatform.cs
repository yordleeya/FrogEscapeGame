using UnityEngine;

public interface IFunctionalPlatform
{
    void OnAttached(Rigidbody2D rigid, RigidbodyType2D bodyType);

    void OnDettaced(Rigidbody2D rigid);
}
