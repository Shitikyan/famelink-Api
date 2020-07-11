using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace FameLink.Common.Models
{
    public class ResponseModel<T> : ResponseModel
    {
        public T Item { get; set; }

        public ResponseModel()
        {
        }

        public ResponseModel(T item)
        {
            Item = item;
        }

        public new static ResponseModel<T> Created() => New(HttpStatusCode.Created);
        public new static ResponseModel<T> NotFound() => New(HttpStatusCode.NotFound);
        public static ResponseModel<T> Accepted(T item) => New(HttpStatusCode.Accepted, item: item);
        public override object GetItem() => Item;

        public static new ResponseModel<T> SetError(string title = "Something went wrong",
                                                    Dictionary<string, List<string>> errors = default,
                                                    HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ResponseModel<T>
            {
                Title = title,
                Errors = errors is null ? new Dictionary<string, List<string>>() : errors,
                StatusCode = statusCode,
            };
        }

        private static ResponseModel<T> New(
            HttpStatusCode statusCode,
            string message = default,
            T item = default) =>
            new ResponseModel<T>
            {
                StatusCode = statusCode,
                Title = message,
                Item = item,
            };
    }

    public class ResponseModel
    {
        public string Title { get; set; }

        public Dictionary<string, List<string>> Errors { get; set; }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        [JsonIgnore]
        public int StatusCodeValue => (int)StatusCode;

        [JsonIgnore]
        public bool IsSuccessStatusCode => StatusCodeValue >= 200 && StatusCodeValue <= 299;

        public static ResponseModel SetError(string title = "Something went wrong",
                                             Dictionary<string, List<string>> errors = default,
                                             HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new ResponseModel
        {
            Title = title,
            Errors = errors is null ? new Dictionary<string, List<string>>() : errors,
            StatusCode = statusCode,
        };

        public virtual object GetItem() => null;

        public static ResponseModel NoContent(string message = default) => New(HttpStatusCode.NoContent, message);
        public static ResponseModel Created() => New(HttpStatusCode.Created);
        public static ResponseModel NotFound() => New(HttpStatusCode.NotFound);
        private static ResponseModel New(HttpStatusCode statusCode, string message = default) =>
        new ResponseModel
        {
            StatusCode = statusCode,
            Title = message,
        };
    }
}
