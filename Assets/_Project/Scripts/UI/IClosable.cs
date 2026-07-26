namespace DacicZero.UI {
    /// <summary> generic interface for standardizing how ui panels close. </summary>
    public interface IClosable {
        void Close();
        bool IsOpen { get; }
    }
}
