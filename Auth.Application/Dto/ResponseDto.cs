namespace Auth.Application.Dto
{
    public class ResponseDto
    {
        protected ResponseDto()
        {
            
        }

        public object? Data { get; private set; }
        public bool IsSuccess { get; private set; } = true;
        public string Message { get; private set; } = "";

        public static ResponseDto Create()
        {
            return new ResponseDto { };
        }

        public ResponseDto CreateError(string error)
        {
            IsSuccess = false;
            Message = error;
            return this;
        }

        public ResponseDto Successful()
        {
            IsSuccess = true;
            Message = "عملیات با موفقیت انجام شد";
            return this;
        }

        public ResponseDto Successful(object data)
        {
            IsSuccess = true;
            Message = "عملیات با موفقیت انجام شد";
            Data = data;

            return this;
        }

        public ResponseDto Successful(string message, object data = null)
        {
            IsSuccess = true;
            Message = message;
            Data = data;

            return this;
        }
    }
}
