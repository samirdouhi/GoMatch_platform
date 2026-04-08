namespace BusinessService.Exceptions
{
    public class ForbiddenBusinessAccessException : Exception
    {
        public ForbiddenBusinessAccessException()
            : base("Accès interdit à cette ressource.") { }
    }
}