namespace ImportCoordinator.Contracts
{
    public class Response<T>
    {
        public static readonly Response<T> Empty = new Response<T>();

        public T Result { get; set; }

        public string Error { get; set; }

        public Response<T> SetResult(T value)
        {
            Result = value;

            return this;
        }

        public Response<T> SetError(string value)
        {
            Error = value;

            return this;
        }
    }


    public class ResponseVoid<T>
    {
        public static readonly ResponseVoid<T> Empty = new ResponseVoid<T>();

        public T Error { get; set; }

        public ResponseVoid<T> SetError(T value)
        {
            Error = value;

            return this;
        }
    }


    public class ResponseVoid : ResponseVoid<string>
    {
    }


    public class Response<TR, TE>
    {
        public static readonly Response<TR, TE> Empty = new Response<TR, TE>();

        public TR Result { get; set; }

        public TE Error { get; set; }

        public Response<TR, TE> SetResult(TR value)
        {
            Result = value;

            return this;
        }

        public Response<TR, TE> SetError(TE value)
        {
            Error = value;

            return this;
        }
    }
}
