namespace KeepAlive.Client.External;

/// <summary>
///     Represents errors that occur while calling external
///     methods.
/// </summary>
public class ExternalException : Exception
{
    /// <summary>
    ///     Initializes a new instance of <see cref="ExternalException"/>.
    /// </summary>
    /// <param name="agent">
    ///     The external agent to get the error message from.
    /// </param>
    /// <param name="errorMessage">
    ///     The error message to use if unable to retrieve from the agent.
    /// </param>
    public ExternalException(
        IExternalAgent agent,
        string? errorMessage = null) :
        base(agent.GetErrorMessage() ?? errorMessage)
    {   
    }
}
