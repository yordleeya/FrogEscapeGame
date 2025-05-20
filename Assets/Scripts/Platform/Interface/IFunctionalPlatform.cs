using UnityEngine;

public interface IFunctionalPlatform
{
    virtual void OnAttached(Rigidbody2D rigid, RigidbodyType2D bodyType)
    {
        rigid.bodyType = bodyType;
    }

    void OnDettaced(Rigidbody2D rigid);
}
