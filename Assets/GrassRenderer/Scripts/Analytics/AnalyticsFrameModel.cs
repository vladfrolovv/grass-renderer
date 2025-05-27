namespace GrassRenderer.Analytics
{
    public struct AnalyticsFrameModel
    {
        public float DeltaTime;
        public float FPS;
        public int VisibleTriangles;
        public int VisibleVertices;
        public float CPUFrameTimeMs;
        public float GPUFrameTimeMs;
    }
}
