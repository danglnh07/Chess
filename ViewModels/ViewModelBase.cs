using System.ComponentModel;

namespace Chess.ViewModels
{
    /// <summary>
    /// Base class of all View Model classes. Implement the INotifyPropertyChanged interface
    /// </summary>
    class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Inplementation of PropertyChanged event in INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Implementation of OnPropertyChanged in INotifyPropertyChanged interface. This method will notify the listener (mostly, the UI)
        /// that the property it's bounded to has changed, so that the UI can automatically changed 
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
