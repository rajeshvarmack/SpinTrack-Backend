namespace SpinTrack.Application.Common.Results
{
    /// <summary>
    /// Result pattern implementation for operations with return value
    /// </summary>
    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result");

        protected internal Result(T? value, bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}
