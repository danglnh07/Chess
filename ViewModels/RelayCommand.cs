using System.Windows.Input;

namespace Chess.ViewModels
{
    /// <summary>
    /// The RelayCommand, which implements ICommand interface. Used for View Model classes
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields
        /// <summary>
        /// The _execute delegate, which will perform the action when an event is fired. 
        /// The parameter (object) is nullable, so make sure to do null check in the implementation of _execute
        /// </summary>
        private readonly Action<object?> _execute;

        /// <summary>
        /// The _canExecute delegate, which will determine if this component can execute the action defined by _execute.
        /// The parameter (object) is nullable, so make sure to do null check in the implementation of _execute
        /// </summary>
        private readonly Predicate<object?>? _canExecute;
        #endregion //End Fields

        /// <summary>
        /// Constructor method of class RelayCommand. If canExecute is null, which means that there is no constraint for this action
        /// (similar to passing an always-return-true function)
        /// </summary>
        /// <param name="execute">The execute delegate</param>
        /// <param name="canExecute">The nullable canExecute delegate</param>
        /// <exception cref="ArgumentNullException">If execute is null, then we throw an ArgumentNullException</exception>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            _execute = execute; //execute is not nullable, so we don't have to perform null check here
            _canExecute = canExecute; //Meanwhile, canExecute is nullable, so we also don't have to check anything here
        }

        /// <summary>
        /// Implementation of CanExecuteChanged in ICommand interface. 
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            /*
             * This CanExecuateChanged will automatically be called if the CanExecute() changed -> allow enable and disable of
             * UI elements' interaction.
             * For example, we have a login form, and a submit button. We only allow clicking button after the form has been by some
             * information. Without this event, we have to manually called CanExecuateChanged to tell the UI that the condition has changed
             */
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Implementation of CanExecute method in ICommand interface 
        /// </summary>
        /// <param name="parameter">The nullable object parameter</param>
        /// <returns>Boolean value, true if _canExecute is null (no constraint) or _canExecute(object) return true</returns>
        public bool CanExecute(object? parameter)
        {
            //If _canExecute is null (no constraint), return true. Else, we delegate the decision to _canExecute
            return _canExecute is null || _canExecute(parameter);
        }

        /// <summary>
        /// Implementation of Execute method in ICommand interface
        /// </summary>
        /// <param name="parameter">The nullable object parameter</param>
        public void Execute(object? parameter)
        {
            //We delegate the action performing to _execute
            _execute(parameter);
        }
    }
}
