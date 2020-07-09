namespace Messages
{
    #region Neutral/system messages

    public class ContinueProcessing
    {
    }

    #endregion

    #region Sucess messages

    // <summary>
    // Base class for signalling valid user input.
    // </summary>
    public class InputSuccess
    {
        public InputSuccess(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }

    #endregion

    #region Error messages

    // <summary>
    // Base class for signalling invalid user input.
    // </summary>
    public class InputError
    {
        public InputError(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; private set; }
    }

    // <summary>
    // Empty user input.
    // </summary>
    public class NullInputError : InputError
    {
        public NullInputError(string reason) : base(reason)
        {
        }
    }

// <summary>
// User provided invalid input. (Currently, an odd number of chars.)
// </summary>
    public class ValidationError : InputError
    {
        public ValidationError(string reason) : base(reason)
        {
        }
    }

    #endregion
}