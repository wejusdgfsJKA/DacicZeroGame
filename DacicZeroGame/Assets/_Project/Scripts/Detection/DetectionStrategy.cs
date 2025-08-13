namespace Detection
{
    public enum DetectionStrategy
    {
        /// <summary>
        /// Goes over all entities in the manager.
        /// </summary>
        Default
    }
    public interface IDetectionStrategy
    {
        void Detect(DetectionMemory memory);
    }
}