
namespace PL
{
    /// <summary>
    /// Interface for all windows in the application that can be refreshed.
    /// </summary>
    public interface IRefreshable
    {
        /// <summary>
        /// Refresh the data of the window.
        /// </summary>
        public void refresh();
    }
}
