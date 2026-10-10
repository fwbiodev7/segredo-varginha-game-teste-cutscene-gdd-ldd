using UnityEngine;

namespace Game.Varginha
{
    [DefaultExecutionOrder(11000), DisallowMultipleComponent]
    public sealed class VarginhaCharacterFrameGeometryRunner : MonoBehaviour
    {
        private void LateUpdate() => VarginhaCharacterFrameGeometry.ApplyPending();
    }
}
