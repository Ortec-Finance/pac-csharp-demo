namespace TaskListAPI
{
    // Each log can specify an event id
    public static class LogEvents
    {
        // Information
        public const int BeginRequest = 901;
        public const int EndRequest = 902;

        public const int CustomersRetrieved = 1301;
        public const int CustomerWithIdRetrieved = 1302;
        public const int CustomerCreated = 1303;
        public const int CustomerWithIdRemoved = 1304;

        // Warning
        public const int DatabaseConcurrencyConflict = 3001;

        public const int CustomerWithIdNotFound = 3401;
        public const int CustomerAlreadyExists = 3402;

        // Error
        public const int InternalError = 5000;
    }
}