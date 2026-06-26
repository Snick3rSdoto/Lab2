namespace HotelBookingApi.Services
{
    public enum ServiceStatus
    {
        Success,
        NotFound,
        Conflict,
        BadRequest
    }

    public class ServiceResult<T>
    {
        private ServiceResult(ServiceStatus status, T? data, string? message)
        {
            Status = status;
            Data = data;
            ErrorMessage = message;
        }

        public ServiceStatus Status { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        public bool IsSuccess => Status == ServiceStatus.Success;
        public bool IsNotFound => Status == ServiceStatus.NotFound;
        public bool IsConflict => Status == ServiceStatus.Conflict;
        public bool IsBadRequest => Status == ServiceStatus.BadRequest;

        public static ServiceResult<T> Success(T data) => new(ServiceStatus.Success, data, null);
        public static ServiceResult<T> NotFound(string message) => new(ServiceStatus.NotFound, default, message);
        public static ServiceResult<T> Conflict(string message) => new(ServiceStatus.Conflict, default, message);
        public static ServiceResult<T> BadRequest(string message) => new(ServiceStatus.BadRequest, default, message);
    }
}
