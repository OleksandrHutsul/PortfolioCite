using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioCite.App.Components.Shared;

internal class FieldValidation : IDisposable
{
    private readonly EditContext _editContext;
    private readonly ValidationMessageStore _store;
    private readonly FieldIdentifier _field;
    private readonly Func<string?> _error;
    private string? _message;
    private bool _disposed;

    public FieldValidation(EditContext editContext, FieldIdentifier field, Func<string?> error)
    {
        _editContext = editContext;
        _field = field;
        _error = error;
        _store = new ValidationMessageStore(editContext);
        _editContext.OnValidationRequested += OnValidationRequested;
        _editContext.OnFieldChanged += OnFieldChanged;
        Apply(notify: false);
    }

    public void Refresh()
    {
        Apply(notify: false);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _editContext.OnValidationRequested -= OnValidationRequested;
        _editContext.OnFieldChanged -= OnFieldChanged;
        var hadMessage = _message is not null;
        _message = null;
        _store.Clear(_field);

        if (hadMessage)
            _editContext.NotifyValidationStateChanged();
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (e.FieldIdentifier.Equals(_field))
            Apply(notify: true);
    }

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        Apply(notify: true);
    }

    private void Apply(bool notify)
    {
        var error = _error();

        if (error == _message)
        {
            if (notify)
                _editContext.NotifyValidationStateChanged();

            return;
        }

        _store.Clear(_field);

        if (error is not null)
            _store.Add(_field, error);

        _message = error;
        _editContext.NotifyValidationStateChanged();
    }
}
